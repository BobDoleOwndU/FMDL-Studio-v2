using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEditor;

[ScriptedImporter(1, "fmdl")]
public class ExperimentalFmdlImporter: ScriptedImporter
{
    public void Read(AssetImportContext ctx, FileStream stream)
    {
        Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(ctx.assetPath));
        fmdl.Read(stream);

        bool isGZFormat = fmdl.versionNum == 2.03f;
        int boneCount = fmdl.section0Block0Entries.Count;
        int materialCount = fmdl.section0Block4Entries.Count;
        int textureCount = fmdl.section0Block6Entries.Count;
        int meshCount = fmdl.section0Block3Entries.Count;
        int meshGroupCount = fmdl.section0Block1Entries.Count;

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
            Fmdl.Section0BlockDEntry section0BlockDEntry = fmdl.section0BlockDEntries[0];

            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3(-section0BlockDEntry.minX, section0BlockDEntry.minY, section0BlockDEntry.minZ), new Vector3(-section0BlockDEntry.maxX, section0BlockDEntry.maxY, section0BlockDEntry.maxZ));
            BoxCollider boxCollider = rootBone.gameObject.AddComponent<BoxCollider>();
            ctx.AddObjectToAsset($"Root Bone BoxCollider", boxCollider);
            boxCollider.center = rootBone.InverseTransformPoint(bounds.center);
            boxCollider.size = bounds.size;
        } //code block

        for(int i = 0; i < boneCount; i++)
        {
            bones[i] = new GameObject().transform;
            ctx.AddObjectToAsset($"{i}", bones[i]);

            Fmdl.Section0Block0Entry section0Block0Entry = fmdl.section0Block0Entries[i];
            Fmdl.Section0BlockDEntry section0BlockDEntry = fmdl.section0BlockDEntries[section0Block0Entry.boundingBoxId];

            //Get the name.
            if (isGZFormat)
                bones[i].name = fmdl.strings[section0Block0Entry.stringId];
            else
                bones[i].name = Hashing.TryGetStringName(fmdl.section0Block16Entries[section0Block0Entry.stringId]);

            //Assign the parent.
            if (section0Block0Entry.parentId != 0xFFFF)
                bones[i].parent = bones[section0Block0Entry.parentId];
            else
                bones[i].parent = rootBone.transform;

            bones[i].position = new Vector3(-section0Block0Entry.worldPositionX, section0Block0Entry.worldPositionY, section0Block0Entry.worldPositionZ);

            //Set up the bounding box.
            Bounds bounds = new Bounds();
            bounds.SetMinMax(new Vector3(-section0BlockDEntry.minX, section0BlockDEntry.minY, section0BlockDEntry.minZ), new Vector3(-section0BlockDEntry.maxX, section0BlockDEntry.maxY, section0BlockDEntry.maxZ));
            BoxCollider boxCollider = bones[i].gameObject.AddComponent<BoxCollider>();
            ctx.AddObjectToAsset($"Bone {i} BoxCollider", boxCollider);
            boxCollider.center = bones[i].InverseTransformPoint(bounds.center);
            boxCollider.size = bounds.size;
        } //for

        Globals.ReadTexturePath();

        for(int i = 0; i < textureCount; i++)
        {
            string name;

            Fmdl.Section0Block6Entry section0Block6Entry = fmdl.section0Block6Entries[i];

            //Get the name.
            if (isGZFormat)
            {
                name = fmdl.strings[section0Block6Entry.pathId] + fmdl.strings[section0Block6Entry.stringId];
                name = name.Substring(0, name.IndexOf(".")) + ".dds";
            } //if
            else
                name = Hashing.TryGetPathName(fmdl.section0Block15Entries[section0Block6Entry.pathId]) + ".dds";

            //Read the file.
            if (File.Exists($"{Globals.texturePath}\\{name}"))
                textures[i] = LoadTextureDXT($"{Globals.texturePath}\\{name}");
            else
                textures[i] = new Texture2D(0, 0);

            ctx.AddObjectToAsset($"Texture {i}", textures[i]);
            textures[i].name = name;
        } //for

        for(int i = 0; i < materialCount; i++)
        {
            string shaderName;
            string materialName;

            Fmdl.Section0Block4Entry section0Block4Entry = fmdl.section0Block4Entries[i];

            //Get the shader and material name.
            if (isGZFormat)
            {
                shaderName = fmdl.strings[fmdl.section0Block8Entries[section0Block4Entry.materialId].typeId];
                materialName = fmdl.strings[section0Block4Entry.stringId];
            } //if
            else
            {
                shaderName = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block8Entries[section0Block4Entry.materialId].typeId]);
                materialName = Hashing.TryGetStringName(fmdl.section0Block16Entries[section0Block4Entry.stringId]);
            } //else

            materials[i] = new Material(Shader.Find($"FoxShaders/{shaderName}"));
            ctx.AddObjectToAsset($"Material {i}", materials[i]);
            materials[i].name = materialName;

            //Link textures.
            for(int j = section0Block4Entry.firstTextureId; j < section0Block4Entry.firstTextureId + section0Block4Entry.numTextures; j++)
            {
                string textureType;

                Fmdl.Section0Block7Entry section0Block7Entry = fmdl.section0Block7Entries[j];

                if (isGZFormat)
                    textureType = fmdl.strings[section0Block7Entry.stringId];
                else
                    textureType = Hashing.TryGetStringName(fmdl.section0Block16Entries[section0Block7Entry.stringId]);

                materials[i].SetTexture(textureType, textures[section0Block7Entry.referenceId]);
                materials[i].SetTextureScale(textureType, new Vector2(1, -1)); //Have to flip textures here because Texture2D.LoadRawData is bugged an imports DDS files upside down.
            } //for

            //Set parameters.
            for(int j = section0Block4Entry.firstParameterId; j < section0Block4Entry.firstParameterId + section0Block4Entry.numParameters; j++)
            {
                string parameterName;

                Fmdl.Section0Block7Entry section0Block7Entry = fmdl.section0Block7Entries[j];

                if (isGZFormat)
                    parameterName = fmdl.strings[section0Block7Entry.stringId];
                else
                    parameterName = Hashing.TryGetStringName(fmdl.section0Block16Entries[section0Block7Entry.stringId]);

                materials[i].SetVector(parameterName, fmdl.materialParameters[section0Block7Entry.referenceId].values);
            } //for
        } //for

        for (int i = 0; i < meshGroupCount; i++)
        {
            foxModel.meshGroups[i] = new FoxMeshGroup();
            string name;

            Fmdl.Section0Block1Entry section0Block1Entry = fmdl.section0Block1Entries[i];

            if (isGZFormat)
                name = fmdl.strings[section0Block1Entry.stringId];
            else
                name = Hashing.TryGetStringName(fmdl.section0Block16Entries[section0Block1Entry.stringId]);

            foxModel.meshGroups[i].name = name;
            foxModel.meshGroups[i].parent = (short)section0Block1Entry.parentId;

            if (section0Block1Entry.invisibilityFlag == 0)
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

            Fmdl.Object obj = fmdl.objects[i];
            Fmdl.Section0Block3Entry section0Block3Entry = fmdl.section0Block3Entries[i];
            Fmdl.Section0Block5Entry section0Block5Entry = fmdl.section0Block5Entries[section0Block3Entry.boneGroupId];

            int vertexLength = obj.vertices.Length;
            int faceLength = obj.faces.Length;
            string name;

            Vector3[] vertices = new Vector3[vertexLength];
            Vector3[] normals = new Vector3[vertexLength];
            Vector4[] tangents = new Vector4[vertexLength];
            Color[] colors = new Color[vertexLength];
            BoneWeight[] boneWeights = new BoneWeight[vertexLength];
            Vector2[] uv = new Vector2[vertexLength];
            int[] triangles = new int[faceLength * 3];
            Matrix4x4[] bindPoses = new Matrix4x4[boneCount];

            for (int j = 0; j < vertexLength; j++)
            {
                vertices[j] = new Vector3(-obj.vertices[j].x, obj.vertices[j].y, obj.vertices[j].z);
                normals[j] = new Vector3(-obj.additionalVertexData[j].normalX, obj.additionalVertexData[j].normalY, obj.additionalVertexData[j].normalZ);
                tangents[j] = new Vector4(-obj.additionalVertexData[j].tangentX, obj.additionalVertexData[j].tangentY, obj.additionalVertexData[j].tangentZ, obj.additionalVertexData[j].tangentW);
                colors[j] = new Color(obj.additionalVertexData[j].colourR, obj.additionalVertexData[j].colourG, obj.additionalVertexData[j].colourB, obj.additionalVertexData[j].colourA);
                boneWeights[j].weight0 = obj.additionalVertexData[j].boneWeightX;
                boneWeights[j].weight1 = obj.additionalVertexData[j].boneWeightY;
                boneWeights[j].weight2 = obj.additionalVertexData[j].boneWeightZ;
                boneWeights[j].weight3 = obj.additionalVertexData[j].boneWeightW;
                boneWeights[j].boneIndex0 = section0Block5Entry.entries[obj.additionalVertexData[j].boneGroup0Id];
                boneWeights[j].boneIndex1 = section0Block5Entry.entries[obj.additionalVertexData[j].boneGroup1Id];
                boneWeights[j].boneIndex2 = section0Block5Entry.entries[obj.additionalVertexData[j].boneGroup2Id];
                boneWeights[j].boneIndex3 = section0Block5Entry.entries[obj.additionalVertexData[j].boneGroup3Id];
                uv[j] = new Vector2(obj.additionalVertexData[j].textureU, -obj.additionalVertexData[j].textureV);
            } //for

            for(int j = 0, h = 0; j < faceLength; j++, h += 3)
            {
                triangles[h] = obj.faces[j].vertex1Id;
                triangles[h + 1] = obj.faces[j].vertex2Id;
                triangles[h + 2] = obj.faces[j].vertex3Id;
            } //for

            for (int j = 0; j < boneCount; j++)
                bindPoses[j] = bones[j].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;

            meshes[i].vertices = vertices;
            meshes[i].normals = normals;
            meshes[i].tangents = tangents;
            meshes[i].colors = colors;
            meshes[i].boneWeights = boneWeights;
            meshes[i].uv = uv;
            meshes[i].triangles = triangles;
            meshes[i].bindposes = bindPoses;

            skinnedMeshRenderer.bones = bones;
            skinnedMeshRenderer.sharedMaterial = materials[section0Block3Entry.materialInstanceId];
            skinnedMeshRenderer.sharedMesh = meshes[i];

            foxModel.meshDefinitions[i].mesh = meshes[i];
            foxModel.meshDefinitions[i].meshGroup = fmdl.section0Block2Entries.Find(x => x.firstObjectId <= i && x.firstObjectId + x.numObjects > i).meshGroupId;
            foxModel.meshDefinitions[i].alpha = (FoxMeshDefinition.Alpha)section0Block3Entry.alphaEnum;
            foxModel.meshDefinitions[i].shadow = (FoxMeshDefinition.Shadow)section0Block3Entry.shadowEnum;

            if (isGZFormat)
                name = fmdl.strings[fmdl.section0Block1Entries[foxModel.meshDefinitions[i].meshGroup].stringId];
            else
                name = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block1Entries[foxModel.meshDefinitions[i].meshGroup].stringId]);

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