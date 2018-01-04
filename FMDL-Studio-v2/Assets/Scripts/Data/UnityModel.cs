using System;
using System.IO;
using UnityEngine;

public class UnityModel
{
    private struct UnityMesh
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector4[] tangents;
        public Color[] colour;
        public BoneWeight[] boneWeights;
        public Vector2[] uv;
        public Vector2[] uv2;
        public Vector2[] uv3;
        public Vector2[] uv4;
        public int[] faces;
    } //struct

    //Apparently you can't extend Unity classes. So we have to use a struct instead.
    private struct UnityMaterial
    {
        public Material material;
        public string materialName;
        public string materialType;
    } //struct

    //Instance Variables
    private UnityMesh[] meshes;

    public GameObject GetDataFromFmdl(Fmdl fmdl)
    {
        Globals.ReadTexturePath();

        meshes = new UnityMesh[fmdl.section0Block3Entries.Count];

        GameObject fmdlGameObject = new GameObject();
        fmdlGameObject.name = fmdl.name;
        GameObject[] subFmdlGameObjects = new GameObject[fmdl.objects.Count];
        Transform[] bones;
        Matrix4x4[] bindPoses;
        Bounds[] bounds = new Bounds[fmdl.section0BlockDEntries.Count];

        fmdlGameObject.AddComponent<FoxModel>().definitions = new FoxMeshDefinition[fmdl.objects.Count];
        UnityMaterial[] materials = new UnityMaterial[fmdl.section0Block4Entries.Count];

        if (fmdl.bonesIndex != -1)
        {
            bones = new Transform[fmdl.section0Block0Entries.Count];
            bindPoses = new Matrix4x4[fmdl.section0Block0Entries.Count];
        } //if
        else
        {
            bones = new Transform[0];
            bindPoses = new Matrix4x4[0];
        } //else

        for(int i = 0; i < fmdl.section0Block4Entries.Count; i++)
        {
            materials[i].material = new Material(Shader.Find("FoxShaders/Base"));
            //materials[i].material = new Material(Shader.Find("Legacy Shaders/Transparent/Cutout/Bumped Diffuse"));
            //materials[i].material = new Material(Shader.Find("Standard"));

            if (fmdl.stringsIndex != -1)
            {
                materials[i].material.name = fmdl.strings[fmdl.section0Block4Entries[i].stringId];
                materials[i].materialName = fmdl.strings[fmdl.section0Block8Entries[fmdl.section0Block4Entries[i].materialId].stringId];
                materials[i].materialType = fmdl.strings[fmdl.section0Block8Entries[fmdl.section0Block4Entries[i].materialId].typeId];

                for (int j = fmdl.section0Block4Entries[i].firstTextureId; j < fmdl.section0Block4Entries[i].firstTextureId + fmdl.section0Block4Entries[i].numTextures; j++)
                {
                    string textureName = "";
                    int extensionLocation;

                    textureName = fmdl.strings[fmdl.section0Block6Entries[fmdl.section0Block7Entries[j].referenceId].pathId] + fmdl.strings[fmdl.section0Block6Entries[fmdl.section0Block7Entries[j].referenceId].stringId];
                    //materials[i].textureType = fmdl.strings[fmdl.section0Block7Entries[fmdl.section0Block4Entries[i].firstTextureId].stringId];

                    extensionLocation = textureName.IndexOf('.');
                    textureName = textureName.Substring(0, extensionLocation);

                    if (File.Exists(Globals.texturePath + "\\" + textureName + ".dds"))
                    {
                        Texture2D texture = LoadTextureDXT(Globals.texturePath + "\\" + textureName + ".dds");
                        texture.name = textureName + ".dds";

                        if (fmdl.strings[fmdl.section0Block7Entries[j].stringId] == "Base_Tex_SRGB")
                            materials[i].material.mainTexture = texture;
                        else if (fmdl.strings[fmdl.section0Block7Entries[j].stringId] == "NormalMap_Tex_NRM")
                            materials[i].material.SetTexture("_BumpMap", texture);
                        else if (fmdl.strings[fmdl.section0Block7Entries[j].stringId] == "SpecularMap_Tex_LIN")
                            materials[i].material.SetTexture("_SRM", texture);
                        else if (fmdl.strings[fmdl.section0Block7Entries[j].stringId] == "Layer_Tex_SRGB")
                            materials[i].material.SetTexture("_LayerTex", texture);
                        else if (fmdl.strings[fmdl.section0Block7Entries[j].stringId] == "LayerMask_Tex_LIN")
                            materials[i].material.SetTexture("_LayerMask", texture);
                        //_MainTex = Diffuse. _BumpMap = Normal Map. _Color = Main Colour. _SpecColor = Specular Map. _Shininess.
                    } //if
                    else
                    {
                        UnityEngine.Debug.Log("Could not find: " + Globals.texturePath + "\\" + textureName + ".dds");
                    } //else
                } //for
            } //if
            else
            {
                materials[i].material.name = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block4Entries[i].stringId]);
                materials[i].materialName = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block8Entries[fmdl.section0Block4Entries[i].materialId].stringId]);
                materials[i].materialType = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block8Entries[fmdl.section0Block4Entries[i].materialId].typeId]);

                if (fmdl.texturePathsIndex != -1)
                {
                    for (int j = fmdl.section0Block4Entries[i].firstTextureId; j < fmdl.section0Block4Entries[i].firstTextureId + fmdl.section0Block4Entries[i].numTextures; j++)
                    {
                        if (File.Exists(Globals.texturePath + "\\" + Hashing.TryGetPathName(fmdl.section0Block15Entries[fmdl.section0Block7Entries[j].referenceId]) + ".dds"))
                        {
                            Texture2D texture = LoadTextureDXT(Globals.texturePath + "\\" + Hashing.TryGetPathName(fmdl.section0Block15Entries[fmdl.section0Block7Entries[j].referenceId]) + ".dds");
                            texture.name = Hashing.TryGetPathName(fmdl.section0Block15Entries[fmdl.section0Block7Entries[j].referenceId]) + ".dds";
                            //materials[i].textureType = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block7Entries[fmdl.section0Block4Entries[i].firstTextureId].stringId]);

                            if (Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block7Entries[j].stringId]) == "Base_Tex_SRGB")
                                materials[i].material.mainTexture = texture;
                            else if (Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block7Entries[j].stringId]) == "NormalMap_Tex_NRM")
                                materials[i].material.SetTexture("_BumpMap", texture);
                            else if (Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block7Entries[j].stringId]) == "SpecularMap_Tex_LIN")
                                materials[i].material.SetTexture("_SRM", texture);
                            else if (Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block7Entries[j].stringId]) == "Layer_Tex_SRGB")
                                materials[i].material.SetTexture("_LayerTex", texture);
                            else if (Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block7Entries[j].stringId]) == "LayerMask_Tex_LIN")
                                materials[i].material.SetTexture("_LayerMask", texture);
                        } //if
                        else
                        {
                            UnityEngine.Debug.Log("Could not find: " + Globals.texturePath + "\\" + Hashing.TryGetPathName(fmdl.section0Block15Entries[fmdl.section0Block7Entries[j].referenceId]) + ".dds");
                        } //else
                    } //for
                } //if
            } //else
        } //for

        for(int i = 0; i < bounds.Length; i++)
        {
            bounds[i].SetMinMax(new Vector3(fmdl.section0BlockDEntries[i].minZ, fmdl.section0BlockDEntries[i].minY, fmdl.section0BlockDEntries[i].minX), new Vector3(fmdl.section0BlockDEntries[i].maxZ, fmdl.section0BlockDEntries[i].maxY, fmdl.section0BlockDEntries[i].maxX));
        } //for

        Transform rootBone = new GameObject("[Root]").transform; //From what I can tell, the real name is "". But it looks kinda dumb having "" as its name; so using "[Root]" as a placeholder seems better.
        rootBone.parent = fmdlGameObject.transform;

        {
            BoxCollider collider = rootBone.gameObject.AddComponent<BoxCollider>();
            collider.center = rootBone.InverseTransformPoint(bounds[0].center); //Have to convert these to local positions. They're stored as world positions.
            collider.size = bounds[0].size;
        } //code block

        for (int i = 0; i < bones.Length; i++)
        {
            bones[i] = new GameObject().transform;
            bones[i].position = new Vector3(fmdl.section0Block0Entries[i].worldPositionZ, fmdl.section0Block0Entries[i].worldPositionY, fmdl.section0Block0Entries[i].worldPositionX);
            
            BoxCollider collider = bones[i].gameObject.AddComponent<BoxCollider>();
            collider.center = bones[i].InverseTransformPoint(bounds[fmdl.section0Block0Entries[i].boundingBoxId].center); //Have to convert these to local positions. They're stored as world positions.
            collider.size = bounds[fmdl.section0Block0Entries[i].boundingBoxId].size;

            if (fmdl.stringsIndex != -1)
                bones[i].name = fmdl.strings[fmdl.section0Block0Entries[i].stringId];
            else
                bones[i].name = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block0Entries[i].stringId]);

            if (fmdl.section0Block0Entries[i].parentId != 0xFFFF)
                bones[i].parent = bones[fmdl.section0Block0Entries[i].parentId];
            else
                bones[i].parent = rootBone;
        } //for

        for (int i = 0; i < fmdl.objects.Count; i++)
        {
            int lod = 0; //Temporary solution. 0 loads the normal faces. 1 loads the first set of LOD faces. 2 loads the next set. Etc....

            meshes[i].vertices = new Vector3[fmdl.objects[i].vertices.Length];
            meshes[i].normals = new Vector3[fmdl.objects[i].additionalVertexData.Length];
            meshes[i].tangents = new Vector4[fmdl.objects[i].additionalVertexData.Length];
            meshes[i].colour = new Color[fmdl.objects[i].additionalVertexData.Length];
            meshes[i].uv = new Vector2[fmdl.objects[i].additionalVertexData.Length];
            meshes[i].uv2 = new Vector2[fmdl.objects[i].additionalVertexData.Length];
            meshes[i].uv3 = new Vector2[fmdl.objects[i].additionalVertexData.Length];
            meshes[i].uv4 = new Vector2[fmdl.objects[i].additionalVertexData.Length];
            //meshes[i].faces = new int[fmdl.objects[i].faces.Length * 3];
            meshes[i].faces = new int[fmdl.objects[i].lodFaces[lod].Length * 3];
            meshes[i].boneWeights = new BoneWeight[fmdl.objects[i].additionalVertexData.Length];

            //Position
            for (int j = 0; j < fmdl.objects[i].vertices.Length; j++)
                meshes[i].vertices[j] = new Vector3(fmdl.objects[i].vertices[j].z, fmdl.objects[i].vertices[j].y, fmdl.objects[i].vertices[j].x);

            //Normals, Bone Weights, Bone Group Ids and UVs
            for (int j = 0; j < fmdl.objects[i].additionalVertexData.Length; j++)
            {
                meshes[i].normals[j] = new Vector3(fmdl.objects[i].additionalVertexData[j].normalZ, fmdl.objects[i].additionalVertexData[j].normalY, fmdl.objects[i].additionalVertexData[j].normalX);
                meshes[i].tangents[j] = new Vector4(fmdl.objects[i].additionalVertexData[j].tangentZ, fmdl.objects[i].additionalVertexData[j].tangentY, fmdl.objects[i].additionalVertexData[j].tangentX, fmdl.objects[i].additionalVertexData[j].tangentW);
                meshes[i].colour[j] = new Color(fmdl.objects[i].additionalVertexData[j].colourR, fmdl.objects[i].additionalVertexData[j].colourG, fmdl.objects[i].additionalVertexData[j].colourB, fmdl.objects[i].additionalVertexData[j].colourA);

                if (fmdl.bonesIndex != -1)
                {
                    meshes[i].boneWeights[j].weight0 = fmdl.objects[i].additionalVertexData[j].boneWeightX;
                    meshes[i].boneWeights[j].weight1 = fmdl.objects[i].additionalVertexData[j].boneWeightY;
                    meshes[i].boneWeights[j].weight2 = fmdl.objects[i].additionalVertexData[j].boneWeightZ;
                    meshes[i].boneWeights[j].weight3 = fmdl.objects[i].additionalVertexData[j].boneWeightW;
                    meshes[i].boneWeights[j].boneIndex0 = fmdl.section0Block5Entries[fmdl.section0Block3Entries[i].boneGroupId].entries[fmdl.objects[i].additionalVertexData[j].boneGroup0Id];
                    meshes[i].boneWeights[j].boneIndex1 = fmdl.section0Block5Entries[fmdl.section0Block3Entries[i].boneGroupId].entries[fmdl.objects[i].additionalVertexData[j].boneGroup1Id];
                    meshes[i].boneWeights[j].boneIndex2 = fmdl.section0Block5Entries[fmdl.section0Block3Entries[i].boneGroupId].entries[fmdl.objects[i].additionalVertexData[j].boneGroup2Id];
                    meshes[i].boneWeights[j].boneIndex3 = fmdl.section0Block5Entries[fmdl.section0Block3Entries[i].boneGroupId].entries[fmdl.objects[i].additionalVertexData[j].boneGroup3Id];
                } //if

                meshes[i].uv[j] = new Vector2(fmdl.objects[i].additionalVertexData[j].textureU, -fmdl.objects[i].additionalVertexData[j].textureV);
                meshes[i].uv2[j] = new Vector2(fmdl.objects[i].additionalVertexData[j].unknownU0, -fmdl.objects[i].additionalVertexData[j].unknownV0);
                meshes[i].uv3[j] = new Vector2(fmdl.objects[i].additionalVertexData[j].unknownU1, -fmdl.objects[i].additionalVertexData[j].unknownV1);
                meshes[i].uv4[j] = new Vector2(fmdl.objects[i].additionalVertexData[j].unknownU2, -fmdl.objects[i].additionalVertexData[j].unknownV2);
            } //for

            //Faces
            /*for (int j = 0, h = 0; j < fmdl.objects[i].faces.Length; j++, h += 3)
            {
                meshes[i].faces[h] = fmdl.objects[i].faces[j].vertex1Id;
                meshes[i].faces[h + 1] = fmdl.objects[i].faces[j].vertex2Id;
                meshes[i].faces[h + 2] = fmdl.objects[i].faces[j].vertex3Id;
            } //for*/

            for (int j = 0, h = 0; j < fmdl.objects[i].lodFaces[lod].Length; j++, h += 3)
            {
                meshes[i].faces[h] = fmdl.objects[i].lodFaces[lod][j].vertex1Id;
                meshes[i].faces[h + 1] = fmdl.objects[i].lodFaces[lod][j].vertex2Id;
                meshes[i].faces[h + 2] = fmdl.objects[i].lodFaces[lod][j].vertex3Id;
            } //for

            //Render the mesh in Unity.
            subFmdlGameObjects[i] = new GameObject();
            FoxMeshDefinition foxMeshDefinition = new FoxMeshDefinition();

            //Get the mesh name.
            for (int j = 0; j < fmdl.section0Block2Entries.Count; j++)
            {
                if (i >= fmdl.section0Block2Entries[j].firstObjectId && i < fmdl.section0Block2Entries[j].firstObjectId + fmdl.section0Block2Entries[j].numObjects)
                {
                    if (fmdl.stringsIndex != -1)
                    {
                        subFmdlGameObjects[i].name = i + " - " + fmdl.strings[fmdl.section0Block1Entries[fmdl.section0Block2Entries[j].meshGroupId].stringId];
                        foxMeshDefinition.meshGroup = fmdl.strings[fmdl.section0Block1Entries[fmdl.section0Block2Entries[j].meshGroupId].stringId];
                    } //if
                    else
                    {
                        subFmdlGameObjects[i].name = i + " - " + Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block1Entries[fmdl.section0Block2Entries[j].meshGroupId].stringId]);
                        foxMeshDefinition.meshGroup = Hashing.TryGetStringName(fmdl.section0Block16Entries[fmdl.section0Block1Entries[fmdl.section0Block2Entries[j].meshGroupId].stringId]);
                    } //else

                    break;
                } //if
            } //for

            subFmdlGameObjects[i].transform.parent = fmdlGameObject.transform;
            SkinnedMeshRenderer meshRenderer = subFmdlGameObjects[i].AddComponent<SkinnedMeshRenderer>();

            meshRenderer.material = materials[fmdl.section0Block3Entries[i].materialInstanceId].material;

            //have to apply a flip here because Texture2D.LoadRawData is bugged and loads dds images upside down.
            meshRenderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1, -1));
            meshRenderer.sharedMaterial.SetTextureScale("_BumpMap", new Vector2(1, -1));
            meshRenderer.sharedMaterial.SetTextureScale("_SRM", new Vector2(1, -1));
            meshRenderer.sharedMaterial.SetTextureScale("_LayerTex", new Vector2(1, -1));
            meshRenderer.sharedMaterial.SetTextureScale("_LayerMask", new Vector2(1, -1));

            foxMeshDefinition.material = materials[fmdl.section0Block3Entries[i].materialInstanceId].materialName;
            foxMeshDefinition.materialType = materials[fmdl.section0Block3Entries[i].materialInstanceId].materialType;

            Mesh mesh = new Mesh();
            foxMeshDefinition.mesh = mesh;

            mesh.vertices = meshes[i].vertices;
            mesh.uv = meshes[i].uv;
            mesh.uv2 = meshes[i].uv2;
            mesh.uv3 = meshes[i].uv3;
            mesh.uv4 = meshes[i].uv4;
            mesh.normals = meshes[i].normals;
            mesh.tangents = meshes[i].tangents;
            mesh.triangles = meshes[i].faces;
            mesh.boneWeights = meshes[i].boneWeights;

            for (int j = 0; j < bones.Length; j++)
            {
                bindPoses[j] = bones[j].worldToLocalMatrix * subFmdlGameObjects[i].transform.localToWorldMatrix;
            } //for

            mesh.bindposes = bindPoses;

            meshRenderer.bones = bones;
            meshRenderer.sharedMesh = mesh;

            fmdlGameObject.GetComponent<FoxModel>().definitions[i] = foxMeshDefinition;
            subFmdlGameObjects[i].AddComponent<MeshCollider>();
        } //for
        return fmdlGameObject;
    } //GetDataFromFmdl

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
