using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System;
using System.Collections.Generic;
using FmdlStudio.Scripts.MonoBehaviours;
using FmdlStudio.Scripts.Static;

namespace FmdlStudio.Scripts.Classes
{
    [ScriptedImporter(1, "fmdl")]
    public class FmdlImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            try
            {
                ReadWithAssetImportContext(ctx, ctx.assetPath);
            } //try
            catch (Exception e)
            {
                Debug.Log($"{e.Message}");
                Debug.Log($"An exception occured{e.StackTrace}");
            } //catch
        } //OnImportAsset

        public void ReadWithoutAssetImportContext(string filePath)
        {
            Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(filePath));
            fmdl.Read(filePath);

            int boneCount = fmdl.fmdlBones != null ? fmdl.fmdlBones.Length : 0;
            int materialCount = fmdl.fmdlMaterialInstances.Length;
            int textureCount = fmdl.fmdlTextures != null ? fmdl.fmdlTextures.Length : 0;
            int meshCount = fmdl.fmdlMeshInfos.Length;
            int meshGroupCount = fmdl.fmdlMeshGroups.Length;

            GameObject mainObject = new GameObject(fmdl.name);
            FoxModel foxModel = mainObject.AddComponent<FoxModel>();
            foxModel.meshGroups = new FoxMeshGroup[meshGroupCount];
            foxModel.meshDefinitions = new FoxMeshDefinition[meshCount];
            Transform[] bones = new Transform[boneCount];
            Texture[] textures = new Texture[textureCount];
            Material[] materials = new Material[materialCount];
            Mesh[] meshes = new Mesh[meshCount];

            Transform rootBone = new GameObject().transform;
            rootBone.name = "[Root]";
            rootBone.parent = mainObject.transform;

            Read(fmdl, mainObject, foxModel, bones, textures, materials, meshes, rootBone);
        } //ReadWithoutAssetImportContext

        private Texture2D LoadTextureDXT(string path)
        {
            byte[] ddsBytes = File.ReadAllBytes(path);
            TextureFormat textureFormat;

            if (ddsBytes[87] == 0x31)
                textureFormat = TextureFormat.DXT1;
            else if (ddsBytes[87] == 0x35)
                textureFormat = TextureFormat.DXT5;
            else
                throw new Exception("Invalid TextureFormat. Only DXT1 and DXT5 formats are supported by this method.");

            byte ddsSizeCheck = ddsBytes[4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            int height = ddsBytes[13] * 256 + ddsBytes[12];
            int width = ddsBytes[17] * 256 + ddsBytes[16];

            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[ddsBytes.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(ddsBytes, DDS_HEADER_SIZE, dxtBytes, 0, ddsBytes.Length - DDS_HEADER_SIZE);

            Texture2D texture = new Texture2D(width, height, textureFormat, false);
            texture.LoadRawTextureData(dxtBytes);
            texture.Apply();

            return (texture);
        } //LoadTextureDXT

        private void ReadWithAssetImportContext(AssetImportContext ctx, string filePath)
        {
            Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(filePath));
            fmdl.Read(filePath);

            int boneCount = fmdl.fmdlBones != null ? fmdl.fmdlBones.Length : 0;
            int materialCount = fmdl.fmdlMaterialInstances.Length;
            int textureCount = fmdl.fmdlTextures != null ? fmdl.fmdlTextures.Length : 0;
            int meshCount = fmdl.fmdlMeshInfos.Length;
            int meshGroupCount = fmdl.fmdlMeshGroups.Length;

            GameObject mainObject = new GameObject(fmdl.name);
            FoxModel foxModel = mainObject.AddComponent<FoxModel>();
            foxModel.meshGroups = new FoxMeshGroup[meshGroupCount];
            foxModel.meshDefinitions = new FoxMeshDefinition[meshCount];
            Transform[] bones = new Transform[boneCount];
            Texture[] textures = new Texture[textureCount];
            Material[] materials = new Material[materialCount];
            Mesh[] meshes = new Mesh[meshCount];

            Transform rootBone = new GameObject().transform;
            rootBone.name = "[Root]";
            rootBone.parent = mainObject.transform;

            Read(fmdl, mainObject, foxModel, bones, textures, materials, meshes, rootBone);

            ctx.AddObjectToAsset("mainObject", mainObject);
            ctx.SetMainObject(mainObject);

            for (int i = 0; i < textureCount; i++)
                ctx.AddObjectToAsset($"Texture {i}", textures[i]);

            for (int i = 0; i < materialCount; i++)
                ctx.AddObjectToAsset($"Material {i}", materials[i]);

            for (int i = 0; i < meshCount; i++)
                ctx.AddObjectToAsset($"Mesh {i}", meshes[i]);
        } //ReadWithAssetImportContext

        private void Read(Fmdl fmdl, GameObject mainObject, FoxModel foxModel, Transform[] bones, Texture[] textures, Material[] materials, Mesh[] meshes, Transform rootBone)
        {
            bool isGZFormat = fmdl.version == 2.03f;
            int boneCount = fmdl.fmdlBones != null ? fmdl.fmdlBones.Length : 0;
            int materialCount = fmdl.fmdlMaterialInstances.Length;
            int textureCount = fmdl.fmdlTextures != null ? fmdl.fmdlTextures.Length : 0;
            int meshCount = fmdl.fmdlMeshInfos.Length;
            int meshGroupCount = fmdl.fmdlMeshGroups.Length;

            {
                Fmdl.FmdlBoundingBox fmdlBoundingBox = fmdl.fmdlBoundingBoxes[0];

                Bounds bounds = new Bounds();
                bounds.SetMinMax(new Vector3(-fmdlBoundingBox.min.x, fmdlBoundingBox.min.y, fmdlBoundingBox.min.z), new Vector3(-fmdlBoundingBox.max.x, fmdlBoundingBox.max.y, fmdlBoundingBox.max.z));
                BoxCollider boxCollider = rootBone.gameObject.AddComponent<BoxCollider>();
                boxCollider.center = rootBone.InverseTransformPoint(bounds.center);
                boxCollider.size = bounds.size;
            } //code block

            for (int i = 0; i < boneCount; i++)
            {
                bones[i] = new GameObject().transform;

                Fmdl.FmdlBone fmdlBone = fmdl.fmdlBones[i];
                Fmdl.FmdlBoundingBox fmdlBoundingBox = fmdl.fmdlBoundingBoxes[fmdlBone.boundingBoxIndex];

                //Get the name.
                if (isGZFormat)
                    bones[i].gameObject.name = fmdl.fmdlStrings[fmdlBone.nameIndex];
                else
                    bones[i].gameObject.name = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlBone.nameIndex]);

                try
                {
                    UInt64.Parse(bones[i].gameObject.name, System.Globalization.NumberStyles.HexNumber);
                    bones[i].name = $"SKL_{Int32.Parse(bones[i - 1].name.Substring(4, 3)) + 1}_UNKNOWN ({bones[i].name})";
                } //try
                catch { }

                //Assign the parent.
                if (fmdlBone.parentIndex != -1)
                    bones[i].parent = bones[fmdlBone.parentIndex];
                else
                    bones[i].parent = rootBone.transform;

                bones[i].position = new Vector3(-fmdlBone.worldPosition.x, fmdlBone.worldPosition.y, fmdlBone.worldPosition.z);

                //Set up the bounding box.
                Bounds bounds = new Bounds();

                //The x min and max are swapped here on purpose! The max value will end up as the smallest value and the min value will end up as the largest value if they aren't swapped here!
                bounds.SetMinMax(new Vector3(-fmdlBoundingBox.max.x, fmdlBoundingBox.min.y, fmdlBoundingBox.min.z), new Vector3(-fmdlBoundingBox.min.x, fmdlBoundingBox.max.y, fmdlBoundingBox.max.z));
                BoxCollider boxCollider = bones[i].gameObject.AddComponent<BoxCollider>();
                boxCollider.center = bones[i].InverseTransformPoint(bounds.center);
                boxCollider.size = bounds.size;
            } //for

            for (int i = 0; i < textureCount; i++)
            {
                string name;

                Fmdl.FmdlTexture fmdlTexture = fmdl.fmdlTextures[i];

                //Get the name.
                if (isGZFormat)
                {
                    name = fmdl.fmdlStrings[fmdlTexture.pathIndex] + fmdl.fmdlStrings[fmdlTexture.nameIndex];
                    name = name.Substring(0, name.IndexOf(".")) + ".dds";
                } //if
                else
                    name = Hashing.TryGetPathName(fmdl.fmdlPathCode64s[fmdlTexture.pathIndex]) + ".dds";

                //Read the file.
                if (File.Exists($"{Globals.texturePath}\\{name}"))
                    textures[i] = LoadTextureDXT($"{Globals.texturePath}\\{name}");
                else
                {
                    Debug.Log($"Could not find {Globals.texturePath}\\{name}");

                    Texture2D texture = new Texture2D(512, 512);

                    for (int j = 0; j < 512; j++)
                        for (int h = 0; h < 512; h++)
                        {
                            Color c = new Color(0.25f, 0.25f, 0.25f);

                            if (((j / 32) % 2 == 0 && (h / 32) % 2 != 0) || ((j / 32) % 2 != 0 && (h / 32) % 2 == 0))
                                c = new Color(0.5f, 0.5f, 0.5f);

                            texture.SetPixel(j, h, c);
                        } //for

                    texture.Apply();

                    textures[i] = texture;
                } //else

                //ctx.AddObjectToAsset($"Texture {i}", textures[i]);
                textures[i].name = name;
            } //for

            for (int i = 0; i < materialCount; i++)
            {
                string shaderName;
                string materialName;

                Fmdl.FmdlMaterialInstance fmdlMaterialInstance = fmdl.fmdlMaterialInstances[i];

                //Get the shader and material name.
                if (isGZFormat)
                {
                    shaderName = fmdl.fmdlStrings[fmdl.fmdlMaterials[fmdlMaterialInstance.materialIndex].typeIndex];
                    materialName = fmdl.fmdlStrings[fmdlMaterialInstance.nameIndex];
                } //if
                else
                {
                    shaderName = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdl.fmdlMaterials[fmdlMaterialInstance.materialIndex].typeIndex]);
                    materialName = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlMaterialInstance.nameIndex]);
                } //else

                materials[i] = new Material(Shader.Find($"FoxShaders/{shaderName}"));
                //ctx.AddObjectToAsset($"Material {i}", materials[i]);
                materials[i].name = materialName;

                //Link textures.
                for (int j = fmdlMaterialInstance.firstTextureIndex; j < fmdlMaterialInstance.firstTextureIndex + fmdlMaterialInstance.textureCount; j++)
                {
                    string textureType;

                    Fmdl.FmdlMaterialParameter fmdlMaterialParameter = fmdl.fmdlMaterialParameters[j];

                    if (isGZFormat)
                        textureType = fmdl.fmdlStrings[fmdlMaterialParameter.nameIndex];
                    else
                        textureType = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlMaterialParameter.nameIndex]);

                    materials[i].SetTexture(textureType, textures[fmdlMaterialParameter.referenceIndex]);
                    materials[i].SetTextureScale(textureType, new Vector2(1, -1)); //Have to flip textures here because Texture2D.LoadRawData is bugged an imports DDS files upside down.
                } //for

                //Set parameters.
                for (int j = fmdlMaterialInstance.firstParameterIndex; j < fmdlMaterialInstance.firstParameterIndex + fmdlMaterialInstance.parameterCount; j++)
                {
                    string parameterName;

                    Fmdl.FmdlMaterialParameter fmdlMaterialParameter = fmdl.fmdlMaterialParameters[j];

                    if (isGZFormat)
                        parameterName = fmdl.fmdlStrings[fmdlMaterialParameter.nameIndex];
                    else
                        parameterName = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlMaterialParameter.nameIndex]);

                    materials[i].SetVector(parameterName, fmdl.fmdlMaterialParameterVectors[fmdlMaterialParameter.referenceIndex]);
                } //for
            } //for

            for (int i = 0; i < meshGroupCount; i++)
            {
                foxModel.meshGroups[i] = new FoxMeshGroup();
                string name;

                Fmdl.FmdlMeshGroup fmdlMeshGroup = fmdl.fmdlMeshGroups[i];

                if (isGZFormat)
                    name = fmdl.fmdlStrings[fmdlMeshGroup.nameIndex];
                else
                    name = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlMeshGroup.nameIndex]);

                foxModel.meshGroups[i].name = name;
                foxModel.meshGroups[i].parent = fmdlMeshGroup.parentIndex;

                if (fmdlMeshGroup.invisibilityFlag == 0)
                    foxModel.meshGroups[i].visible = true;
                else
                    foxModel.meshGroups[i].visible = false;
            } //for

            for (int i = 0; i < meshCount; i++)
            {
                meshes[i] = new Mesh();
                //ctx.AddObjectToAsset($"Mesh {i}", meshes[i]);
                GameObject gameObject = new GameObject();
                gameObject.transform.parent = mainObject.transform;
                SkinnedMeshRenderer skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
                foxModel.meshDefinitions[i] = new FoxMeshDefinition();

                Fmdl.FmdlMesh fmdlMesh = fmdl.fmdlMeshes[i];
                Fmdl.FmdlMeshInfo fmdlMeshInfo = fmdl.fmdlMeshInfos[i];
                Fmdl.FmdlBoneGroup fmdlBoneGroup = fmdl.fmdlBoneGroups != null ? fmdl.fmdlBoneGroups[fmdlMeshInfo.boneGroupIndex] : new Fmdl.FmdlBoneGroup();

                int vertexLength = fmdlMesh.vertices.Length;
                int faceLength = fmdlMesh.triangles.Length;
                string name;

                Vector3[] vertices = new Vector3[vertexLength];
                Vector3[] normals = new Vector3[vertexLength];
                Vector4[] tangents = fmdlMesh.tangents != null ? new Vector4[vertexLength] : new Vector4[0];
                Color[] colors = fmdlMesh.colors != null ? new Color[vertexLength] : new Color[0];
                BoneWeight[] boneWeights = fmdlMesh.boneWeights != null ? new BoneWeight[vertexLength] : new BoneWeight[0];
                Vector2[] uv = fmdlMesh.uv != null ? new Vector2[vertexLength] : new Vector2[0];
                Vector2[] uv2 = fmdlMesh.uv2 != null ? new Vector2[vertexLength] : new Vector2[0];
                Vector2[] uv3 = fmdlMesh.uv3 != null ? new Vector2[vertexLength] : new Vector2[0];
                Vector2[] uv4 = fmdlMesh.uv4 != null ? new Vector2[vertexLength] : new Vector2[0];
                int[] triangles = new int[faceLength];
                Matrix4x4[] bindPoses;

                List<Transform> usedBones = new List<Transform>(0);
                Transform[] usedBonesArray;

                for (int j = 0; j < vertexLength; j++)
                {
                    vertices[j] = new Vector3(-fmdlMesh.vertices[j].x, fmdlMesh.vertices[j].y, fmdlMesh.vertices[j].z);
                    normals[j] = new Vector3(-fmdlMesh.normals[j].x, fmdlMesh.normals[j].y, fmdlMesh.normals[j].z);
                    if (tangents.Length > 0)
                        tangents[j] = new Vector4(-fmdlMesh.tangents[j].x, fmdlMesh.tangents[j].y, fmdlMesh.tangents[j].z, fmdlMesh.tangents[j].w);
                    if (colors.Length > 0)
                        colors[j] = new Color(fmdlMesh.colors[j].x / 255f, fmdlMesh.colors[j].y / 255f, fmdlMesh.colors[j].z / 255f, fmdlMesh.colors[j].w / 255f);
                    if (boneWeights.Length > 0)
                    {
                        boneWeights[j].weight0 = fmdlMesh.boneWeights[j].x / 255f;
                        boneWeights[j].weight1 = fmdlMesh.boneWeights[j].y / 255f;
                        boneWeights[j].weight2 = fmdlMesh.boneWeights[j].z / 255f;
                        boneWeights[j].weight3 = fmdlMesh.boneWeights[j].w / 255f;
                        boneWeights[j].boneIndex0 = fmdlBoneGroup.boneIndices[(int)fmdlMesh.boneIndices[j].x];
                        boneWeights[j].boneIndex1 = fmdlBoneGroup.boneIndices[(int)fmdlMesh.boneIndices[j].y];
                        boneWeights[j].boneIndex2 = fmdlBoneGroup.boneIndices[(int)fmdlMesh.boneIndices[j].z];
                        boneWeights[j].boneIndex3 = fmdlBoneGroup.boneIndices[(int)fmdlMesh.boneIndices[j].w];
                    } //if
                    if (uv.Length > 0)
                        uv[j] = new Vector2(fmdlMesh.uv[j].x, 1 - fmdlMesh.uv[j].y);
                    if (uv2.Length > 0)
                        uv2[j] = new Vector2(fmdlMesh.uv2[j].x, 1 - fmdlMesh.uv2[j].y);
                    if (uv3.Length > 0)
                        uv3[j] = new Vector2(fmdlMesh.uv3[j].x, 1 - fmdlMesh.uv3[j].y);
                    if (uv4.Length > 0)
                        uv4[j] = new Vector2(fmdlMesh.uv4[j].x, 1 - fmdlMesh.uv4[j].y);
                } //for

                for (int j = 0; j < faceLength; j++)
                    triangles[j] = fmdlMesh.triangles[j];

                for (int j = 0; j < boneWeights.Length; j++)
                {
                    if (boneWeights[j].weight0 > 0f)
                    {
                        if (usedBones.Contains(bones[boneWeights[j].boneIndex0]))
                            boneWeights[j].boneIndex0 = usedBones.IndexOf(bones[boneWeights[j].boneIndex0]);
                        else
                        {
                            usedBones.Add(bones[boneWeights[j].boneIndex0]);
                            boneWeights[j].boneIndex0 = usedBones.IndexOf(bones[boneWeights[j].boneIndex0]);
                        } //else
                    } //if
                    else
                        boneWeights[j].boneIndex0 = 0;

                    if (boneWeights[j].weight1 > 0f)
                    {
                        if (usedBones.Contains(bones[boneWeights[j].boneIndex1]))
                            boneWeights[j].boneIndex1 = usedBones.IndexOf(bones[boneWeights[j].boneIndex1]);
                        else
                        {
                            usedBones.Add(bones[boneWeights[j].boneIndex1]);
                            boneWeights[j].boneIndex1 = usedBones.IndexOf(bones[boneWeights[j].boneIndex1]);
                        } //else
                    } //if
                    else
                        boneWeights[j].boneIndex1 = 0;

                    if (boneWeights[j].weight2 > 0f)
                    {
                        if (usedBones.Contains(bones[boneWeights[j].boneIndex2]))
                            boneWeights[j].boneIndex2 = usedBones.IndexOf(bones[boneWeights[j].boneIndex2]);
                        else
                        {
                            usedBones.Add(bones[boneWeights[j].boneIndex2]);
                            boneWeights[j].boneIndex2 = usedBones.IndexOf(bones[boneWeights[j].boneIndex2]);
                        } //else
                    } //if
                    else
                        boneWeights[j].boneIndex2 = 0;

                    if (boneWeights[j].weight3 > 0f)
                    {
                        if (usedBones.Contains(bones[boneWeights[j].boneIndex3]))
                            boneWeights[j].boneIndex3 = usedBones.IndexOf(bones[boneWeights[j].boneIndex3]);
                        else
                        {
                            usedBones.Add(bones[boneWeights[j].boneIndex3]);
                            boneWeights[j].boneIndex3 = usedBones.IndexOf(bones[boneWeights[j].boneIndex3]);
                        } //else
                    } //if
                    else
                        boneWeights[j].boneIndex3 = 0;
                } //for

                usedBonesArray = usedBones.ToArray();
                bindPoses = new Matrix4x4[usedBonesArray.Length];

                for (int j = 0; j < usedBonesArray.Length; j++)
                    bindPoses[j] = usedBones[j].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;

                meshes[i].vertices = vertices;
                meshes[i].normals = normals;
                meshes[i].tangents = tangents;
                meshes[i].colors = colors;
                meshes[i].boneWeights = boneWeights;
                meshes[i].uv = uv;
                meshes[i].uv2 = uv2;
                meshes[i].uv3 = uv3;
                meshes[i].uv4 = uv4;
                meshes[i].triangles = triangles;
                meshes[i].bindposes = bindPoses;

                skinnedMeshRenderer.bones = usedBonesArray;
                skinnedMeshRenderer.sharedMaterial = materials[fmdlMeshInfo.materialInstanceIndex];
                skinnedMeshRenderer.sharedMesh = meshes[i];

                foxModel.meshDefinitions[i].mesh = meshes[i];
                foxModel.meshDefinitions[i].meshGroup = Array.Find(fmdl.fmdlMeshGroupEntries, x => x.firstMeshIndex <= i && x.firstMeshIndex + x.meshCount > i).meshGroupIndex;
                foxModel.meshDefinitions[i].alpha = (FoxMeshDefinition.Alpha)fmdlMeshInfo.alphaEnum;
                foxModel.meshDefinitions[i].shadow = (FoxMeshDefinition.Shadow)fmdlMeshInfo.shadowEnum;

                if (isGZFormat)
                    name = fmdl.fmdlStrings[fmdl.fmdlMeshGroups[foxModel.meshDefinitions[i].meshGroup].nameIndex];
                else
                    name = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdl.fmdlMeshGroups[foxModel.meshDefinitions[i].meshGroup].nameIndex]);

                gameObject.name = $"{i} - {name}";
            } //for
        } //Read
    } //class
} //namespace