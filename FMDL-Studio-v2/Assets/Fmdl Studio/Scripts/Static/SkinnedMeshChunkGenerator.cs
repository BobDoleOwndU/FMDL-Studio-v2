using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FmdlStudio.Scripts.Classes;

namespace FmdlStudio.Scripts.Static
{
    public static class SkinnedMeshChunkGenerator
    {
        public static List<SkinnedMeshChunk> GenerateMeshChunks(SkinnedMeshRenderer sourceSkinnedMeshRenderer, bool splitByLooseParts, bool splitByBonesCount, int bonesPerMeshBudget)
        {
            // Start chunk, required for removing unused bones without affecting the source mesh - even if no further chunking is necessary
            Mesh mesh = CloneMeshPart(sourceSkinnedMeshRenderer.sharedMesh, new List<int>(sourceSkinnedMeshRenderer.sharedMesh.triangles), new List<int>(), new Dictionary<int, int>());
            List<SkinnedMeshChunk> chunks = new List<SkinnedMeshChunk>() { new SkinnedMeshChunk(sourceSkinnedMeshRenderer, mesh, 0, bonesPerMeshBudget) };

            // First split by loose parts as it does not create new vertices
            if (splitByLooseParts)
            {
                for (int chunkIndex = chunks.Count - 1; chunkIndex >= 0; --chunkIndex)
                {
                    SkinnedMeshChunk chunk = chunks[chunkIndex];
                    if (!chunk.WithinBonesCountBudget)
                    {
                        Debug.Log("Mesh (" + chunk.gameObject.name + ") has too many bone influences and will be split by loose parts.");

                        // remove source chunk
                        chunks.RemoveAt(chunkIndex);

                        // create sub-chunks
                        chunks.AddRange(GenerateMeshChunksByLooseParts(chunk, bonesPerMeshBudget));

                        // remove temporary GameObject of source chunk
                        chunk.DestroyAndClear();
                    }
                }
            }

            // Split by bones count - this will create new vertices
            if (splitByBonesCount)
            {
                for (int chunkIndex = chunks.Count - 1; chunkIndex >= 0; --chunkIndex)
                {
                    SkinnedMeshChunk chunk = chunks[chunkIndex];
                    if (!chunk.WithinBonesCountBudget)
                    {
                        Debug.Log("Mesh (" + chunk.gameObject.name + ") has still too many bone influences and will be split by max bone influences.");

                        // remove source chunk
                        chunks.RemoveAt(chunkIndex);

                        // create sub-chunks
                        chunks.AddRange(GenerateMeshChunksByBonesCount(chunk, bonesPerMeshBudget));

                        // remove temporary GameObject of source chunk
                        chunk.DestroyAndClear();
                    }
                }
            }

            // this here should not be possible if at least the max-bones chunking method is active
            for (int chunkIndex = chunks.Count - 1; chunkIndex >= 0; --chunkIndex)
            {
                if (!chunks[chunkIndex].WithinBonesCountBudget)
                {
                    ClearChunks(chunks);
                    EditorUtility.DisplayDialog("Bones limit exceeded!", "A mesh cannot be weighted to more than " + bonesPerMeshBudget + " bones!", "Ok");
                    throw new Exception("A mesh cannot be weighted to more than " + bonesPerMeshBudget + " bones!");
                }
            }

            return chunks;
        }

        public static void ClearChunks(List<SkinnedMeshChunk> chunks)
        {
            for (int chunkIndex = chunks.Count - 1; chunkIndex >= 0; --chunkIndex)
            {
                chunks[chunkIndex].DestroyAndClear();
            }
            chunks.Clear();
        }


        /* Chunks by loose parts ######################################################################################### */

        private static List<SkinnedMeshChunk> GenerateMeshChunksByLooseParts(SkinnedMeshChunk sourceChunk, int bonesPerMeshBudget)
        {
            List<List<int>> chunksTriangles = ChunkTrianglesByLooseParts(sourceChunk.Mesh);

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Write chunks (loose parts method):", 0.6f);

            List<SkinnedMeshChunk> meshChunks = new List<SkinnedMeshChunk>(chunksTriangles.Count);
            for (int chunkIndex = 0; chunkIndex < chunksTriangles.Count; ++chunkIndex)
            {
                List<int> chunkTriangles = chunksTriangles[chunkIndex];
                GetMappingsBetweenNewAndOldVertices(chunkTriangles, out List<int> new2oldVertexMap, out Dictionary<int, int> old2newVertexMap);

                Mesh chunkMesh = CloneMeshPart(sourceChunk.Mesh, chunkTriangles, new2oldVertexMap, old2newVertexMap);

                SkinnedMeshChunk skinnedMeshChunk = new SkinnedMeshChunk(sourceChunk.skinnedMeshRenderer, chunkMesh, chunkIndex, bonesPerMeshBudget);
                meshChunks.Add(skinnedMeshChunk);
            }

            return meshChunks;
        }

        private static List<List<int>> ChunkTrianglesByLooseParts(Mesh mesh)
        {
            List<List<int>> chunksTriangles = new List<List<int>>();
            Dictionary<int, int> vertex2chunk = new Dictionary<int, int>(mesh.vertexCount);
            List<HashSet<int>> chunksVertices = new List<HashSet<int>>();

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Building proto chunks (loose parts method):", 0.2f);

            // Never ever access mesh arrays inside a loop as it will currently create throw-away copies each time
            int[] meshTriangles = mesh.triangles;

            for (int t = 0; t < meshTriangles.Length; t += 3)
            {
                // Get vertex indices of triangle
                int v0 = meshTriangles[t];
                int v1 = meshTriangles[t + 1];
                int v2 = meshTriangles[t + 2];

                // Get chunk index of triangle
                int chunkIndex = GetOrPrepareProtoChunkIndexByLooseParts(v0, v1, v2, chunksTriangles.Count, vertex2chunk);

                // Triangles
                if (chunksTriangles.Count <= chunkIndex)
                {
                    chunksTriangles.Add(new List<int>());
                }
                chunksTriangles[chunkIndex].Add(v0);
                chunksTriangles[chunkIndex].Add(v1);
                chunksTriangles[chunkIndex].Add(v2);

                // Data structure for performance reasons
                if (chunksVertices.Count <= chunkIndex)
                {
                    chunksVertices.Add(new HashSet<int>());
                }
                chunksVertices[chunkIndex].Add(v0);
                chunksVertices[chunkIndex].Add(v1);
                chunksVertices[chunkIndex].Add(v2);
            }

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Merging proto chunks (loose parts method):", 0.4f);

            // Merge chunks
            for (int sourceChunkIndex = chunksTriangles.Count - 1; sourceChunkIndex > 0; --sourceChunkIndex)
            {
                for (int targetChunkIndex = sourceChunkIndex - 1; targetChunkIndex >= 0; --targetChunkIndex)
                {
                    bool sourceChunkWasMerged = false;

                    // ... check every vertex index ...
                    for (int ts = 0; ts < chunksTriangles[sourceChunkIndex].Count; ++ts)
                    {
                        // ... if it is contained in the following chunk as well and if true: merge both chunks
                        int sourceChunkVertexIndex = chunksTriangles[sourceChunkIndex][ts];
                        if (chunksVertices[targetChunkIndex].Contains(sourceChunkVertexIndex))
                        {
                            chunksTriangles[targetChunkIndex].AddRange(chunksTriangles[sourceChunkIndex]);
                            chunksTriangles.RemoveAt(sourceChunkIndex);
                            sourceChunkWasMerged = true;
                            break;
                        }
                    }

                    if (sourceChunkWasMerged) { break; }
                }
            }

            return chunksTriangles;
        }

        private static int GetOrPrepareProtoChunkIndexByLooseParts(int vertexIndex0, int vertexIndex1, int vertexIndex2, int chunksCount, Dictionary<int, int> vertex2chunk)
        {
            int chunkIndex0 = vertex2chunk.TryGetValue(vertexIndex0, out chunkIndex0) ? chunkIndex0 : -1;
            int chunkIndex1 = vertex2chunk.TryGetValue(vertexIndex1, out chunkIndex1) ? chunkIndex1 : -1;
            int chunkIndex2 = vertex2chunk.TryGetValue(vertexIndex2, out chunkIndex2) ? chunkIndex2 : -1;

            int bestChunkIndex = GetMajorityIndex(chunkIndex0, chunkIndex1, chunkIndex2);
            if (bestChunkIndex < 0)
            {
                bestChunkIndex = chunksCount;
            }

            if (chunkIndex0 < 0)
            {
                vertex2chunk.Add(vertexIndex0, bestChunkIndex);

            }
            if (chunkIndex1 < 0)
            {
                vertex2chunk.Add(vertexIndex1, bestChunkIndex);

            }
            if (chunkIndex2 < 0)
            {
                vertex2chunk.Add(vertexIndex2, bestChunkIndex);

            }

            return bestChunkIndex;
        }


        /* Chunks by bones ######################################################################################### */

        private static List<SkinnedMeshChunk> GenerateMeshChunksByBonesCount(SkinnedMeshChunk sourceChunk, int bonesPerMeshBudget)
        {
            List<List<int>> chunksTrianglesRaw = ChunkTrianglesByBonesCount(sourceChunk.Mesh, bonesPerMeshBudget);

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Separate chunks (max bones method):", 0.5f);

            List<List<int>> chunksTriangles = SeparateChunks(sourceChunk.Mesh, chunksTrianglesRaw, out Dictionary<int, List<KeyValuePair<int, int>>> toBeClonedVerticesMapping);

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Write chunks (max bones method):", 0.6f);

            List<SkinnedMeshChunk> meshChunks = new List<SkinnedMeshChunk>(chunksTriangles.Count);
            for (int chunkIndex = 0; chunkIndex < chunksTriangles.Count; ++chunkIndex)
            {
                List<int> chunkTriangles = chunksTriangles[chunkIndex];
                GetMappingsBetweenNewAndOldVertices(chunkTriangles, out List<int> new2oldVertexMap, out Dictionary<int, int> old2newVertexMap);

                // account for the cloned vertices
                List<int> new2oldVertexMapRemapped = new List<int>(new2oldVertexMap);
                if (toBeClonedVerticesMapping.ContainsKey(chunkIndex))
                {
                    for (int i = 0; i < new2oldVertexMap.Count; ++i)
                    {
                        new2oldVertexMapRemapped[i] = MapIndex(new2oldVertexMap[i], toBeClonedVerticesMapping[chunkIndex], true);
                    }
                }

                Mesh chunkMesh = CloneMeshPart(sourceChunk.Mesh, chunkTriangles, new2oldVertexMapRemapped, old2newVertexMap);

                SkinnedMeshChunk skinnedMeshChunk = new SkinnedMeshChunk(sourceChunk.skinnedMeshRenderer, chunkMesh, chunkIndex, bonesPerMeshBudget);
                meshChunks.Add(skinnedMeshChunk);
            }

            return meshChunks;
        }

        private static List<List<int>> ChunkTrianglesByBonesCount(Mesh mesh, int bonesPerMeshBudget)
        {
            // Vertices per chunk
            List<List<int>> chunksTriangles = new List<List<int>>();

            // Bones per chunk
            List<HashSet<int>> chunksBones = new List<HashSet<int>>();

            BoneWeight[] weights = new BoneWeight[3];
            HashSet<int> bones = new HashSet<int>();

            // Never ever access mesh arrays inside a loop as it will currently create throw-away copies each time
            int[] meshTriangles = mesh.triangles;
            BoneWeight[] meshBoneWeights = mesh.boneWeights;

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Building proto chunks (max bones method): ", 0.2f);

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                int vertexIndex0 = meshTriangles[i + 0];
                int vertexIndex1 = meshTriangles[i + 1];
                int vertexIndex2 = meshTriangles[i + 2];

                // Get chunk index of triangle
                weights[0] = meshBoneWeights[vertexIndex0];
                weights[1] = meshBoneWeights[vertexIndex1];
                weights[2] = meshBoneWeights[vertexIndex2];

                int chunkIndex = GetOrPrepareProtoChunkIndexByBonesCount(weights, chunksTriangles, chunksBones);

                // Add triangle
                chunksTriangles[chunkIndex].Add(vertexIndex0);
                chunksTriangles[chunkIndex].Add(vertexIndex1);
                chunksTriangles[chunkIndex].Add(vertexIndex2);

                // Add bones
                bones = GetWeightedBones(meshBoneWeights[vertexIndex0]);
                bones.UnionWith(GetWeightedBones(meshBoneWeights[vertexIndex1]));
                bones.UnionWith(GetWeightedBones(meshBoneWeights[vertexIndex2]));
                chunksBones[chunkIndex].UnionWith(bones);
            }

            EditorUtility.DisplayProgressBar("Pre-process Mesh", "Merging proto chunks (max bones method):", 0.4f);

            // First run merges triangles with shared bones, 2nd merges any triangles up to limit
            for (int run = 1; run <= 2; ++run)
            {
                for (int sourceChunkIndex = chunksTriangles.Count - 1; sourceChunkIndex > 0; --sourceChunkIndex)
                {
                    for (int targetChunkIndex = sourceChunkIndex - 1; targetChunkIndex >= 0; --targetChunkIndex)
                    {
                        int sharedBones = chunksBones[sourceChunkIndex].IntersectCount(chunksBones[targetChunkIndex]);

                        // for the first run do not merge chunks without shared bones to reduce cluttering
                        if (run <= 1 && sharedBones <= 0) { continue; }

                        // skip chunks which are too big to be merged
                        int uniqueTargetBones = chunksBones[targetChunkIndex].Count - sharedBones;
                        if (chunksBones[sourceChunkIndex].Count + uniqueTargetBones > bonesPerMeshBudget) { continue; }

                        chunksTriangles[targetChunkIndex].AddRange(chunksTriangles[sourceChunkIndex]);
                        chunksBones[targetChunkIndex].UnionWith(chunksBones[sourceChunkIndex]);

                        chunksTriangles.RemoveAt(sourceChunkIndex);
                        chunksBones.RemoveAt(sourceChunkIndex);
                        break;
                    }
                }
                ++run;
            }

            return chunksTriangles;
        }

        private static int GetOrPrepareProtoChunkIndexByBonesCount(BoneWeight[] boneWeights, List<List<int>> vertexChunks, List<HashSet<int>> vertexChunksBones)
        {
            // Check if all vertices have weights
            if (!CheckIfAllWeightsAreValid(boneWeights)) { return -1; }

            // Chunk is already known
            for (int chunkIndex = 0; chunkIndex < vertexChunksBones.Count; ++chunkIndex)
            {
                if (CheckIfSetContainsAllWeightedBones(boneWeights, vertexChunksBones[chunkIndex]))
                {
                    return chunkIndex;
                }
            }

            // Create new chunk
            vertexChunksBones.Add(new HashSet<int>());
            vertexChunks.Add(new List<int>());

            return vertexChunksBones.Count - 1;
        }

        private static List<List<int>> SeparateChunks(Mesh mesh, List<List<int>> chunksTrianglesToSplit, out Dictionary<int, List<KeyValuePair<int, int>>> toBeClonedVerticesMapping)
        {
            // Number of triangles stays the same, we only create new vertices.
            List<List<int>> chunksTriangles = new List<List<int>>(chunksTrianglesToSplit.Count);
            Dictionary<int, int> vertex2chunk = new Dictionary<int, int>(mesh.vertexCount);
            for (int chunkIndex = 0; chunkIndex < chunksTrianglesToSplit.Count; ++chunkIndex)
            {
                chunksTriangles.Add(new List<int>());

                foreach (int vertexIndex in chunksTrianglesToSplit[chunkIndex])
                {
                    if (!vertex2chunk.ContainsKey(vertexIndex))
                    {
                        vertex2chunk.Add(vertexIndex, chunkIndex);
                    }
                }
            }

            toBeClonedVerticesMapping = new Dictionary<int, List<KeyValuePair<int, int>>>();
            int clonedVerticesCount = 0;

            // Never ever access mesh arrays inside a loop as it will currently create throw-away copies each time
            int[] meshTriangles = mesh.triangles;

            // create new vertices for triangles bordering two chunks
            for (int t = 0; t < meshTriangles.Length; t += 3)
            {
                // Process vertex 1
                int vertexIndex = meshTriangles[t];

                // the first vertex defines the chunk of the whole triangle
                int chunkIndex = vertex2chunk.TryGetValue(vertexIndex, out chunkIndex) ? chunkIndex : -1;

                chunksTriangles[chunkIndex].Add(vertexIndex);

                // Process vertex 2 and 3
                for (int v = 1; v < 3; ++v)
                {
                    // get vertex and chunk index (use the cloned one where applicable)
                    vertexIndex = MapIndex(meshTriangles[t + v], chunkIndex, toBeClonedVerticesMapping);
                    int otherChunkIndex = vertex2chunk.TryGetValue(vertexIndex, out otherChunkIndex) ? otherChunkIndex : -1;

                    // check if this vertex is from another chunk
                    bool vertexIsFromOtherChunk = chunkIndex != otherChunkIndex;
                    if (vertexIsFromOtherChunk)
                    {
                        // create a clone for our chunk
                        int clonedVertexIndex = mesh.vertexCount + clonedVerticesCount;
                        if (!toBeClonedVerticesMapping.ContainsKey(chunkIndex))
                        {
                            toBeClonedVerticesMapping.Add(chunkIndex, new List<KeyValuePair<int, int>>());
                        }
                        toBeClonedVerticesMapping[chunkIndex].Add(new KeyValuePair<int, int>(vertexIndex, clonedVertexIndex));
                        chunksTrianglesToSplit[chunkIndex].AddUnique(clonedVertexIndex);
                        vertex2chunk.Add(clonedVertexIndex, chunkIndex);
                        vertexIndex = clonedVertexIndex;
                        ++clonedVerticesCount;
                    }

                    // add vertex or replace old vertex index with new one if the vertex was cloned
                    chunksTriangles[chunkIndex].Add(vertexIndex);
                }
            }

            return chunksTriangles;
        }


        /* Index utilities ######################################################################################### */

        // Returns the first index or the index which is used most often.
        private static int GetMajorityIndex(int i0, int i1, int i2)
        {
            int bestI = i0;
            if (i1 >= 0)
            {
                bestI = bestI >= 0 && i1 == i2 ? i1 : bestI;
            }
            bestI = bestI <= 0 ? i2 : bestI;

            return bestI;
        }

        // Map index to new one. Either from key > value or value > key (inverse). Mapping index decides which mapping to use.
        private static int MapIndex(int index, int mappingIndex, Dictionary<int, List<KeyValuePair<int, int>>> mappings, bool inverse = false)
        {
            if (!mappings.ContainsKey(mappingIndex)) { return index; }
            return MapIndex(index, mappings[mappingIndex], inverse);
        }

        // Map index to new one. Either from key > value or value > key (inverse).
        private static int MapIndex(int index, List<KeyValuePair<int, int>> mapping, bool inverse = false)
        {
            foreach (KeyValuePair<int, int> vertexMap in mapping)
            {
                if (inverse)
                {
                    if (vertexMap.Value == index)
                    {
                        return vertexMap.Key;
                    }
                }
                else
                {
                    if (vertexMap.Key == index)
                    {
                        return vertexMap.Value;
                    }
                }
            }
            return index;
        }


        /* Mesh utilities ######################################################################################### */

        // Create a new mesh from the supplied triangles.
        private static Mesh CloneMeshPart(Mesh oldMesh, List<int> oldTriangles, List<int> new2oldVertexMap, Dictionary<int, int> old2newVertexMap)
        {
            Mesh newMesh = new Mesh();

            // Vertex data
            if (new2oldVertexMap.Count > 0)
            {
                newMesh.vertices = oldMesh.vertices.CloneSubset(new2oldVertexMap);
                newMesh.boneWeights = oldMesh.boneWeights.CloneSubset(new2oldVertexMap);
                newMesh.colors = oldMesh.colors.CloneSubset(new2oldVertexMap);
                newMesh.normals = oldMesh.normals.CloneSubset(new2oldVertexMap);
                newMesh.tangents = oldMesh.tangents.CloneSubset(new2oldVertexMap);
                newMesh.uv = oldMesh.uv.CloneSubset(new2oldVertexMap);
                newMesh.uv2 = oldMesh.uv2.CloneSubset(new2oldVertexMap);
                newMesh.uv3 = oldMesh.uv3.CloneSubset(new2oldVertexMap);
                newMesh.uv4 = oldMesh.uv4.CloneSubset(new2oldVertexMap);
            }
            else
            {
                newMesh.vertices = (Vector3[])oldMesh.vertices.Clone();
                newMesh.boneWeights = (BoneWeight[])oldMesh.boneWeights.Clone();
                newMesh.colors = (Color[])oldMesh.colors.Clone();
                newMesh.normals = (Vector3[])oldMesh.normals.Clone();
                newMesh.tangents = (Vector4[])oldMesh.tangents.Clone();
                newMesh.uv = (Vector2[])oldMesh.uv.Clone();
                newMesh.uv2 = (Vector2[])oldMesh.uv2.Clone();
                newMesh.uv3 = (Vector2[])oldMesh.uv3.Clone();
                newMesh.uv4 = (Vector2[])oldMesh.uv4.Clone();
            }

            newMesh.bindposes = (Matrix4x4[])oldMesh.bindposes.Clone();

            // Triangle data
            if (old2newVertexMap.Count > 0)
            {
                newMesh.triangles = MapTrianglesToNewIndices(oldTriangles, old2newVertexMap);
            }
            else
            {
                newMesh.triangles = (int[])oldMesh.triangles.Clone();
            }

            newMesh.RecalculateBounds();

            return newMesh;
        }

        // Map the triangle vertex indices to the supplied new ones.
        private static int[] MapTrianglesToNewIndices(List<int> oldTriangles, Dictionary<int, int> old2newVertexMap)
        {
            int[] newTriangles = new int[oldTriangles.Count];
            for (int t = 0; t < oldTriangles.Count; ++t)
            {
                int oldVertexIndex = oldTriangles[t];
                newTriangles[t] = old2newVertexMap[oldVertexIndex];
            }
            return newTriangles;
        }

        // Create mappings for a mesh which only consists out of the provided triangles.
        private static void GetMappingsBetweenNewAndOldVertices(List<int> oldTriangles, out List<int> new2oldVertexMap, out Dictionary<int, int> old2newVertexMap)
        {
            new2oldVertexMap = new List<int>();
            old2newVertexMap = new Dictionary<int, int>();
            HashSet<int> new2oldVertexIndicesHashSet = new HashSet<int>();
            for (int t = 0; t < oldTriangles.Count; ++t)
            {
                int oldVertexIndex = oldTriangles[t];
                if (new2oldVertexIndicesHashSet.Contains(oldVertexIndex)) { continue; }
                new2oldVertexIndicesHashSet.Add(oldVertexIndex);
                old2newVertexMap.Add(oldVertexIndex, new2oldVertexMap.Count);
                new2oldVertexMap.Add(oldVertexIndex);
            }
        }


        /* Bone utilities ######################################################################################### */

        // Remove all bones from the skinnedMeshRenderer which are not weighted to the mesh. 
        public static void RemoveUnusedBones(SkinnedMeshChunk meshChunk)
        {
            Mesh workingMesh = meshChunk.Mesh;

            // Find used bone indices --------------------------
            HashSet<int> usedBoneIndices = new HashSet<int>();

            BoneWeight[] boneWeights = workingMesh.boneWeights;

            // for every vertex...
            for (int i = 0; workingMesh.vertexCount > i; ++i)
            {
                usedBoneIndices.Add(boneWeights[i].boneIndex0);
                usedBoneIndices.Add(boneWeights[i].boneIndex1);
                usedBoneIndices.Add(boneWeights[i].boneIndex2);
                usedBoneIndices.Add(boneWeights[i].boneIndex3);
            }

            if (usedBoneIndices.Count <= 0) { return; }

            // Find used bones --------------------------
            Transform[] usedBones = new Transform[usedBoneIndices.Count];
            int newBoneIdx = 0;
            for (int i = 0; meshChunk.BonesCount > i; ++i)
            {
                Transform bone = meshChunk.skinnedMeshRenderer.bones[i];
                if (!usedBoneIndices.Contains(i)) { continue; }

                usedBones[newBoneIdx] = bone;
                ++newBoneIdx;
            }

            // Remap bone weights to used bones --------------------------
            Transform[] allBones = meshChunk.skinnedMeshRenderer.bones;

            // for every vertex...
            for (int i = 0; workingMesh.vertexCount > i; ++i)
            {
                boneWeights[i].boneIndex0 = TryConvertSourceToTargetBoneIndex(boneWeights[i].boneIndex0, allBones, usedBones);
                boneWeights[i].boneIndex1 = TryConvertSourceToTargetBoneIndex(boneWeights[i].boneIndex1, allBones, usedBones);
                boneWeights[i].boneIndex2 = TryConvertSourceToTargetBoneIndex(boneWeights[i].boneIndex2, allBones, usedBones);
                boneWeights[i].boneIndex3 = TryConvertSourceToTargetBoneIndex(boneWeights[i].boneIndex3, allBones, usedBones);
            }

            // update skinnedMeshRenderer --------------------------
            meshChunk.skinnedMeshRenderer.bones = usedBones;

            // update mesh --------------------------
            workingMesh.boneWeights = boneWeights;
            Matrix4x4[] bindPoses = new Matrix4x4[meshChunk.BonesCount];
            for (int b = 0; b < meshChunk.BonesCount; ++b)
            {
                bindPoses[b] = meshChunk.skinnedMeshRenderer.bones[b].worldToLocalMatrix * meshChunk.gameObject.transform.localToWorldMatrix;
            }
            workingMesh.bindposes = bindPoses;
        }

        // Returns the target array index where the specified source bone can be found. Otherwise -1.
        private static int TryConvertSourceToTargetBoneIndex(int sourceBoneIdx, Transform[] sourceBones, Transform[] targetBones)
        {
            for (int i = 0; targetBones.Length > i; ++i)
            {
                if (targetBones[i].name == sourceBones[sourceBoneIdx].name)
                {
                    return i;
                }
            }
            return -1;
        }

        // Returns true if every bone weight has at least one weighted bone.
        private static bool CheckIfAllWeightsAreValid(BoneWeight[] boneWeights)
        {
            for (int i = 0; i < boneWeights.Length; ++i)
            {
                BoneWeight boneWeight = boneWeights[i];

                if (boneWeight.weight0 <= 0 &&
                    boneWeight.weight1 <= 0 &&
                    boneWeight.weight2 <= 0 &&
                    boneWeight.weight3 <= 0) { return false; }
            }
            return true;
        }

        // Check if weighted bones are all part of the provided set.
        private static bool CheckIfSetContainsAllWeightedBones(BoneWeight[] boneWeights, HashSet<int> boneIndices)
        {
            for (int i = 0; i < boneWeights.Length; ++i)
            {
                BoneWeight boneWeight = boneWeights[i];

                if ((boneWeight.weight0 > 0 && !boneIndices.Contains(boneWeight.boneIndex0)) ||
                    (boneWeight.weight1 > 0 && !boneIndices.Contains(boneWeight.boneIndex1)) ||
                    (boneWeight.weight2 > 0 && !boneIndices.Contains(boneWeight.boneIndex2)) ||
                    (boneWeight.weight3 > 0 && !boneIndices.Contains(boneWeight.boneIndex3)))
                {
                    return false;
                }
            }
            return true;
        }

        // Return the bone indices which have some weights.
        private static HashSet<int> GetWeightedBones(BoneWeight boneWeight)
        {
            HashSet<int> boneIndices = new HashSet<int>();
            if (boneWeight.weight0 > 0) { boneIndices.Add(boneWeight.boneIndex0); }
            if (boneWeight.weight1 > 0) { boneIndices.Add(boneWeight.boneIndex1); }
            if (boneWeight.weight2 > 0) { boneIndices.Add(boneWeight.boneIndex2); }
            if (boneWeight.weight3 > 0) { boneIndices.Add(boneWeight.boneIndex3); }
            return boneIndices;
        }
    }
}