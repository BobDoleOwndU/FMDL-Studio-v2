using System;
using System.IO;
using UnityEngine;

public class UnityModel
{
    private struct UnityMesh
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public BoneWeight[] boneWeights;
        public Vector2[] UVs;
        public int[] faces;
    } //struct

    //Apparently you can't extend Unity classes. So we have to use a struct instead.
    private struct UnityMaterial
    {
        public Material material;
        public string materialType;
        public string textureType;
    } //struct

    //Instance Variables
    private UnityMesh[] meshes;

    public void GetDataFromFmdl(Fmdl fmdl)
    {
        meshes = new UnityMesh[fmdl.GetSection0Block3Entries().Length];

        GameObject fmdlGameObject = new GameObject();
        fmdlGameObject.name = fmdl.GetName();
        GameObject[] subFmdlGameObjects = new GameObject[fmdl.GetObjects().Length];
        Transform[] bones;
        Matrix4x4[] bindPoses;
        Bounds[] bounds = new Bounds[fmdl.GetSection0BlockDEntries().Length];

        UnityMaterial[] materials = new UnityMaterial[fmdl.GetSection0Block4Entries().Length];

        if (fmdl.GetBonesPosition() != -1)
        {
            bones = new Transform[fmdl.GetSection0Block0Entries().Length];
            bindPoses = new Matrix4x4[fmdl.GetSection0Block0Entries().Length];
        } //if
        else
        {
            bones = new Transform[0];
            bindPoses = new Matrix4x4[0];
        } //else

        for(int i = 0; i < fmdl.GetSection0Block4Entries().Length; i++)
        {
            //materials[i].material = new Material(Shader.Find("Custom/Fox Shader (temp)"));
            materials[i].material = new Material(Shader.Find("Legacy Shaders/Transparent/Cutout/Diffuse"));

            if (fmdl.GetStringTablePosition() != -1)
            {
                materials[i].material.name = fmdl.GetStrings()[fmdl.GetSection0Block8Entries()[fmdl.GetSection0Block4Entries()[i].materialId].nameId];
                materials[i].materialType = fmdl.GetStrings()[fmdl.GetSection0Block8Entries()[fmdl.GetSection0Block4Entries()[i].materialId].typeId];

                string textureName = "";
                int extensionLocation;

                textureName = fmdl.GetStrings()[fmdl.GetSection0Block6Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].referenceId].pathId] + fmdl.GetStrings()[fmdl.GetSection0Block6Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].referenceId].nameId];
                materials[i].textureType = fmdl.GetStrings()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].nameId];

                extensionLocation = textureName.IndexOf('.');
                textureName = textureName.Substring(0, extensionLocation);

                if (File.Exists(fmdl.GetName() + "\\" + textureName + ".dds"))
                {
                    Texture2D texture = LoadTextureDXT(fmdl.GetName() + "\\" + textureName + ".dds");
                    texture.name = textureName + ".dds";
                    materials[i].material.mainTexture = texture;
                } //if
                else
                {
                    UnityEngine.Debug.Log("Could not find: " + fmdl.GetName() + "\\" + textureName + ".dds");
                } //else
            } //if
            else
            {
                materials[i].material.name = Hashing.TryGetStringName(fmdl.GetSection0Block16Entries()[fmdl.GetSection0Block8Entries()[fmdl.GetSection0Block4Entries()[i].materialId].nameId]);
                materials[i].materialType = Hashing.TryGetStringName(fmdl.GetSection0Block16Entries()[fmdl.GetSection0Block8Entries()[fmdl.GetSection0Block4Entries()[i].materialId].typeId]);

                if (fmdl.GetTextureListPosition() != -1)
                {
                    if (File.Exists(fmdl.GetName() + "\\" + Hashing.TryGetPathName(fmdl.GetSection0Block15Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].referenceId]) + ".dds"))
                    {
                        Texture2D texture = LoadTextureDXT(fmdl.GetName() + "\\" + Hashing.TryGetPathName(fmdl.GetSection0Block15Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].referenceId]) + ".dds");
                        texture.name = Hashing.TryGetPathName(fmdl.GetSection0Block15Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].referenceId]) + ".dds";
                        materials[i].textureType = Hashing.TryGetStringName(fmdl.GetSection0Block16Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].nameId]);
                        materials[i].material.mainTexture = texture;
                    } //if
                    else
                    {
                        UnityEngine.Debug.Log("Could not find: " + fmdl.GetName() + "\\" + Hashing.TryGetPathName(fmdl.GetSection0Block15Entries()[fmdl.GetSection0Block7Entries()[fmdl.GetSection0Block4Entries()[i].firstTexture].referenceId]) + ".dds");
                    } //else
                } //if
            } //else
        } //for

        for(int i = 0; i < bounds.Length; i++)
        {
            bounds[i].SetMinMax(new Vector3(fmdl.GetSection0BlockDEntries()[i].minZ, fmdl.GetSection0BlockDEntries()[i].minY, fmdl.GetSection0BlockDEntries()[i].minX), new Vector3(fmdl.GetSection0BlockDEntries()[i].maxZ, fmdl.GetSection0BlockDEntries()[i].maxY, fmdl.GetSection0BlockDEntries()[i].maxX));
            //bounds[i].SetMinMax(new Vector3(-100.0f, -100.0f, -100.0f), new Vector3(100.0f, 100.0f, 100.0f));
        } //for

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = new GameObject().transform;
            bones[i].position = new Vector3(fmdl.GetSection0Block0Entries()[i].worldPositionZ, fmdl.GetSection0Block0Entries()[i].worldPositionY, fmdl.GetSection0Block0Entries()[i].worldPositionX);

            if(fmdl.GetStringTablePosition() != -1)
                bones[i].name = fmdl.GetStrings()[fmdl.GetSection0Block0Entries()[i].nameId];
            else
                bones[i].name = Hashing.TryGetStringName(fmdl.GetSection0Block16Entries()[fmdl.GetSection0Block0Entries()[i].nameId]);

            if (fmdl.GetSection0Block0Entries()[i].parentId == 0xFFFF)
                bones[i].parent = fmdlGameObject.transform;
            else
                bones[i].parent = bones[fmdl.GetSection0Block0Entries()[i].parentId];
        } //for

        for (int i = 0; i < fmdl.GetObjects().Length; i++)
        {
            meshes[i].vertices = new Vector3[fmdl.GetObjects()[i].vertices.Length];
            meshes[i].normals = new Vector3[fmdl.GetObjects()[i].additionalVertexData.Length];
            meshes[i].UVs = new Vector2[fmdl.GetObjects()[i].additionalVertexData.Length];
            meshes[i].faces = new int[fmdl.GetObjects()[i].faces.Length * 3];
            meshes[i].boneWeights = new BoneWeight[fmdl.GetObjects()[i].additionalVertexData.Length];

            //Position
            for (int j = 0; j < fmdl.GetObjects()[i].vertices.Length; j++)
                meshes[i].vertices[j] = new Vector3(fmdl.GetObjects()[i].vertices[j].z, fmdl.GetObjects()[i].vertices[j].y, fmdl.GetObjects()[i].vertices[j].x);

            //Normals, Bone Weights, Bone Group Ids and UVs
            for (int j = 0; j < fmdl.GetObjects()[i].additionalVertexData.Length; j++)
            {
                meshes[i].normals[j] = new Vector3(fmdl.GetObjects()[i].additionalVertexData[j].normalZ, fmdl.GetObjects()[i].additionalVertexData[j].normalY, fmdl.GetObjects()[i].additionalVertexData[j].normalX);

                if (fmdl.GetBonesPosition() != -1)
                {
                    meshes[i].boneWeights[j].weight0 = fmdl.GetObjects()[i].additionalVertexData[j].boneWeightX;
                    meshes[i].boneWeights[j].weight1 = fmdl.GetObjects()[i].additionalVertexData[j].boneWeightY;
                    meshes[i].boneWeights[j].weight2 = fmdl.GetObjects()[i].additionalVertexData[j].boneWeightZ;
                    meshes[i].boneWeights[j].weight3 = fmdl.GetObjects()[i].additionalVertexData[j].boneWeightW;
                    meshes[i].boneWeights[j].boneIndex0 = fmdl.GetSection0Block5Entries()[fmdl.GetSection0Block3Entries()[i].boneGroupId].entries[fmdl.GetObjects()[i].additionalVertexData[j].boneGroup0Id];
                    meshes[i].boneWeights[j].boneIndex1 = fmdl.GetSection0Block5Entries()[fmdl.GetSection0Block3Entries()[i].boneGroupId].entries[fmdl.GetObjects()[i].additionalVertexData[j].boneGroup1Id];
                    meshes[i].boneWeights[j].boneIndex2 = fmdl.GetSection0Block5Entries()[fmdl.GetSection0Block3Entries()[i].boneGroupId].entries[fmdl.GetObjects()[i].additionalVertexData[j].boneGroup2Id];
                    meshes[i].boneWeights[j].boneIndex3 = fmdl.GetSection0Block5Entries()[fmdl.GetSection0Block3Entries()[i].boneGroupId].entries[fmdl.GetObjects()[i].additionalVertexData[j].boneGroup3Id];
                } //if

                meshes[i].UVs[j] = new Vector2(fmdl.GetObjects()[i].additionalVertexData[j].textureU, fmdl.GetObjects()[i].additionalVertexData[j].textureV);
            } //for

            //Faces
            for (int j = 0, h = 0; j < fmdl.GetObjects()[i].faces.Length; j++, h += 3)
            {
                meshes[i].faces[h] = fmdl.GetObjects()[i].faces[j].vertex1Id;
                meshes[i].faces[h + 1] = fmdl.GetObjects()[i].faces[j].vertex2Id;
                meshes[i].faces[h + 2] = fmdl.GetObjects()[i].faces[j].vertex3Id;
            } //for

            //Render the mesh in Unity.
            subFmdlGameObjects[i] = new GameObject();

            //Get the mesh name.
            for (int j = 0; j < fmdl.GetSection0Block2Entries().Length; j++)
            {
                if (i >= fmdl.GetSection0Block2Entries()[j].numPrecedingObjects && i < fmdl.GetSection0Block2Entries()[j].numPrecedingObjects + fmdl.GetSection0Block2Entries()[j].numObjects)
                {
                    if(fmdl.GetStringTablePosition() != -1)
                        subFmdlGameObjects[i].name = i + " - " + fmdl.GetStrings()[fmdl.GetSection0Block1Entries()[fmdl.GetSection0Block2Entries()[j].meshGroupId].nameId];
                    else
                        subFmdlGameObjects[i].name = i + " - " + Hashing.TryGetStringName(fmdl.GetSection0Block16Entries()[fmdl.GetSection0Block1Entries()[fmdl.GetSection0Block2Entries()[j].meshGroupId].nameId]);
                    break;
                } //if
            } //for

            subFmdlGameObjects[i].transform.parent = fmdlGameObject.transform;
            SkinnedMeshRenderer meshRenderer = subFmdlGameObjects[i].AddComponent<SkinnedMeshRenderer>();

            meshRenderer.material = materials[fmdl.GetSection0Block3Entries()[i].materialInstanceId].material;
            //meshRenderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");

            Mesh mesh = new Mesh();
            mesh.vertices = meshes[i].vertices;
            mesh.uv = meshes[i].UVs;
            mesh.normals = meshes[i].normals;
            mesh.triangles = meshes[i].faces;
            mesh.boneWeights = meshes[i].boneWeights;

            for (int j = 0; j < bones.Length; j++)
            {
                bindPoses[j] = bones[j].worldToLocalMatrix * subFmdlGameObjects[i].transform.localToWorldMatrix;
            } //for

            mesh.bindposes = bindPoses;
            //mesh.bounds = bounds[fmdl.GetSection0Block9Entries()[i].boundingBoxId]; Not right. Boxes don't match up to meshes. Need to figure out what's actually done.

            meshRenderer.bones = bones;
            meshRenderer.sharedMesh = mesh;
            subFmdlGameObjects[i].AddComponent<MeshCollider>();
        } //for
    } //GetDataFromFmdl

    public static Texture2D LoadTextureDXT(string path)
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
