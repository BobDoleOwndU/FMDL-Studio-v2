using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System;
using System.Collections.Generic;

[ScriptedImporter(1, "fmdl")]
public class ExperimentalFmdlImporter: ScriptedImporter
{
    public void Read(AssetImportContext ctx, FileStream stream)
    {
        ExpFmdl fmdl = new ExpFmdl(Path.GetFileNameWithoutExtension(ctx.assetPath));
        fmdl.Read(stream);

        bool isGZFormat = fmdl.version == 2.03f;
        int boneCount = fmdl.fmdlBones != null ? fmdl.fmdlBones.Length : 0;
        int materialCount = fmdl.fmdlMaterialInstances.Length;
        int textureCount = fmdl.fmdlTextures != null ? fmdl.fmdlTextures.Length : 0;
        int meshCount = fmdl.fmdlMeshInfos.Length;
        int meshGroupCount = fmdl.fmdlMeshGroups.Length;

        GameObject mainObject = new GameObject(fmdl.name);
        ctx.AddObjectToAsset($"{mainObject.name}", mainObject);
        ctx.SetMainObject(mainObject);
        FoxModel foxModel = mainObject.AddComponent<FoxModel>();
        foxModel.meshGroups = new FoxMeshGroup[meshGroupCount];
        foxModel.meshDefinitions = new FoxMeshDefinition[meshCount];
        Transform[] bones = new Transform[boneCount];
        Texture[] textures = new Texture[textureCount];
        Material[] materials = new Material[materialCount];
        Mesh[] meshes = new Mesh[meshCount];

        Transform rootBone = new GameObject().transform;
        ctx.AddObjectToAsset("Root Bone", rootBone);
        rootBone.name = "[Root]";
        rootBone.parent = mainObject.transform;

        {
            ExpFmdl.FmdlBoundingBox fmdlBoundingBox = fmdl.fmdlBoundingBoxes[0];

            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3(-fmdlBoundingBox.min.x, fmdlBoundingBox.min.y, fmdlBoundingBox.min.z), new Vector3(-fmdlBoundingBox.max.x, fmdlBoundingBox.max.y, fmdlBoundingBox.max.z));
            BoxCollider boxCollider = rootBone.gameObject.AddComponent<BoxCollider>();
            ctx.AddObjectToAsset($"Root Bone BoxCollider", boxCollider);
            boxCollider.center = rootBone.InverseTransformPoint(bounds.center);
            boxCollider.size = bounds.size;
        } //code block

        for(int i = 0; i < boneCount; i++)
        {
            bones[i] = new GameObject().transform;
            ctx.AddObjectToAsset($"{i}", bones[i]);

            ExpFmdl.FmdlBone fmdlBone = fmdl.fmdlBones[i];
            ExpFmdl.FmdlBoundingBox fmdlBoundingBox = fmdl.fmdlBoundingBoxes[fmdlBone.boundingBoxIndex];

            //Get the name.
            if (isGZFormat)
                bones[i].name = fmdl.fmdlStrings[fmdlBone.nameIndex];
            else
                bones[i].name = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlBone.nameIndex]);

            //Assign the parent.
            if (fmdlBone.parentIndex != -1)
                bones[i].parent = bones[fmdlBone.parentIndex];
            else
                bones[i].parent = rootBone.transform;

            bones[i].position = new Vector3(-fmdlBone.worldPosition.x, fmdlBone.worldPosition.y, fmdlBone.worldPosition.z);

            //Set up the bounding box.
            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3(-fmdlBoundingBox.min.x, fmdlBoundingBox.min.y, fmdlBoundingBox.min.z), new Vector3(-fmdlBoundingBox.max.x, fmdlBoundingBox.max.y, fmdlBoundingBox.max.z));
            BoxCollider boxCollider = bones[i].gameObject.AddComponent<BoxCollider>();
            ctx.AddObjectToAsset($"Bone {i} BoxCollider", boxCollider);
            boxCollider.center = bones[i].InverseTransformPoint(bounds.center);
            boxCollider.size = bounds.size;
        } //for

        Globals.ReadTexturePath();

        for(int i = 0; i < textureCount; i++)
        {
            string name;

            ExpFmdl.FmdlTexture fmdlTexture = fmdl.fmdlTextures[i];

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

                        if(((j / 32) % 2 == 0 && (h / 32) % 2 != 0) || ((j / 32) % 2 != 0 && (h / 32) % 2 == 0))
                            c = new Color(0.5f, 0.5f, 0.5f);

                        texture.SetPixel(j, h, c);
                    } //for

                texture.Apply();

                textures[i] = texture;
            } //else

            ctx.AddObjectToAsset($"Texture {i}", textures[i]);
            textures[i].name = name;
        } //for

        for(int i = 0; i < materialCount; i++)
        {
            string shaderName;
            string materialName;

            ExpFmdl.FmdlMaterialInstance fmdlMaterialInstance = fmdl.fmdlMaterialInstances[i];

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
            ctx.AddObjectToAsset($"Material {i}", materials[i]);
            materials[i].name = materialName;

            //Link textures.
            for(int j = fmdlMaterialInstance.firstTextureIndex; j < fmdlMaterialInstance.firstTextureIndex + fmdlMaterialInstance.textureCount; j++)
            {
                string textureType;

                ExpFmdl.FmdlMaterialParameter fmdlMaterialParameter = fmdl.fmdlMaterialParameters[j];

                if (isGZFormat)
                    textureType = fmdl.fmdlStrings[fmdlMaterialParameter.nameIndex];
                else
                    textureType = Hashing.TryGetStringName(fmdl.fmdlStrCode64s[fmdlMaterialParameter.nameIndex]);

                materials[i].SetTexture(textureType, textures[fmdlMaterialParameter.referenceIndex]);
                materials[i].SetTextureScale(textureType, new Vector2(1, -1)); //Have to flip textures here because Texture2D.LoadRawData is bugged an imports DDS files upside down.
            } //for

            //Set parameters.
            for(int j = fmdlMaterialInstance.firstParameterIndex; j < fmdlMaterialInstance.firstParameterIndex + fmdlMaterialInstance.parameterCount; j++)
            {
                string parameterName;

                ExpFmdl.FmdlMaterialParameter fmdlMaterialParameter = fmdl.fmdlMaterialParameters[j];

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

            ExpFmdl.FmdlMeshGroup fmdlMeshGroup = fmdl.fmdlMeshGroups[i];

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
            ctx.AddObjectToAsset($"Mesh {i}", meshes[i]);
            GameObject gameObject = new GameObject();
            gameObject.transform.parent = mainObject.transform;
            SkinnedMeshRenderer skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
            foxModel.meshDefinitions[i] = new FoxMeshDefinition();

            ExpFmdl.FmdlMesh fmdlMesh = fmdl.fmdlMeshes[i];
            ExpFmdl.FmdlMeshInfo fmdlMeshInfo = fmdl.fmdlMeshInfos[i];
            ExpFmdl.FmdlBoneGroup fmdlBoneGroup = fmdl.fmdlBoneGroups != null ? fmdl.fmdlBoneGroups[fmdlMeshInfo.boneGroupIndex] : new ExpFmdl.FmdlBoneGroup();

            int vertexLength = fmdlMesh.vertices.Length;
            int faceLength = fmdlMesh.triangles.Length;
            string name;

            Vector3[] vertices = new Vector3[vertexLength];
            Vector3[] normals = new Vector3[vertexLength];
            Vector4[] tangents = new Vector4[vertexLength];
            Color[] colors = new Color[vertexLength];
            BoneWeight[] boneWeights = new BoneWeight[vertexLength];
            Vector2[] uv = new Vector2[vertexLength];
            Vector2[] uv2 = new Vector2[vertexLength];
            Vector2[] uv3 = new Vector2[vertexLength];
            Vector2[] uv4 = new Vector2[vertexLength];
            int[] triangles = new int[faceLength];
            Matrix4x4[] bindPoses;

            List<Transform> usedBones = new List<Transform>(0);
            Transform[] usedBonesArray;

            for (int j = 0; j < vertexLength; j++)
            {
                vertices[j] = new Vector3(-fmdlMesh.vertices[j].x, fmdlMesh.vertices[j].y, fmdlMesh.vertices[j].z);
                normals[j] = new Vector3(-fmdlMesh.normals[j].x, fmdlMesh.normals[j].y, fmdlMesh.normals[j].z);
                tangents[j] = new Vector4(-fmdlMesh.tangents[j].x, fmdlMesh.tangents[j].y, fmdlMesh.tangents[j].z, fmdlMesh.tangents[j].w);
                colors[j] = new Color(fmdlMesh.colors[j].x / 255f, fmdlMesh.colors[j].y / 255f, fmdlMesh.colors[j].z / 255f, fmdlMesh.colors[j].w / 255f);
                if (fmdl.fmdlBones != null)
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
                uv[j] = new Vector2(fmdlMesh.uv[j].x, -fmdlMesh.uv[j].y);
                uv2[j] = new Vector2(fmdlMesh.uv2[j].x, -fmdlMesh.uv2[j].y);
                uv3[j] = new Vector2(fmdlMesh.uv3[j].x, -fmdlMesh.uv3[j].y);
                uv4[j] = new Vector2(fmdlMesh.uv4[j].x, -fmdlMesh.uv4[j].y);
            } //for

            for (int j = 0; j < faceLength; j++)
                triangles[j] = fmdlMesh.triangles[j];
            
            for(int j = 0; j < boneWeights.Length; j++)
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

    public override void OnImportAsset(AssetImportContext ctx)
    {
        using (FileStream stream = new FileStream(ctx.assetPath, FileMode.Open))
        {
            try
            {
                Read(ctx, stream);
            } //try
            catch(Exception e)
            {
                stream.Close();
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            } //catch

            stream.Close();
        } //using
    } //OnImportAsset

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
} //class