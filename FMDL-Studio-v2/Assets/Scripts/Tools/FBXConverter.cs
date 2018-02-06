using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static class FBXConverter
{
    static int videoId = 600000000;
    static int textureId = 700000000;
    static int materialId = 800000000;
    static int geometryId = 900000000;
    static int modelId = 1000000001;

    static List<Tuple<int, GameObject>> objects = new List<Tuple<int, GameObject>>(0);
    static List<Tuple<int, int>> objectConnections = new List<Tuple<int, int>>(0);

    static List<int> nodes = new List<int>(0);
    static List<Tuple<int, Transform>> bones = new List<Tuple<int, Transform>>(0);
    static List<Tuple<int, int>> nodesToBones = new List<Tuple<int, int>>(0);
    static List<Tuple<int, int>> boneConnections = new List<Tuple<int, int>>(0);

    static List<int> geometry = new List<int>(0);
    static List<Tuple<int, SkinnedMeshRenderer>> meshes = new List<Tuple<int, SkinnedMeshRenderer>>(0);
    static List<Tuple<int, int>> geometryToMeshes = new List<Tuple<int, int>>(0);
    static List<int> deformers = new List<int>(0);
    static List<Tuple<int, int>> deformersToGeometry = new List<Tuple<int, int>>(0);
    static List<Tuple<int, int>> subDeformersToDeformers = new List<Tuple<int, int>>(0);
    static List<Tuple<int, int>> bonesToSubDeformers = new List<Tuple<int, int>>(0);

    static List<Tuple<int, Material>> materials = new List<Tuple<int, Material>>(0);
    static List<Tuple<int, int>> materialsToMeshes = new List<Tuple<int, int>>(0);
    static List<int> videos = new List<int>(0);
    static List<Tuple<int, Texture>> textures = new List<Tuple<int, Texture>>(0);
    static List<Tuple<int, int>> videosToTextures = new List<Tuple<int, int>>(0);
    static List<Tuple<int, int, string>> texturesToMaterials = new List<Tuple<int, int, string>>(0);

    public static void ConvertToFBX(GameObject gameObject, string filePath)
    {
        Globals.ReadTexturePath();
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

        Clear();

        int numModelObjects = 1;
        StringBuilder fbx = new StringBuilder();
        StringBuilder header = new StringBuilder();

        GetNumObjects(gameObject.transform, ref numModelObjects);

        objects.Add(new Tuple<int, GameObject>(1000000000, gameObject));

        GetObjects(gameObject.transform);

        //Object Properties
        fbx.Append("\n; Object properties");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nObjects:  {");

        //Bone Nodes
        for (int i = 0; i < bones.Count; i++)
        {
            fbx.AppendFormat("\n\tNodeAttribute: {0}, \"NodeAttribute::\", \"LimbNode\" {{", nodesToBones[i].Item1);
            fbx.Append("\n\t\tTypeFlags: \"Skeleton\"");
            fbx.Append("\n\t}");
        } //for

        //Meshes
        for (int i = 0; i < meshes.Count; i++)
        {
            fbx.AppendFormat("\n\tGeometry: {0}, \"Geometry::Scene\", \"Mesh\" {{", geometryToMeshes[i].Item1);

            //Vertex Positions
            fbx.AppendFormat("\n\t\tVertices: *{0} {{", meshes[i].Item2.sharedMesh.vertices.Length * 3);
            fbx.Append("\n\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.vertices.Length; j++)
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.vertices[j].x, meshes[i].Item2.sharedMesh.vertices[j].y, meshes[i].Item2.sharedMesh.vertices[j].z);
            fbx.Length--;
            fbx.Append("\n\t\t}");

            //Faces
            fbx.AppendFormat("\n\t\tPolygonVertexIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], (-meshes[i].Item2.sharedMesh.triangles[j * 3 + 1] - 1));
            fbx.Length--;
            fbx.Append("\n\t\t}");

            //Normals
            fbx.Append("\n\t\tGeometryVersion: 124");
            fbx.Append("\n\t\tLayerElementNormal: 0 {");
            fbx.Append("\n\t\t\tVersion: 102");
            fbx.Append("\n\t\t\tName: \"Normals\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"Direct\"");
            fbx.AppendFormat("\n\t\t\tNormals: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length * 3);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
            {
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]].x, meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]].y, meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]].z);
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].x, meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].y, meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].z);
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].x, meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].y, meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].z);
            } //for
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tNormalsW: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length; j++)
                fbx.Append("1,");
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //Binormals
            fbx.Append("\n\t\tLayerElementBinormal: 0 {");
            fbx.Append("\n\t\t\tVersion: 102");
            fbx.Append("\n\t\t\tName: \"Binormals\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"Direct\"");
            fbx.AppendFormat("\n\t\t\tBinormals: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length * 3);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
            {
                Vector3 binormal0 = Vector3.Cross(meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3]], new Vector3(meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].x, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].y, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].z));
                Vector3 binormal1 = Vector3.Cross(meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]], new Vector3(meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].x, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].y, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].z));
                Vector3 binormal2 = Vector3.Cross(meshes[i].Item2.sharedMesh.normals[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]], new Vector3(meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].x, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].y, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].z));

                fbx.AppendFormat("{0},{1},{2},", binormal0.x, binormal0.y, -binormal0.z);
                fbx.AppendFormat("{0},{1},{2},", binormal1.x, binormal1.y, -binormal1.z);
                fbx.AppendFormat("{0},{1},{2},", binormal2.x, binormal2.y, -binormal2.z);
            } //for
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tBinormalsW: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length; j++)
                fbx.Append("1,");
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //Tangents
            fbx.Append("\n\t\tLayerElementTangent: 0 {");
            fbx.Append("\n\t\t\tVersion: 102");
            fbx.Append("\n\t\t\tName: \"Tangents\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"Direct\"");
            fbx.AppendFormat("\n\t\t\tTangents: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length * 3);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
            {
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].x, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].y, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].z);
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].x, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].y, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].z);
                fbx.AppendFormat("{0},{1},{2},", -meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].x, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].y, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].z);
            } //for
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tTangentsW: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3]].w, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 2]].w, meshes[i].Item2.sharedMesh.tangents[meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]].w);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //Colours
            fbx.Append("\n\t\tLayerElementColor: 0 {");
            fbx.Append("\n\t\t\tVersion: 101");
            fbx.Append("\n\t\t\tName: \"VertexColors\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
            fbx.AppendFormat("\n\t\t\tColorIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //UV Set 0
            fbx.Append("\n\t\tLayerElementUV: 0 {");
            fbx.Append("\n\t\t\tVersion: 101");
            fbx.Append("\n\t\t\tName: \"UVSet0\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
            fbx.AppendFormat("\n\t\t\tUV: *{0} {{", meshes[i].Item2.sharedMesh.uv.Length * 2);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.uv.Length; j++)
                fbx.AppendFormat("{0},{1},", meshes[i].Item2.sharedMesh.uv[j].x, meshes[i].Item2.sharedMesh.uv[j].y);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
            fbx.Append("\n\t\t\t\ta: ");
            for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
            fbx.Length--;
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //UV Set 1
            if (!meshes[i].Item2.sharedMesh.uv2.IsNullOrZeroes())
            {
                fbx.Append("\n\t\tLayerElementUV: 1 {");
                fbx.Append("\n\t\t\tVersion: 101");
                fbx.Append("\n\t\t\tName: \"UVSet1\"");
                fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                fbx.AppendFormat("\n\t\t\tUV: *{0} {{", meshes[i].Item2.sharedMesh.uv2.Length * 2);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < meshes[i].Item2.sharedMesh.uv2.Length; j++)
                    fbx.AppendFormat("{0},{1},", meshes[i].Item2.sharedMesh.uv2[j].x, -meshes[i].Item2.sharedMesh.uv2[j].y);
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                    fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.Append("\n\t\t}");

                //UV Set 2
                if (!meshes[i].Item2.sharedMesh.uv3.IsNullOrZeroes())
                {
                    fbx.Append("\n\t\tLayerElementUV: 2 {");
                    fbx.Append("\n\t\t\tVersion: 101");
                    fbx.Append("\n\t\t\tName: \"UVSet2\"");
                    fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                    fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                    fbx.AppendFormat("\n\t\t\tUV: *{0} {{", meshes[i].Item2.sharedMesh.uv3.Length * 2);
                    fbx.Append("\n\t\t\t\ta: ");
                    for (int j = 0; j < meshes[i].Item2.sharedMesh.uv3.Length; j++)
                        fbx.AppendFormat("{0},{1},", meshes[i].Item2.sharedMesh.uv3[j].x, -meshes[i].Item2.sharedMesh.uv3[j].y);
                    fbx.Length--;
                    fbx.Append("\n\t\t\t}");
                    fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
                    fbx.Append("\n\t\t\t\ta: ");
                    for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                        fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
                    fbx.Length--;
                    fbx.Append("\n\t\t\t}");
                    fbx.Append("\n\t\t}");

                    //UV Set 3
                    if (!meshes[i].Item2.sharedMesh.uv4.IsNullOrZeroes())
                    {
                        fbx.Append("\n\t\tLayerElementUV: 3 {");
                        fbx.Append("\n\t\t\tVersion: 101");
                        fbx.Append("\n\t\t\tName: \"UVSet3\"");
                        fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                        fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                        fbx.AppendFormat("\n\t\t\tUV: *{0} {{", meshes[i].Item2.sharedMesh.uv4.Length * 2);
                        fbx.Append("\n\t\t\t\ta: ");
                        for (int j = 0; j < meshes[i].Item2.sharedMesh.uv4.Length; j++)
                            fbx.AppendFormat("{0},{1},", meshes[i].Item2.sharedMesh.uv4[j].x, -meshes[i].Item2.sharedMesh.uv4[j].y);
                        fbx.Length--;
                        fbx.Append("\n\t\t\t}");
                        fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", meshes[i].Item2.sharedMesh.triangles.Length);
                        fbx.Append("\n\t\t\t\ta: ");
                        for (int j = 0; j < meshes[i].Item2.sharedMesh.triangles.Length / 3; j++)
                            fbx.AppendFormat("{0},{1},{2},", meshes[i].Item2.sharedMesh.triangles[j * 3], meshes[i].Item2.sharedMesh.triangles[j * 3 + 2], meshes[i].Item2.sharedMesh.triangles[j * 3 + 1]);
                        fbx.Length--;
                        fbx.Append("\n\t\t\t}");
                        fbx.Append("\n\t\t}");
                    } //if
                } //if
            } //if

            //Material
            fbx.Append("\n\t\tLayerElementMaterial: 0 {");
            fbx.Append("\n\t\t\tVersion: 101");
            fbx.Append("\n\t\t\tName: \"Material\"");
            fbx.Append("\n\t\t\tMappingInformationType: \"AllSame\"");
            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
            fbx.Append("\n\t\t\tMaterials: *1 {");
            fbx.Append("\n\t\t\t\ta: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //Layer 0
            fbx.Append("\n\t\tLayer: 0 {");
            fbx.Append("\n\t\t\tVersion: 100");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementNormal\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementBinormal\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementTangent\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementMaterial\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementColor\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t\tLayerElement:  {");
            fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
            fbx.Append("\n\t\t\t\tTypedIndex: 0");
            fbx.Append("\n\t\t\t}");
            fbx.Append("\n\t\t}");

            //Layer 1
            if (!meshes[i].Item2.sharedMesh.uv2.IsNullOrZeroes())
            {
                fbx.Append("\n\t\tLayer: 1 {");
                fbx.Append("\n\t\t\tVersion: 100");
                fbx.Append("\n\t\t\tLayerElement:  {");
                fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
                fbx.Append("\n\t\t\t\tTypedIndex: 1");
                fbx.Append("\n\t\t\t}");
                fbx.Append("\n\t\t}");

                //Layer 2
                if (!meshes[i].Item2.sharedMesh.uv3.IsNullOrZeroes())
                {
                    fbx.Append("\n\t\tLayer: 2 {");
                    fbx.Append("\n\t\t\tVersion: 100");
                    fbx.Append("\n\t\t\tLayerElement:  {");
                    fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
                    fbx.Append("\n\t\t\t\tTypedIndex: 2");
                    fbx.Append("\n\t\t\t}");
                    fbx.Append("\n\t\t}");

                    //Layer 3
                    if (!meshes[i].Item2.sharedMesh.uv4.IsNullOrZeroes())
                    {
                        fbx.Append("\n\t\tLayer: 3 {");
                        fbx.Append("\n\t\t\tVersion: 100");
                        fbx.Append("\n\t\t\tLayerElement:  {");
                        fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
                        fbx.Append("\n\t\t\t\tTypedIndex: 3");
                        fbx.Append("\n\t\t\t}");
                        fbx.Append("\n\t\t}");
                    } //if
                } //if
            } //if
            fbx.Append("\n\t}");
        } //for

        //General Objects Names/Positions
        for (int i = 0; i < objects.Count; i++)
        {
            fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"Null\" {{", objects[i].Item1, objects[i].Item2.name);
            fbx.Append("\n\t\tVersion: 232");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            if (objects[i].Item2.transform.localPosition.x != 0 || objects[i].Item2.transform.localPosition.y != 0 || objects[i].Item2.transform.localPosition.z != 0)
                fbx.AppendFormat("\n\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",{0},{1},{2}", -objects[i].Item2.transform.localPosition.x, objects[i].Item2.transform.localPosition.y, objects[i].Item2.transform.localPosition.z);
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tShading: Y");
            fbx.Append("\n\t\tCulling: \"CullingOff\"");
            fbx.Append("\n\t}");
        } //for

        //Bones Names/Positions
        for (int i = 0; i < bones.Count; i++)
        {
            fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"LimbNode\" {{", bones[i].Item1, bones[i].Item2.gameObject.name);
            fbx.Append("\n\t\tVersion: 232");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",1");
            fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
            if (bones[i].Item2.transform.localPosition.x != 0 || bones[i].Item2.transform.localPosition.y != 0 || bones[i].Item2.transform.localPosition.z != 0)
                fbx.AppendFormat("\n\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",{0},{1},{2}", -bones[i].Item2.transform.localPosition.x, bones[i].Item2.transform.localPosition.y, bones[i].Item2.transform.localPosition.z);
            fbx.AppendFormat("\n\t\t\tP: \"MaxHandle\", \"int\", \"Integer\", \"UH\",{0}", i + 1);
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tShading: T");
            fbx.Append("\n\t\tCulling: \"CullingOff\"");
            fbx.Append("\n\t}");
        } //for

        //Mesh Names
        for (int i = 0; i < meshes.Count; i++)
        {
            fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"Mesh\" {{", meshes[i].Item1, meshes[i].Item2.name);
            fbx.Append("\n\t\tVersion: 232");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tShading: W");
            fbx.Append("\n\t\tCulling: \"CullingOff\"");
            fbx.Append("\n\t}");
        } //for

        //BindPose/Nodes
        fbx.Append("\n\tPose: 2222222222, \"Pose::BIND_POSES\", \"BindPose\" {");
        fbx.Append("\n\t\tType: \"BindPose\"");
        fbx.Append("\n\t\tVersion: 100");
        fbx.AppendFormat("\n\t\tNbPoseNodes: {0}", geometry.Count);
        for (int i = 0; i < geometry.Count; i++)
        {
            fbx.Append("\n\t\tPoseNode:  {");
            fbx.AppendFormat("\n\t\t\tNode: {0}", geometry[i]);
            fbx.Append("\n\t\t}");
        } //for
        fbx.Append("\n\t}");

        //Materials
        for (int i = 0; i < materials.Count; i++)
        {
            fbx.AppendFormat("\n\tMaterial: {0}, \"Material::{1}\", \"\" {{", materials[i].Item1, materials[i].Item2);
            fbx.Append("\n\t\tVersion: 102");
            fbx.Append("\n\t\tShadingModel: \"lambert\"");
            fbx.Append("\n\t\tMultiLayer: 0");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0,0,0");
            fbx.Append("\n\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",1,1,1");
            fbx.Append("\n\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",0");
            fbx.Append("\n\t\t\tP: \"Emissive\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"Ambient\", \"Vector3D\", \"Vector\", \"\",0,0,0");
            fbx.Append("\n\t\t\tP: \"Diffuse\", \"Vector3D\", \"Vector\", \"\",1,1,1");
            fbx.Append("\n\t\t\tP: \"Opacity\", \"double\", \"Number\", \"\",1");
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t}");
        } //for

        //Deformers
        for (int i = 0; i < deformers.Count; i++)
        {
            HashSet<int> usedBones = GetUsedBones(meshes[i].Item2);

            fbx.AppendFormat("\n\tDeformer: {0}, \"Deformer::\", \"Skin\" {{", deformers[i]);
            fbx.Append("\n\t\tVersion: 100");
            fbx.Append("\n\t\tLink_DeformAcuracy: 50");
            fbx.Append("\n\t}");

            foreach (int j in usedBones)
            {
                List<int> indices = new List<int>(0);
                List<float> weights = new List<float>(0);

                subDeformersToDeformers.Add(new Tuple<int, int>(modelId, deformers[i]));
                bonesToSubDeformers.Add(new Tuple<int, int>(bones[j].Item1, modelId));

                for (int h = 0; h < meshes[i].Item2.sharedMesh.boneWeights.Length; h++)
                {
                    if (meshes[i].Item2.sharedMesh.boneWeights[h].boneIndex0 == j)
                    {
                        indices.Add(h);
                        weights.Add(meshes[i].Item2.sharedMesh.boneWeights[h].weight0);
                    } //if
                    else if (meshes[i].Item2.sharedMesh.boneWeights[h].boneIndex1 == j)
                    {
                        indices.Add(h);
                        weights.Add(meshes[i].Item2.sharedMesh.boneWeights[h].weight1);
                    } //if
                    else if (meshes[i].Item2.sharedMesh.boneWeights[h].boneIndex2 == j)
                    {
                        indices.Add(h);
                        weights.Add(meshes[i].Item2.sharedMesh.boneWeights[h].weight2);
                    } //if
                    else if (meshes[i].Item2.sharedMesh.boneWeights[h].boneIndex3 == j)
                    {
                        indices.Add(h);
                        weights.Add(meshes[i].Item2.sharedMesh.boneWeights[h].weight3);
                    } //if
                } //for ends

                fbx.AppendFormat("\n\tDeformer: {0}, \"SubDeformer::\", \"Cluster\" {{", modelId);
                fbx.Append("\n\t\tVersion: 100");
                fbx.Append("\n\t\tUserData: \"\", \"\"");
                fbx.AppendFormat("\n\t\tIndexes: *{0} {{", indices.Count);
                fbx.Append("\n\t\t\ta: ");
                for (int h = 0; h < indices.Count; h++)
                    fbx.AppendFormat("{0},", indices[h]);
                fbx.Length--;
                fbx.Append("\n\t\t}");
                fbx.AppendFormat("\n\t\tWeights: *{0} {{", weights.Count);
                fbx.Append("\n\t\t\ta: ");
                for (int h = 0; h < weights.Count; h++)
                    fbx.AppendFormat("{0},", weights[h]);
                fbx.Length--;
                fbx.Append("\n\t\t}");
                fbx.Append("\n\t\tTransform: *16 {");
                fbx.Append("\n\t\t\ta: ");
                fbx.AppendFormat("{0},{1},{2},{3},", meshes[i].Item2.sharedMesh.bindposes[j][0, 0], -meshes[i].Item2.sharedMesh.bindposes[j][1, 0], -meshes[i].Item2.sharedMesh.bindposes[j][2, 0], meshes[i].Item2.sharedMesh.bindposes[j][3, 0]);
                fbx.AppendFormat("{0},{1},{2},{3},", -meshes[i].Item2.sharedMesh.bindposes[j][0, 1], meshes[i].Item2.sharedMesh.bindposes[j][1, 1], meshes[i].Item2.sharedMesh.bindposes[j][2, 1], meshes[i].Item2.sharedMesh.bindposes[j][3, 1]);
                fbx.AppendFormat("{0},{1},{2},{3},", -meshes[i].Item2.sharedMesh.bindposes[j][0, 2], meshes[i].Item2.sharedMesh.bindposes[j][1, 2], meshes[i].Item2.sharedMesh.bindposes[j][2, 2], meshes[i].Item2.sharedMesh.bindposes[j][3, 2]);
                fbx.AppendFormat("{0},{1},{2},{3}", -meshes[i].Item2.sharedMesh.bindposes[j][0, 3], meshes[i].Item2.sharedMesh.bindposes[j][1, 3], meshes[i].Item2.sharedMesh.bindposes[j][2, 3], meshes[i].Item2.sharedMesh.bindposes[j][3, 3]);
                fbx.Append("\n\t\t}");
                fbx.Append("\n\t}");
                modelId++;
            } //foreach
        } //for

        //Texture Videos
        for (int i = 0; i < videos.Count; i++)
        {
            fbx.AppendFormat("\n\tVideo: {0}, \"Video::{1}\", \"Clip\" {{", videos[i], textures[i].Item2.name);
            fbx.Append("\n\t\tType: \"Clip\"");
            fbx.Append("\n\t\tProperties70:  {");
            fbx.AppendFormat("\n\t\t\tP: \"Path\", \"KString\", \"XRefUrl\", \"\", \"{0}\"", Globals.texturePath + "\\" + textures[i].Item2.name);
            fbx.Append("\n\t\t}");
            fbx.Append("\n\t\tUseMipMap: 0");
            fbx.AppendFormat("\n\t\tFilename: \"{0}\"", Globals.texturePath + "\\" + textures[i].Item2.name);
            //fbx.AppendFormat("\n\t\tRelativeFilename: \"{0}\"", gameObject.name + "\\" + textures[i].Item2.name);
            fbx.Append("\n\t}");
        } //for

        //Textures
        for (int i = 0; i < textures.Count; i++)
        {
            fbx.AppendFormat("\n\tTexture: {0}, \"Texture::{1}\", \"\" {{", textures[i].Item1, textures[i].Item2.name);
            fbx.Append("\n\t\tType: \"TextureVideoClip\"");
            fbx.Append("\n\t\tVersion: 202");
            fbx.AppendFormat("\n\t\tTextureName: \"Texture::{0}\"", textures[i].Item2.name);
            fbx.Append("\n\t\tProperties70:  {");
            fbx.Append("\n\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"UVSet0\"");
            fbx.Append("\n\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",1");
            fbx.Append("\n\t\t}");
            fbx.AppendFormat("\n\t\tMedia: \"Video::{0}\"", textures[i].Item2.name);
            fbx.AppendFormat("\n\t\tFileName: \"{0}\"", Globals.texturePath + "\\" + textures[i].Item2.name);
            //fbx.AppendFormat("\n\t\tRelativeFilename: \"{0}\"", gameObject.name + "\\" + textures[i].Item2.name);
            fbx.Append("\n\t\tModelUVTranslation: 0,0");
            fbx.Append("\n\t\tModelUVScaling: 1,1");
            fbx.Append("\n\t\tTexture_Alpha_Source: \"Alpha_Black\"");
            fbx.Append("\n\t\tCropping: 0,0,0,0");
            fbx.Append("\n\t}");
        } //for
        fbx.Append("\n}");

        //Object Connections
        fbx.Append("\n\n; Object connections");
        fbx.Append("\n;------------------------------------------------------------------");
        fbx.Append("\n\nConnections:  {");

        fbx.AppendFormat("\n\n\t;Model::{0}, Model::RootNode", objects[0].Item2.name);
        fbx.AppendFormat("\n\tC: \"OO\",{0},0", objects[0].Item1);

        //General Object Connections
        for (int i = 0; i < objectConnections.Count; i++)
        {
            string name1 = "";
            string name2 = objects.Find(x => x.Item1 == objectConnections[i].Item2).Item2.name;

            if (objects.Find(x => x.Item1 == objectConnections[i].Item1) != null)
                name1 = objects.Find(x => x.Item1 == objectConnections[i].Item1).Item2.name;
            else if (meshes.Find(x => x.Item1 == objectConnections[i].Item1) != null)
                name1 = meshes.Find(x => x.Item1 == objectConnections[i].Item1).Item2.name;
            else if (i < bones.Count)
                if (meshes.Find(x => x.Item1 == bones[i].Item1) != null)
                    name1 = bones.Find(x => x.Item1 == objectConnections[i].Item1).Item2.gameObject.name;

            fbx.AppendFormat("\n\n\t;Model::{0}, Model::{1}", name1, name2);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", objectConnections[i].Item1, objectConnections[i].Item2);
        } //for

        //Bone Node to Bone Connections
        for (int i = 0; i < nodesToBones.Count; i++)
        {
            string name1 = bones.Find(x => x.Item1 == nodesToBones[i].Item2).Item2.gameObject.name;

            fbx.AppendFormat("\n\n\t;NodeAttribute::, Model::{0}", name1);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", nodesToBones[i].Item1, nodesToBones[i].Item2);
        } //for

        //Bone to Bone Connections
        for (int i = 0; i < boneConnections.Count; i++)
        {
            string name1 = bones.Find(x => x.Item1 == boneConnections[i].Item1).Item2.gameObject.name;
            string name2 = bones.Find(x => x.Item1 == boneConnections[i].Item2).Item2.gameObject.name;

            fbx.AppendFormat("\n\n\t;Model::{0}, Model::{1}", name1, name2);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", boneConnections[i].Item1, boneConnections[i].Item2);
        } //for

        //Material to Mesh Connections
        for (int i = 0; i < materialsToMeshes.Count; i++)
        {
            string name1 = materials.Find(x => x.Item1 == materialsToMeshes[i].Item1).Item2.name;
            string name2 = meshes.Find(x => x.Item1 == materialsToMeshes[i].Item2).Item2.name;

            fbx.AppendFormat("\n\n\t;Material::{0}, Model::{1}", name1, name2);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", materialsToMeshes[i].Item1, materialsToMeshes[i].Item2);
        } //for

        //Geometry to Mesh Connections
        for (int i = 0; i < geometryToMeshes.Count; i++)
        {
            string name1 = meshes.Find(x => x.Item1 == geometryToMeshes[i].Item2).Item2.name;

            fbx.AppendFormat("\n\n\t;Geometry::Scene, Model::{0}", name1);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", geometryToMeshes[i].Item1, geometryToMeshes[i].Item2);
        } //for

        //Deformer to Geometry Connections
        for (int i = 0; i < deformersToGeometry.Count; i++)
        {
            fbx.Append("\n\n\t;Deformer::, Geometry::Scene");
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", deformersToGeometry[i].Item1, deformersToGeometry[i].Item2);
        } //for

        //SubDeformer to Deformer Connections
        for (int i = 0; i < subDeformersToDeformers.Count; i++)
        {
            fbx.Append("\n\n\t;SubDeformer::, Deformer::");
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", subDeformersToDeformers[i].Item1, subDeformersToDeformers[i].Item2);
        } //for

        //Bone to SubDeformer Connections
        for (int i = 0; i < bonesToSubDeformers.Count; i++)
        {
            string name1 = bones.Find(x => x.Item1 == bonesToSubDeformers[i].Item1).Item2.gameObject.name;

            fbx.AppendFormat("\n\n\t;Model::{0}, SubDeformer::", name1);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", bonesToSubDeformers[i].Item1, bonesToSubDeformers[i].Item2);
        } //for

        //Texture to Material Connections
        for (int i = 0; i < texturesToMaterials.Count; i++)
        {
            string name1 = textures.Find(x => x.Item1 == texturesToMaterials[i].Item1).Item2.name;
            string name2 = materials.Find(x => x.Item1 == texturesToMaterials[i].Item2).Item2.name;

            fbx.AppendFormat("\n\n\t;Texture::{0}, Material::{1}", name1, name2);
            fbx.AppendFormat("\n\tC: \"OP\",{0},{1}, \"{2}\"", texturesToMaterials[i].Item1, texturesToMaterials[i].Item2, texturesToMaterials[i].Item3);
        } //for

        //Texture Video to Texture Connections
        for (int i = 0; i < videosToTextures.Count; i++)
        {
            string name1 = textures.Find(x => x.Item1 == videosToTextures[i].Item2).Item2.name;

            fbx.AppendFormat("\n\n\t;Video::{0}, Texture::{0}", name1);
            fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", videosToTextures[i].Item1, videosToTextures[i].Item2);
        } //for

        //Takes
        fbx.Append("\n}");
        fbx.Append("\n;Takes section");
        fbx.Append("\n;----------------------------------------------------");
        fbx.Append("\n\nTakes:  {");
        fbx.Append("\n\tCurrent: \"\"");
        fbx.Append("\n}");

        //Header
        header.Append("; FBX 7.4.0 project file");
        header.Append("\n; Copyright (C) 1997-2015 Autodesk Inc. and/or its licensors.");
        header.Append("\n; All rights reserved.");
        header.Append("\n; ----------------------------------------------------");
        header.Append("\n\nFBXHeaderExtension:  {");
        header.Append("\n\tFBXHeaderVersion: 1003");
        header.Append("\n\tFBXVersion: 7400");
        header.Append("\n\tCreationTimeStamp:  {");
        header.Append("\n\t\tVersion: 1000");
        header.AppendFormat("\n\t\tYear: {0}", DateTime.Now.Year);
        header.AppendFormat("\n\t\tMonth: {0}", DateTime.Now.Month);
        header.AppendFormat("\n\t\tDay: {0}", DateTime.Now.Day);
        header.AppendFormat("\n\t\tHour: {0}", DateTime.Now.Hour);
        header.AppendFormat("\n\t\tMinute: {0}", DateTime.Now.Minute);
        header.AppendFormat("\n\t\tSecond: {0}", DateTime.Now.Second);
        header.AppendFormat("\n\t\tMillisecond: {0}", DateTime.Now.Millisecond);
        header.Append("\n\t}");
        header.Append("\n\tCreator: \"Fmdl Studio v2\"");
        header.Append("\n\tSceneInfo: \"SceneInfo::GlobalInfo\", \"UserData\" {");
        header.Append("\n\t\tType: \"UserData\"");
        header.Append("\n\t\tVersion: 100");
        header.Append("\n\t\tMetaData:  {");
        header.Append("\n\t\t\tVersion: 100");
        header.Append("\n\t\t\tTitle: \"Unity to FBX converter\"");
        header.Append("\n\t\t\tSubject: \"\"");
        header.Append("\n\t\t\tAuthor: \"BobDoleOwndU and Joey35233\"");
        header.Append("\n\t\t\tKeywords: \"export fmdl fbx unity\"");
        header.Append("\n\t\t\tRevision: \"0.1\"");
        header.Append("\n\t\t\tComment: \"This is an unofficial exporter. Results may vary.\"");
        header.Append("\n\t\t}");
        header.Append("\n\t\tProperties70:  {");
        header.AppendFormat("\n\t\t\tP: \"DocumentUrl\", \"KString\", \"Url\", \"\", \"{0}\"", filePath);
        header.AppendFormat("\n\t\t\tP: \"SrcDocumentUrl\", \"KString\", \"Url\", \"\", \"{0}\"", filePath);
        header.Append("\n\t\t\tP: \"Original\", \"Compound\", \"\", \"\"");
        header.Append("\n\t\t\tP: \"Original|ApplicationVendor\", \"KString\", \"\", \"\", \"BobDoleOwndU and Joey35233\"");
        header.Append("\n\t\t\tP: \"Original|ApplicationName\", \"KString\", \"\", \"\", \"Fmdl Studio v2\"");
        header.Append("\n\t\t\tP: \"Original|ApplicationVersion\", \"KString\", \"\", \"\", \"0.1\"");
        header.Append("\n\t\t\tP: \"Original|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
        header.Append("\n\t\t\tP: \"Original|FileName\", \"KString\", \"\", \"\", \"\"");
        header.Append("\n\t\t\tP: \"LastSaved\", \"Compound\", \"\", \"\"");
        header.Append("\n\t\t\tP: \"LastSaved|ApplicationVendor\", \"KString\", \"\", \"\", \"BobDoleOwndU and Joey35233\"");
        header.Append("\n\t\t\tP: \"LastSaved|ApplicationName\", \"KString\", \"\", \"\", \"Fmdl Studio v2\"");
        header.Append("\n\t\t\tP: \"LastSaved|ApplicationVersion\", \"KString\", \"\", \"\", \"0.1\"");
        header.Append("\n\t\t\tP: \"LastSaved|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
        header.Append("\n\t\t}");
        header.Append("\n\t}");
        header.Append("\n}");

        //Global Settings
        header.Append("\nGlobalSettings:  {");
        header.Append("\n\tVersion: 1000");
        header.Append("\n\tProperties70:  {");
        header.Append("\n\t\tP: \"UpAxis\", \"int\", \"Integer\", \"\",1");
        header.Append("\n\t\tP: \"UpAxisSign\", \"int\", \"Integer\", \"\",1");
        header.Append("\n\t\tP: \"FrontAxis\", \"int\", \"Integer\", \"\",2");
        header.Append("\n\t\tP: \"FrontAxisSign\", \"int\", \"Integer\", \"\",1");
        header.Append("\n\t\tP: \"CoordAxis\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\tP: \"CoordAxisSign\", \"int\", \"Integer\", \"\",1");
        header.Append("\n\t\tP: \"OriginalUpAxis\", \"int\", \"Integer\", \"\",-1");
        header.Append("\n\t\tP: \"OriginalUpAxisSign\", \"int\", \"Integer\", \"\",1");
        header.Append("\n\t\tP: \"UnitScaleFactor\", \"double\", \"Number\", \"\",100");
        header.Append("\n\t\tP: \"OriginalUnitScaleFactor\", \"double\", \"Number\", \"\",100");
        header.Append("\n\t\tP: \"AmbientColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
        header.Append("\n\t\tP: \"DefaultCamera\", \"KString\", \"\", \"\", \"Producer Perspective\"");
        header.Append("\n\t\tP: \"TimeMode\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\tP: \"TimeProtocol\", \"enum\", \"\", \"\",2");
        header.Append("\n\t\tP: \"SnapOnFrameMode\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\tP: \"TimeSpanStart\", \"KTime\", \"Time\", \"\",0");
        header.Append("\n\t\tP: \"TimeSpanStop\", \"KTime\", \"Time\", \"\",46186158000");
        header.Append("\n\t\tP: \"CustomFrameRate\", \"double\", \"Number\", \"\",-1");
        header.Append("\n\t\tP: \"TimeMarker\", \"Compound\", \"\", \"\"");
        header.Append("\n\t\tP: \"CurrentTimeMarker\", \"int\", \"Integer\", \"\",-1");
        header.Append("\n\t}");
        header.Append("\n}");

        //Documents Description
        header.Append("\n\n; Documents Description");
        header.Append("\n;------------------------------------------------------------------");
        header.Append("\n\nDocuments:  {");
        header.Append("\n\tCount: 1");
        header.Append("\n\tDocument: 9999999999, \"Scene\", \"Scene\" {");
        header.Append("\n\t\tProperties70:  {");
        header.Append("\n\t\t\tP: \"SourceObject\", \"object\", \"\", \"\"");
        header.Append("\n\t\t\tP: \"ActiveAnimStackName\", \"KString\", \"\", \"\", \"\"");
        header.Append("\n\t\t}");
        header.Append("\n\t\tRootNode: 0");
        header.Append("\n\t}");
        header.Append("\n}");

        //Document References
        header.Append("\n\n; Document References");
        header.Append("\n;------------------------------------------------------------------");
        header.Append("\n\nReferences:  {");
        header.Append("\n}");

        //Object Definitions
        header.Append("\n\n; Object definitions");
        header.Append("\n;------------------------------------------------------------------");
        header.Append("\n\nDefinitions:  {");
        header.Append("\n\tVersion: 100");
        header.AppendFormat("\n\tCount: {0}", numModelObjects + meshes.Count + bones.Count + materials.Count + nodes.Count + deformers.Count + subDeformersToDeformers.Count + videos.Count + textures.Count + 2); //+ 2 is for global settings and pose.

        header.Append("\n\tObjectType: \"GlobalSettings\" {");
        header.Append("\n\t\tCount: 1");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"Model\" {");
        header.AppendFormat("\n\t\tCount: {0}", numModelObjects);
        header.Append("\n\t\tPropertyTemplate: \"FbxNode\" {");
        header.Append("\n\t\t\tProperties70:  {");
        header.Append("\n\t\t\t\tP: \"QuaternionInterpolate\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"RotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"ScalingOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"ScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"TranslationActive\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"TranslationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"TranslationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"TranslationMinX\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"TranslationMinY\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"TranslationMinZ\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"TranslationMaxX\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"TranslationMaxY\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"TranslationMaxZ\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationSpaceForLimitOnly\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationStiffnessX\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationStiffnessY\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationStiffnessZ\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"AxisLen\", \"double\", \"Number\", \"\",10");
        header.Append("\n\t\t\t\tP: \"PreRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"PostRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"RotationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"RotationMinX\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationMinY\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationMinZ\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationMaxX\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationMaxY\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"RotationMaxZ\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingActive\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",1,1,1");
        header.Append("\n\t\t\t\tP: \"ScalingMinX\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingMinY\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingMinZ\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingMaxX\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingMaxY\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ScalingMaxZ\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"GeometricTranslation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"GeometricRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"GeometricScaling\", \"Vector3D\", \"Vector\", \"\",1,1,1");
        header.Append("\n\t\t\t\tP: \"MinDampRangeX\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MinDampRangeY\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MinDampRangeZ\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MaxDampRangeX\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MaxDampRangeY\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MaxDampRangeZ\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MinDampStrengthX\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MinDampStrengthY\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MinDampStrengthZ\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MaxDampStrengthX\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MaxDampStrengthY\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"MaxDampStrengthZ\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"PreferedAngleX\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"PreferedAngleY\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"PreferedAngleZ\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"LookAtProperty\", \"object\", \"\", \"\"");
        header.Append("\n\t\t\t\tP: \"UpVectorProperty\", \"object\", \"\", \"\"");
        header.Append("\n\t\t\t\tP: \"Show\", \"bool\", \"\", \"\",1");
        header.Append("\n\t\t\t\tP: \"NegativePercentShapeSupport\", \"bool\", \"\", \"\",1");
        header.Append("\n\t\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",-1");
        header.Append("\n\t\t\t\tP: \"Freeze\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"LODBox\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",0,0,0");
        header.Append("\n\t\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A\",0,0,0");
        header.Append("\n\t\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",1,1,1");
        header.Append("\n\t\t\t\tP: \"Visibility\", \"Visibility\", \"\", \"A\",1");
        header.Append("\n\t\t\t\tP: \"Visibility Inheritance\", \"Visibility Inheritance\", \"\", \"\",1");
        header.Append("\n\t\t\t}");
        header.Append("\n\t\t}");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"NodeAttribute\" {");
        header.AppendFormat("\n\t\tCount: {0}", bones.Count);
        header.Append("\n\t\tPropertyTemplate: \"FbxSkeleton\" {");
        header.Append("\n\t\t\tProperties70:  {");
        header.Append("\n\t\t\t\tP: \"Color\", \"ColorRGB\", \"Color\", \"\",0.8,0.8,0.8");
        header.Append("\n\t\t\t\tP: \"Size\", \"double\", \"Number\", \"\",100");
        header.Append("\n\t\t\t\tP: \"LimbLength\", \"double\", \"Number\", \"\",1");
        header.Append("\n\t\t\t}");
        header.Append("\n\t\t}");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"Geometry\" {");
        header.AppendFormat("\n\t\tCount: {0}", meshes.Count);
        header.Append("\n\t\tPropertyTemplate: \"FbxMesh\" {");
        header.Append("\n\t\t\tProperties70:  {");
        header.Append("\n\t\t\t\tP: \"Color\", \"ColorRGB\", \"Color\", \"\",0.8,0.8,0.8");
        header.Append("\n\t\t\t\tP: \"BBoxMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"BBoxMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"Primary Visibility\", \"bool\", \"\", \"\",1");
        header.Append("\n\t\t\t\tP: \"Casts Shadows\", \"bool\", \"\", \"\",1");
        header.Append("\n\t\t\t\tP: \"Receive Shadows\", \"bool\", \"\", \"\",1");
        header.Append("\n\t\t\t}");
        header.Append("\n\t\t}");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"Material\" {");
        header.AppendFormat("\n\t\tCount: {0}", materials.Count);
        header.Append("\n\t\tPropertyTemplate: \"FbxSurfaceLambert\" {");
        header.Append("\n\t\t\tProperties70:  {");
        header.Append("\n\t\t\t\tP: \"ShadingModel\", \"KString\", \"\", \"\", \"Lambert\"");
        header.Append("\n\t\t\t\tP: \"MultiLayer\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"EmissiveColor\", \"Color\", \"\", \"A\",0,0,0");
        header.Append("\n\t\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",1");
        header.Append("\n\t\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
        header.Append("\n\t\t\t\tP: \"AmbientFactor\", \"Number\", \"\", \"A\",1");
        header.Append("\n\t\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",0.8,0.8,0.8");
        header.Append("\n\t\t\t\tP: \"DiffuseFactor\", \"Number\", \"\", \"A\",1");
        header.Append("\n\t\t\t\tP: \"Bump\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"NormalMap\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",1");
        header.Append("\n\t\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",0,0,0");
        header.Append("\n\t\t\t\tP: \"TransparencyFactor\", \"Number\", \"\", \"A\",0");
        header.Append("\n\t\t\t\tP: \"DisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"DisplacementFactor\", \"double\", \"Number\", \"\",1");
        header.Append("\n\t\t\t\tP: \"VectorDisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"VectorDisplacementFactor\", \"double\", \"Number\", \"\",1");
        header.Append("\n\t\t\t}");
        header.Append("\n\t\t}");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"Deformer\" {");
        header.AppendFormat("\n\t\tCount: {0}", deformers.Count + subDeformersToDeformers.Count);
        header.Append("\n\t}");
        header.Append("\n\tObjectType: \"Pose\" {");
        header.Append("\n\t\tCount: 1");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"Texture\" {");
        header.AppendFormat("\n\t\tCount: {0}", textures.Count);
        header.Append("\n\t\tPropertyTemplate: \"FbxFileTexture\" {");
        header.Append("\n\t\t\tProperties70:  {");
        header.Append("\n\t\t\t\tP: \"TextureTypeUse\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Texture alpha\", \"Number\", \"\", \"A\",1");
        header.Append("\n\t\t\t\tP: \"CurrentMappingType\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"WrapModeU\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"WrapModeV\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"UVSwap\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"PremultiplyAlpha\", \"bool\", \"\", \"\",1");
        header.Append("\n\t\t\t\tP: \"Translation\", \"Vector\", \"\", \"A\",0,0,0");
        header.Append("\n\t\t\t\tP: \"Rotation\", \"Vector\", \"\", \"A\",0,0,0");
        header.Append("\n\t\t\t\tP: \"Scaling\", \"Vector\", \"\", \"A\",1,1,1");
        header.Append("\n\t\t\t\tP: \"TextureRotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"TextureScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
        header.Append("\n\t\t\t\tP: \"CurrentTextureBlendMode\", \"enum\", \"\", \"\",1");
        header.Append("\n\t\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"default\"");
        header.Append("\n\t\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"UseMipMap\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t}");
        header.Append("\n\t\t}");
        header.Append("\n\t}");

        header.Append("\n\tObjectType: \"Video\" {");
        header.AppendFormat("\n\t\tCount: {0}", videos.Count);
        header.Append("\n\t\tPropertyTemplate: \"FbxVideo\" {");
        header.Append("\n\t\t\tProperties70:  {");
        header.Append("\n\t\t\t\tP: \"ImageSequence\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"ImageSequenceOffset\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\t\t\tP: \"FrameRate\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"LastFrame\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Width\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Height\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Path\", \"KString\", \"XRefUrl\", \"\", \"\"");
        header.Append("\n\t\t\t\tP: \"StartFrame\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\t\t\tP: \"StopFrame\", \"int\", \"Integer\", \"\",0");
        header.Append("\n\t\t\t\tP: \"PlaySpeed\", \"double\", \"Number\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Offset\", \"KTime\", \"Time\", \"\",0");
        header.Append("\n\t\t\t\tP: \"InterlaceMode\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"FreeRunning\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"Loop\", \"bool\", \"\", \"\",0");
        header.Append("\n\t\t\t\tP: \"AccessMode\", \"enum\", \"\", \"\",0");
        header.Append("\n\t\t\t}");
        header.Append("\n\t\t}");
        header.Append("\n\t}");

        header.Append("\n}");

        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            writer.Write(header.ToString());
            writer.Write(fbx.ToString());
            stream.Close();
        } //using

        Debug.Log("Done!");
    } //ConvertToFbx

    private static void GetObjects(Transform transform)
    {
        GetMeshes(transform);

        foreach (Transform t in meshes[0].Item2.bones)
        {
            bones.Add(new Tuple<int, Transform>(modelId, t));
            modelId++;
            nodes.Add(modelId);
            nodesToBones.Add(new Tuple<int, int>(modelId, modelId - 1));
            modelId++;
        } //foreach

        GetBoneConnections();
        GetGameObjects(transform);
        GetGameObjectConnections();
    } //GetObjects

    private static void GetMeshes(Transform transform)
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(new Tuple<int, SkinnedMeshRenderer>(modelId, t.gameObject.GetComponent<SkinnedMeshRenderer>()));
                geometry.Add(geometryId);
                geometryToMeshes.Add(new Tuple<int, int>(geometryId, modelId));

                modelId++;

                deformers.Add(modelId);
                deformersToGeometry.Add(new Tuple<int, int>(modelId, geometryId));

                geometryId++;

                int foundMaterialId = -1;

                for (int i = 0; i < materials.Count; i++)
                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial == materials[i].Item2)
                        foundMaterialId = materials[i].Item1;

                if (foundMaterialId == -1)
                {
                    materials.Add(new Tuple<int, Material>(materialId, t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial));
                    materialsToMeshes.Add(new Tuple<int, int>(materialId, modelId - 1));

                    //Diffuse
                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.HasProperty("Base_Tex_SRGB"))
                        if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("Base_Tex_SRGB"))
                        {
                            int foundTextureId = -1;

                            for (int i = 0; i < textures.Count; i++)
                                if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("Base_Tex_SRGB") == textures[i].Item2)
                                    foundTextureId = textures[i].Item1;

                            if (foundTextureId == -1)
                            {
                                videos.Add(videoId);
                                textures.Add(new Tuple<int, Texture>(textureId, t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("Base_Tex_SRGB")));
                                videosToTextures.Add(new Tuple<int, int>(videoId, textureId));
                                texturesToMaterials.Add(new Tuple<int, int, string>(textureId, materialId, "DiffuseColor"));
                                videoId++;
                                textureId++;
                            } //if
                            else
                                texturesToMaterials.Add(new Tuple<int, int, string>(foundTextureId, materialId, "DiffuseColor"));
                        } //if

                    //Normal
                    if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.HasProperty("NormalMap_Tex_NRM"))
                        if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("NormalMap_Tex_NRM"))
                        {
                            int foundTextureId = -1;

                            for (int i = 0; i < textures.Count; i++)
                                if (t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("NormalMap_Tex_NRM") == textures[i].Item2)
                                    foundTextureId = textures[i].Item1;

                            if (foundTextureId == -1)
                            {
                                videos.Add(videoId);
                                textures.Add(new Tuple<int, Texture>(textureId, t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial.GetTexture("NormalMap_Tex_NRM")));
                                videosToTextures.Add(new Tuple<int, int>(videoId, textureId));
                                texturesToMaterials.Add(new Tuple<int, int, string>(textureId, materialId, "Bump"));
                                videoId++;
                                textureId++;
                            } //if
                            else
                                texturesToMaterials.Add(new Tuple<int, int, string>(foundTextureId, materialId, "Bump"));
                        } //if

                    materialId++;
                } //if
                else
                    materialsToMeshes.Add(new Tuple<int, int>(foundMaterialId, modelId - 1));

                modelId++;
                GetMeshes(t);
            } //if
        } //foreach
    } //GetMeshes ends

    private static void GetNumObjects(Transform transform, ref int count)
    {
        foreach (Transform t in transform)
        {
            count++;
            GetNumObjects(t, ref count);
        } //foreach
    } //GetNumObjects


    private static void GetGameObjects(Transform transform)
    {
        foreach (Transform t in transform)
        {
            bool add = true;

            for (int i = 0; i < meshes.Count; i++)
            {
                if (t.name == meshes[i].Item2.name)
                    add = false;
            } //for

            for (int i = 0; i < bones.Count; i++)
            {
                if (t.name == bones[i].Item2.name)
                    add = false;
            } //for

            if (add)
            {
                objects.Add(new Tuple<int, GameObject>(modelId, t.gameObject));
                modelId++;
            } //if

            GetGameObjects(t);
        } //foreach ends
    } //GetGameObjects

    private static void GetBoneConnections()
    {
        for (int i = 0; i < bones.Count; i++)
        {
            for (int j = 1; j < bones.Count; j++)
                if (bones[j].Item2.parent.gameObject.name == bones[i].Item2.gameObject.name)
                    boneConnections.Add(new Tuple<int, int>(bones[j].Item1, bones[i].Item1));
        } //for
    } //GetGameObjectConnections

    private static void GetGameObjectConnections()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            for (int j = 0; j < objects.Count; j++)
                if (objects[j].Item2.transform.parent != null)
                    if (objects[j].Item2.transform.parent.name == objects[i].Item2.name)
                        objectConnections.Add(new Tuple<int, int>(objects[j].Item1, objects[i].Item1));

            for (int j = 0; j < meshes.Count; j++)
                if (meshes[j].Item2.gameObject.transform.parent.name == objects[i].Item2.name)
                    objectConnections.Add(new Tuple<int, int>(meshes[j].Item1, objects[i].Item1));

            for (int j = 0; j < bones.Count; j++)
                if (bones[j].Item2.parent.name == objects[i].Item2.name)
                    objectConnections.Add(new Tuple<int, int>(bones[j].Item1, objects[i].Item1));
        } //for ends
    } //GetGameObjectConnections

    private static HashSet<int> GetUsedBones(SkinnedMeshRenderer skinnedMeshRenderer)
    {
        HashSet<int> usedBones = new HashSet<int>();

        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.boneWeights.Length; i++)
        {
            if (skinnedMeshRenderer.sharedMesh.boneWeights[i].weight0 > 0)
                usedBones.Add(skinnedMeshRenderer.sharedMesh.boneWeights[i].boneIndex0);
            if (skinnedMeshRenderer.sharedMesh.boneWeights[i].weight1 > 0)
                usedBones.Add(skinnedMeshRenderer.sharedMesh.boneWeights[i].boneIndex1);
            if (skinnedMeshRenderer.sharedMesh.boneWeights[i].weight2 > 0)
                usedBones.Add(skinnedMeshRenderer.sharedMesh.boneWeights[i].boneIndex2);
            if (skinnedMeshRenderer.sharedMesh.boneWeights[i].weight3 > 0)
                usedBones.Add(skinnedMeshRenderer.sharedMesh.boneWeights[i].boneIndex3);
        } //for

        return usedBones;
    } //GetUsedBones

    private static void Clear()
    {
        videoId = 600000000;
        textureId = 700000000;
        materialId = 800000000;
        geometryId = 900000000;
        modelId = 1000000001;
        objects.Clear();
        objectConnections.Clear();
        nodes.Clear();
        bones.Clear();
        nodesToBones.Clear();
        boneConnections.Clear();
        geometry.Clear();
        meshes.Clear();
        geometryToMeshes.Clear();
        deformers.Clear();
        subDeformersToDeformers.Clear();
        bonesToSubDeformers.Clear();
        materials.Clear();
        materialsToMeshes.Clear();
        videos.Clear();
        textures.Clear();
        videosToTextures.Clear();
        texturesToMaterials.Clear();
    } //Clear
} //class