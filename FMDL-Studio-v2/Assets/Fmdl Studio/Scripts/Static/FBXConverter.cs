using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FmdlStudio.Scripts.Static
{
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

            int boneCount = bones.Count;
            int meshCount = meshes.Count;
            int objectCount = objects.Count;
            int geometryCount = geometry.Count;
            int materialCount = materials.Count;
            int deformerCount = deformers.Count;
            int videoCount = videos.Count;
            int textureCount = textures.Count;

            const string BAR_STRING = "Converting to FBX!";
            EditorUtility.DisplayProgressBar(BAR_STRING, "Starting!", 0);

            //Object Properties
            fbx.Append("\n; Object properties");
            fbx.Append("\n;------------------------------------------------------------------");
            fbx.Append("\n\nObjects:  {");

            //Bone Nodes
            for (int i = 0; i < boneCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Bone Nodes: {i}/{boneCount}", (float)i / boneCount);

                fbx.AppendFormat("\n\tNodeAttribute: {0}, \"NodeAttribute::\", \"LimbNode\" {{", nodesToBones[i].Item1);
                fbx.Append("\n\t\tTypeFlags: \"Skeleton\"");
                fbx.Append("\n\t}");
            } //for

            //Meshes
            for (int i = 0; i < meshCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Meshes: {i}/{meshCount}", (float)i / meshCount);

                Vector3[] vertices = meshes[i].Item2.sharedMesh.vertices;
                int[] triangles = meshes[i].Item2.sharedMesh.triangles;
                Vector3[] normals = meshes[i].Item2.sharedMesh.normals;
                Vector4[] tangents = meshes[i].Item2.sharedMesh.tangents;
                Color[] colors = meshes[i].Item2.sharedMesh.colors;
                Vector2[] uv = meshes[i].Item2.sharedMesh.uv;
                Vector2[] uv2 = meshes[i].Item2.sharedMesh.uv2;
                Vector2[] uv3 = meshes[i].Item2.sharedMesh.uv3;
                Vector2[] uv4 = meshes[i].Item2.sharedMesh.uv4;

                int vertexCount = vertices.Length;
                int triangleCount = triangles.Length;
                int colorCount = colors.Length;
                int uvCount = uv.Length;
                int uv2Count = uv2.Length;
                int uv3Count = uv3.Length;
                int uv4Count = uv4.Length;

                fbx.AppendFormat("\n\tGeometry: {0}, \"Geometry::Scene\", \"Mesh\" {{", geometryToMeshes[i].Item1);

                //Vertex Positions
                fbx.AppendFormat("\n\t\tVertices: *{0} {{", vertexCount * 3);
                fbx.Append("\n\t\t\ta: ");
                for (int j = 0; j < vertexCount; j++)
                    fbx.AppendFormat("{0},{1},{2},", -vertices[j].x, vertices[j].y, vertices[j].z);
                fbx.Length--;
                fbx.Append("\n\t\t}");

                //Faces
                fbx.AppendFormat("\n\t\tPolygonVertexIndex: *{0} {{", triangleCount);
                fbx.Append("\n\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                    fbx.AppendFormat("{0},{1},{2},", triangles[j * 3], triangles[j * 3 + 2], (-triangles[j * 3 + 1] - 1));
                fbx.Length--;
                fbx.Append("\n\t\t}");

                //Normals
                fbx.Append("\n\t\tGeometryVersion: 124");
                fbx.Append("\n\t\tLayerElementNormal: 0 {");
                fbx.Append("\n\t\t\tVersion: 102");
                fbx.Append("\n\t\t\tName: \"Normals\"");
                fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                fbx.Append("\n\t\t\tReferenceInformationType: \"Direct\"");
                fbx.AppendFormat("\n\t\t\tNormals: *{0} {{", triangleCount * 3);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                {
                    fbx.AppendFormat("{0},{1},{2},", -normals[triangles[j * 3]].x, normals[triangles[j * 3]].y, normals[triangles[j * 3]].z);
                    fbx.AppendFormat("{0},{1},{2},", -normals[triangles[j * 3 + 2]].x, normals[triangles[j * 3 + 2]].y, normals[triangles[j * 3 + 2]].z);
                    fbx.AppendFormat("{0},{1},{2},", -normals[triangles[j * 3 + 1]].x, normals[triangles[j * 3 + 1]].y, normals[triangles[j * 3 + 1]].z);
                } //for
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.AppendFormat("\n\t\t\tNormalsW: *{0} {{", triangleCount);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount; j++)
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
                fbx.AppendFormat("\n\t\t\tBinormals: *{0} {{", triangleCount * 3);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                {
                    Vector3 binormal0 = Vector3.Cross(normals[triangles[j * 3]], new Vector3(tangents[triangles[j * 3]].x, tangents[triangles[j * 3]].y, tangents[triangles[j * 3]].z));
                    Vector3 binormal1 = Vector3.Cross(normals[triangles[j * 3 + 2]], new Vector3(tangents[triangles[j * 3 + 2]].x, tangents[triangles[j * 3 + 2]].y, tangents[triangles[j * 3 + 2]].z));
                    Vector3 binormal2 = Vector3.Cross(normals[triangles[j * 3 + 1]], new Vector3(tangents[triangles[j * 3 + 1]].x, tangents[triangles[j * 3 + 1]].y, tangents[triangles[j * 3 + 1]].z));

                    fbx.AppendFormat("{0},{1},{2},", binormal0.x, binormal0.y, -binormal0.z);
                    fbx.AppendFormat("{0},{1},{2},", binormal1.x, binormal1.y, -binormal1.z);
                    fbx.AppendFormat("{0},{1},{2},", binormal2.x, binormal2.y, -binormal2.z);
                } //for
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.AppendFormat("\n\t\t\tBinormalsW: *{0} {{", triangleCount);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount; j++)
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
                fbx.AppendFormat("\n\t\t\tTangents: *{0} {{", triangleCount * 3);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                {
                    fbx.AppendFormat("{0},{1},{2},", -tangents[triangles[j * 3]].x, tangents[triangles[j * 3]].y, tangents[triangles[j * 3]].z);
                    fbx.AppendFormat("{0},{1},{2},", -tangents[triangles[j * 3 + 2]].x, tangents[triangles[j * 3 + 2]].y, tangents[triangles[j * 3 + 2]].z);
                    fbx.AppendFormat("{0},{1},{2},", -tangents[triangles[j * 3 + 1]].x, tangents[triangles[j * 3 + 1]].y, tangents[triangles[j * 3 + 1]].z);
                } //for
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.AppendFormat("\n\t\t\tTangentsW: *{0} {{", triangleCount);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                    fbx.AppendFormat("{0},{1},{2},", tangents[triangles[j * 3]].w, tangents[triangles[j * 3 + 2]].w, tangents[triangles[j * 3 + 1]].w);
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.Append("\n\t\t}");

                //Colours
                fbx.Append("\n\t\tLayerElementColor: 0 {");
                fbx.Append("\n\t\t\tVersion: 101");
                fbx.Append("\n\t\t\tName: \"VertexColors\"");
                fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                fbx.AppendFormat("\n\t\t\tColors: *{0} {{", colorCount * 4);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < colorCount; j++)
                {
                    fbx.AppendFormat("{0},{1},{2},{3},", colors[j].r, colors[j].g, colors[j].b, colors[j].a);
                } //for
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.AppendFormat("\n\t\t\tColorIndex: *{0} {{", triangleCount);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                    fbx.AppendFormat("{0},{1},{2},", triangles[j * 3], triangles[j * 3 + 2], triangles[j * 3 + 1]);
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.Append("\n\t\t}");

                //UV Set 0
                fbx.Append("\n\t\tLayerElementUV: 0 {");
                fbx.Append("\n\t\t\tVersion: 101");
                fbx.Append("\n\t\t\tName: \"UVSet0\"");
                fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                fbx.AppendFormat("\n\t\t\tUV: *{0} {{", uvCount * 2);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < uvCount; j++)
                    fbx.AppendFormat("{0},{1},", uv[j].x, uv[j].y);
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", triangleCount);
                fbx.Append("\n\t\t\t\ta: ");
                for (int j = 0; j < triangleCount / 3; j++)
                    fbx.AppendFormat("{0},{1},{2},", triangles[j * 3], triangles[j * 3 + 2], triangles[j * 3 + 1]);
                fbx.Length--;
                fbx.Append("\n\t\t\t}");
                fbx.Append("\n\t\t}");

                //UV Set 1
                if (!uv2.IsNullOrZeroes())
                {
                    fbx.Append("\n\t\tLayerElementUV: 1 {");
                    fbx.Append("\n\t\t\tVersion: 101");
                    fbx.Append("\n\t\t\tName: \"UVSet1\"");
                    fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                    fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                    fbx.AppendFormat("\n\t\t\tUV: *{0} {{", uv2Count * 2);
                    fbx.Append("\n\t\t\t\ta: ");
                    for (int j = 0; j < uv2Count; j++)
                        fbx.AppendFormat("{0},{1},", uv2[j].x, uv2[j].y);
                    fbx.Length--;
                    fbx.Append("\n\t\t\t}");
                    fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", triangleCount);
                    fbx.Append("\n\t\t\t\ta: ");
                    for (int j = 0; j < triangleCount / 3; j++)
                        fbx.AppendFormat("{0},{1},{2},", triangles[j * 3], triangles[j * 3 + 2], triangles[j * 3 + 1]);
                    fbx.Length--;
                    fbx.Append("\n\t\t\t}");
                    fbx.Append("\n\t\t}");

                    //UV Set 2
                    if (!uv3.IsNullOrZeroes())
                    {
                        fbx.Append("\n\t\tLayerElementUV: 2 {");
                        fbx.Append("\n\t\t\tVersion: 101");
                        fbx.Append("\n\t\t\tName: \"UVSet2\"");
                        fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                        fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                        fbx.AppendFormat("\n\t\t\tUV: *{0} {{", uv3Count * 2);
                        fbx.Append("\n\t\t\t\ta: ");
                        for (int j = 0; j < uv3Count; j++)
                            fbx.AppendFormat("{0},{1},", uv3[j].x, uv3[j].y);
                        fbx.Length--;
                        fbx.Append("\n\t\t\t}");
                        fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", triangleCount);
                        fbx.Append("\n\t\t\t\ta: ");
                        for (int j = 0; j < triangleCount / 3; j++)
                            fbx.AppendFormat("{0},{1},{2},", triangles[j * 3], triangles[j * 3 + 2], triangles[j * 3 + 1]);
                        fbx.Length--;
                        fbx.Append("\n\t\t\t}");
                        fbx.Append("\n\t\t}");

                        //UV Set 3
                        if (!uv4.IsNullOrZeroes())
                        {
                            fbx.Append("\n\t\tLayerElementUV: 3 {");
                            fbx.Append("\n\t\t\tVersion: 101");
                            fbx.Append("\n\t\t\tName: \"UVSet3\"");
                            fbx.Append("\n\t\t\tMappingInformationType: \"ByPolygonVertex\"");
                            fbx.Append("\n\t\t\tReferenceInformationType: \"IndexToDirect\"");
                            fbx.AppendFormat("\n\t\t\tUV: *{0} {{", uv4Count * 2);
                            fbx.Append("\n\t\t\t\ta: ");
                            for (int j = 0; j < uv4Count; j++)
                                fbx.AppendFormat("{0},{1},", uv4[j].x, uv4[j].y);
                            fbx.Length--;
                            fbx.Append("\n\t\t\t}");
                            fbx.AppendFormat("\n\t\t\tUVIndex: *{0} {{", triangleCount);
                            fbx.Append("\n\t\t\t\ta: ");
                            for (int j = 0; j < triangleCount / 3; j++)
                                fbx.AppendFormat("{0},{1},{2},", triangles[j * 3], triangles[j * 3 + 2], triangles[j * 3 + 1]);
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
                if (!uv2.IsNullOrZeroes())
                {
                    fbx.Append("\n\t\tLayer: 1 {");
                    fbx.Append("\n\t\t\tVersion: 100");
                    fbx.Append("\n\t\t\tLayerElement:  {");
                    fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
                    fbx.Append("\n\t\t\t\tTypedIndex: 1");
                    fbx.Append("\n\t\t\t}");
                    fbx.Append("\n\t\t}");

                    //Layer 2
                    if (!uv3.IsNullOrZeroes())
                    {
                        fbx.Append("\n\t\tLayer: 2 {");
                        fbx.Append("\n\t\t\tVersion: 100");
                        fbx.Append("\n\t\t\tLayerElement:  {");
                        fbx.Append("\n\t\t\t\tType: \"LayerElementUV\"");
                        fbx.Append("\n\t\t\t\tTypedIndex: 2");
                        fbx.Append("\n\t\t\t}");
                        fbx.Append("\n\t\t}");

                        //Layer 3
                        if (!uv4.IsNullOrZeroes())
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
            for (int i = 0; i < objectCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Objects: {i}/{objectCount}", (float)i / objectCount);

                Tuple<int, GameObject> obj = objects[i];

                fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"Null\" {{", obj.Item1, obj.Item2.name);
                fbx.Append("\n\t\tVersion: 232");
                fbx.Append("\n\t\tProperties70:  {");
                fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
                if (obj.Item2.transform.localPosition.x != 0 || obj.Item2.transform.localPosition.y != 0 || obj.Item2.transform.localPosition.z != 0)
                    fbx.AppendFormat("\n\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",{0},{1},{2}", -obj.Item2.transform.localPosition.x, obj.Item2.transform.localPosition.y, obj.Item2.transform.localPosition.z);
                fbx.Append("\n\t\t}");
                fbx.Append("\n\t\tShading: Y");
                fbx.Append("\n\t\tCulling: \"CullingOff\"");
                fbx.Append("\n\t}");
            } //for

            //Bones Names/Positions
            for (int i = 0; i < boneCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Bones: {i}/{boneCount}", (float)i / boneCount);

                Tuple<int, Transform> bone = bones[i];

                fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"LimbNode\" {{", bone.Item1, bone.Item2.gameObject.name);
                fbx.Append("\n\t\tVersion: 232");
                fbx.Append("\n\t\tProperties70:  {");
                fbx.Append("\n\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",1");
                fbx.Append("\n\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
                fbx.Append("\n\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");
                if (bone.Item2.transform.localPosition.x != 0 || bone.Item2.transform.localPosition.y != 0 || bone.Item2.transform.localPosition.z != 0)
                    fbx.AppendFormat("\n\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",{0},{1},{2}", -bone.Item2.transform.localPosition.x, bone.Item2.transform.localPosition.y, bone.Item2.transform.localPosition.z);
                fbx.AppendFormat("\n\t\t\tP: \"MaxHandle\", \"int\", \"Integer\", \"UH\",{0}", i + 1);
                fbx.Append("\n\t\t}");
                fbx.Append("\n\t\tShading: T");
                fbx.Append("\n\t\tCulling: \"CullingOff\"");
                fbx.Append("\n\t}");
            } //for

            //Mesh Names
            for (int i = 0; i < meshCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Mesh Names: {i}/{meshCount}", (float)i / meshCount);

                Tuple<int, SkinnedMeshRenderer> mesh = meshes[i];

                fbx.AppendFormat("\n\tModel: {0}, \"Model::{1}\", \"Mesh\" {{", mesh.Item1, mesh.Item2.name);
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
            fbx.AppendFormat("\n\t\tNbPoseNodes: {0}", geometryCount);
            for (int i = 0; i < geometryCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Pose Nodes: {i}/{geometryCount}", (float)i / geometryCount);

                fbx.Append("\n\t\tPoseNode:  {");
                fbx.AppendFormat("\n\t\t\tNode: {0}", geometry[i]);
                fbx.Append("\n\t\t}");
            } //for
            fbx.Append("\n\t}");

            //Materials
            for (int i = 0; i < materialCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Materials: {i}/{materialCount}", (float)i / materialCount);

                Tuple<int, Material> material = materials[i];

                fbx.AppendFormat("\n\tMaterial: {0}, \"Material::{1}\", \"\" {{", material.Item1, material.Item2);
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
            for (int i = 0; i < deformerCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Deformers: {i}/{deformerCount}", (float)i / deformerCount);

                Tuple<int, SkinnedMeshRenderer> mesh = meshes[i];
                List<Transform> meshBones = new List<Transform>(0);
                meshBones.AddRange(mesh.Item2.bones);
                HashSet<int> usedBones = GetUsedBones(mesh.Item2);

                fbx.AppendFormat("\n\tDeformer: {0}, \"Deformer::\", \"Skin\" {{", deformers[i]);
                fbx.Append("\n\t\tVersion: 100");
                fbx.Append("\n\t\tLink_DeformAcuracy: 50");
                fbx.Append("\n\t}");

                foreach (int j in usedBones)
                {
                    BoneWeight[] boneWeights = mesh.Item2.sharedMesh.boneWeights;
                    Matrix4x4[] bindPoses = mesh.Item2.sharedMesh.bindposes;

                    List<int> indices = new List<int>(0);
                    List<float> weights = new List<float>(0);

                    subDeformersToDeformers.Add(new Tuple<int, int>(modelId, deformers[i]));
                    bonesToSubDeformers.Add(new Tuple<int, int>(bones[j].Item1, modelId));

                    for (int h = 0; h < boneWeights.Length; h++)
                    {
                        if (meshBones[boneWeights[h].boneIndex0] == bones[j].Item2)
                        {
                            indices.Add(h);
                            weights.Add(boneWeights[h].weight0);
                        } //if
                        else if (meshBones[boneWeights[h].boneIndex1] == bones[j].Item2)
                        {
                            indices.Add(h);
                            weights.Add(boneWeights[h].weight1);
                        } //if
                        else if (meshBones[boneWeights[h].boneIndex2] == bones[j].Item2)
                        {
                            indices.Add(h);
                            weights.Add(boneWeights[h].weight2);
                        } //if
                        else if (meshBones[boneWeights[h].boneIndex3] == bones[j].Item2)
                        {
                            indices.Add(h);
                            weights.Add(boneWeights[h].weight3);
                        } //if
                    } //for ends

                    int indexCount = indices.Count;
                    int weightCount = weights.Count;
                    int meshBoneCount = mesh.Item2.bones.Length;
                    int bindPoseIndex = -1;

                    for (int h = 0; h < meshBoneCount; h++)
                    {
                        if (mesh.Item2.bones[h] == bones[j].Item2)
                        {
                            bindPoseIndex = h;
                            break;
                        } //if
                    } //for

                    fbx.AppendFormat("\n\tDeformer: {0}, \"SubDeformer::\", \"Cluster\" {{", modelId);
                    fbx.Append("\n\t\tVersion: 100");
                    fbx.Append("\n\t\tUserData: \"\", \"\"");
                    fbx.AppendFormat("\n\t\tIndexes: *{0} {{", indexCount);
                    fbx.Append("\n\t\t\ta: ");
                    for (int h = 0; h < indexCount; h++)
                        fbx.AppendFormat("{0},", indices[h]);
                    fbx.Length--;
                    fbx.Append("\n\t\t}");
                    fbx.AppendFormat("\n\t\tWeights: *{0} {{", weightCount);
                    fbx.Append("\n\t\t\ta: ");
                    for (int h = 0; h < weightCount; h++)
                        fbx.AppendFormat("{0},", weights[h]);
                    fbx.Length--;
                    fbx.Append("\n\t\t}");
                    fbx.Append("\n\t\tTransform: *16 {");
                    fbx.Append("\n\t\t\ta: ");
                    fbx.AppendFormat("{0},{1},{2},{3},", bindPoses[bindPoseIndex][0, 0], -bindPoses[bindPoseIndex][1, 0], -bindPoses[bindPoseIndex][2, 0], bindPoses[bindPoseIndex][3, 0]);
                    fbx.AppendFormat("{0},{1},{2},{3},", -bindPoses[bindPoseIndex][0, 1], bindPoses[bindPoseIndex][1, 1], bindPoses[bindPoseIndex][2, 1], bindPoses[bindPoseIndex][3, 1]);
                    fbx.AppendFormat("{0},{1},{2},{3},", -bindPoses[bindPoseIndex][0, 2], bindPoses[bindPoseIndex][1, 2], bindPoses[bindPoseIndex][2, 2], bindPoses[bindPoseIndex][3, 2]);
                    fbx.AppendFormat("{0},{1},{2},{3}", -bindPoses[bindPoseIndex][0, 3], bindPoses[bindPoseIndex][1, 3], bindPoses[bindPoseIndex][2, 3], bindPoses[bindPoseIndex][3, 3]);
                    fbx.Append("\n\t\t}");
                    fbx.Append("\n\t}");
                    modelId++;
                } //foreach
            } //for

            //Texture Videos
            for (int i = 0; i < videoCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Texture Videos: {i}/{videoCount}", (float)i / videoCount);

                Texture texture = textures[i].Item2;

                fbx.AppendFormat("\n\tVideo: {0}, \"Video::{1}\", \"Clip\" {{", videos[i], texture.name);
                fbx.Append("\n\t\tType: \"Clip\"");
                fbx.Append("\n\t\tProperties70:  {");
                fbx.AppendFormat("\n\t\t\tP: \"Path\", \"KString\", \"XRefUrl\", \"\", \"{0}\"", Globals.texturePath + "\\" + texture.name);
                fbx.Append("\n\t\t}");
                fbx.Append("\n\t\tUseMipMap: 0");
                fbx.AppendFormat("\n\t\tFilename: \"{0}\"", Globals.texturePath + "\\" + texture.name);
                //fbx.AppendFormat("\n\t\tRelativeFilename: \"{0}\"", gameObject.name + "\\" + texture.name);
                fbx.Append("\n\t}");
            } //for

            //Textures
            for (int i = 0; i < textureCount; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Textures: {i}/{textureCount}", (float)i / textureCount);

                Tuple<int, Texture> texture = textures[i];

                fbx.AppendFormat("\n\tTexture: {0}, \"Texture::{1}\", \"\" {{", texture.Item1, texture.Item2.name);
                fbx.Append("\n\t\tType: \"TextureVideoClip\"");
                fbx.Append("\n\t\tVersion: 202");
                fbx.AppendFormat("\n\t\tTextureName: \"Texture::{0}\"", texture.Item2.name);
                fbx.Append("\n\t\tProperties70:  {");
                fbx.Append("\n\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"UVSet0\"");
                fbx.Append("\n\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",1");
                fbx.Append("\n\t\t}");
                fbx.AppendFormat("\n\t\tMedia: \"Video::{0}\"", texture.Item2.name);
                fbx.AppendFormat("\n\t\tFileName: \"{0}\"", Globals.texturePath + "\\" + texture.Item2.name);
                //fbx.AppendFormat("\n\t\tRelativeFilename: \"{0}\"", gameObject.name + "\\" + texture.Item2.name);
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
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Object Connections: {i}/{objectConnections.Count}", (float)i / objectConnections.Count);

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
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Node to Bone Connections: {i}/{nodesToBones.Count}", (float)i / nodesToBones.Count);

                string name1 = bones.Find(x => x.Item1 == nodesToBones[i].Item2).Item2.gameObject.name;

                fbx.AppendFormat("\n\n\t;NodeAttribute::, Model::{0}", name1);
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", nodesToBones[i].Item1, nodesToBones[i].Item2);
            } //for

            //Bone to Bone Connections
            for (int i = 0; i < boneConnections.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Bone Connections: {i}/{boneConnections.Count}", (float)i / boneConnections.Count);

                string name1 = bones.Find(x => x.Item1 == boneConnections[i].Item1).Item2.gameObject.name;
                string name2 = bones.Find(x => x.Item1 == boneConnections[i].Item2).Item2.gameObject.name;

                fbx.AppendFormat("\n\n\t;Model::{0}, Model::{1}", name1, name2);
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", boneConnections[i].Item1, boneConnections[i].Item2);
            } //for

            //Material to Mesh Connections
            for (int i = 0; i < materialsToMeshes.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Material to Mesh Connections: {i}/{materialsToMeshes.Count}", (float)i / materialsToMeshes.Count);

                string name1 = materials.Find(x => x.Item1 == materialsToMeshes[i].Item1).Item2.name;
                string name2 = meshes.Find(x => x.Item1 == materialsToMeshes[i].Item2).Item2.name;

                fbx.AppendFormat("\n\n\t;Material::{0}, Model::{1}", name1, name2);
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", materialsToMeshes[i].Item1, materialsToMeshes[i].Item2);
            } //for

            //Geometry to Mesh Connections
            for (int i = 0; i < geometryToMeshes.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Geometry to Mesh Connections: {i}/{geometryToMeshes.Count}", (float)i / geometryToMeshes.Count);

                string name1 = meshes.Find(x => x.Item1 == geometryToMeshes[i].Item2).Item2.name;

                fbx.AppendFormat("\n\n\t;Geometry::Scene, Model::{0}", name1);
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", geometryToMeshes[i].Item1, geometryToMeshes[i].Item2);
            } //for

            //Deformer to Geometry Connections
            for (int i = 0; i < deformersToGeometry.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Deformer to Geometry Connections: {i}/{deformersToGeometry.Count}", (float)i / deformersToGeometry.Count);

                fbx.Append("\n\n\t;Deformer::, Geometry::Scene");
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", deformersToGeometry[i].Item1, deformersToGeometry[i].Item2);
            } //for

            //SubDeformer to Deformer Connections
            for (int i = 0; i < subDeformersToDeformers.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"SubDeformer to Deformer Connections: {i}/{subDeformersToDeformers.Count}", (float)i / subDeformersToDeformers.Count);

                fbx.Append("\n\n\t;SubDeformer::, Deformer::");
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", subDeformersToDeformers[i].Item1, subDeformersToDeformers[i].Item2);
            } //for

            //Bone to SubDeformer Connections
            for (int i = 0; i < bonesToSubDeformers.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Bone to SubDeformer Connections: {i}/{bonesToSubDeformers.Count}", (float)i / bonesToSubDeformers.Count);

                string name1 = bones.Find(x => x.Item1 == bonesToSubDeformers[i].Item1).Item2.gameObject.name;

                fbx.AppendFormat("\n\n\t;Model::{0}, SubDeformer::", name1);
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", bonesToSubDeformers[i].Item1, bonesToSubDeformers[i].Item2);
            } //for

            //Texture to Material Connections
            for (int i = 0; i < texturesToMaterials.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Texture to Material Connections: {i}/{texturesToMaterials.Count}", (float)i / texturesToMaterials.Count);

                string name1 = textures.Find(x => x.Item1 == texturesToMaterials[i].Item1).Item2.name;
                string name2 = materials.Find(x => x.Item1 == texturesToMaterials[i].Item2).Item2.name;

                fbx.AppendFormat("\n\n\t;Texture::{0}, Material::{1}", name1, name2);
                fbx.AppendFormat("\n\tC: \"OP\",{0},{1}, \"{2}\"", texturesToMaterials[i].Item1, texturesToMaterials[i].Item2, texturesToMaterials[i].Item3);
            } //for

            //Texture Video to Texture Connections
            for (int i = 0; i < videosToTextures.Count; i++)
            {
                EditorUtility.DisplayProgressBar(BAR_STRING, $"Video to Texture Connections: {i}/{videosToTextures.Count}", (float)i / videosToTextures.Count);

                string name1 = textures.Find(x => x.Item1 == videosToTextures[i].Item2).Item2.name;

                fbx.AppendFormat("\n\n\t;Video::{0}, Texture::{0}", name1);
                fbx.AppendFormat("\n\tC: \"OO\",{0},{1}", videosToTextures[i].Item1, videosToTextures[i].Item2);
            } //for

            EditorUtility.DisplayProgressBar(BAR_STRING, "Takes: 0/1", 0);

            //Takes
            fbx.Append("\n}");
            fbx.Append("\n;Takes section");
            fbx.Append("\n;----------------------------------------------------");
            fbx.Append("\n\nTakes:  {");
            fbx.Append("\n\tCurrent: \"\"");
            fbx.Append("\n}");

            EditorUtility.DisplayProgressBar(BAR_STRING, "Header: 0/1", 0);

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

            EditorUtility.ClearProgressBar();
            Debug.Log("Done!");
        } //ConvertToFbx

        private static void GetObjects(Transform transform)
        {
            GetMeshes(transform);

            foreach (Transform t in transform)
            {
                if (t.gameObject.name == "[Root]")
                {
                    GetBones(t);
                    break;
                } //if
            } //foreach

            /*foreach (Transform t in meshes[0].Item2.bones)
            {
                bones.Add(new Tuple<int, Transform>(modelId, t));
                modelId++;
                nodes.Add(modelId);
                nodesToBones.Add(new Tuple<int, int>(modelId, modelId - 1));
                modelId++;
            } //foreach*/

            GetBoneConnections();
            GetGameObjects(transform);
            GetGameObjectConnections();
        } //GetObjects

        private static void GetBones(Transform transform)
        {
            foreach (Transform t in transform)
            {
                bones.Add(new Tuple<int, Transform>(modelId, t));
                modelId++;
                nodes.Add(modelId);
                nodesToBones.Add(new Tuple<int, int>(modelId, modelId - 1));
                modelId++;

                GetBones(t);
            } //foreach
        } //GetBones

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
            int boneCount = bones.Count;

            for (int i = 0; i < boneCount; i++)
            {
                Tuple<int, Transform> bone0 = bones[i];

                for (int j = 1; j < boneCount; j++)
                {
                    Tuple<int, Transform> bone1 = bones[j];

                    if (bone1.Item2.parent.gameObject.name == bone0.Item2.gameObject.name)
                        boneConnections.Add(new Tuple<int, int>(bone1.Item1, bone0.Item1));
                } //for
            } //for
        } //GetGameObjectConnections

        private static void GetGameObjectConnections()
        {
            int objectCount = objects.Count;
            int meshCount = meshes.Count;
            int boneCount = bones.Count;

            for (int i = 0; i < objectCount; i++)
            {
                Tuple<int, GameObject> obj = objects[i];

                for (int j = 0; j < objectCount; j++)
                {
                    Tuple<int, GameObject> obj2 = objects[j];

                    if (obj2.Item2.transform.parent != null)
                        if (obj2.Item2.transform.parent.name == obj.Item2.name)
                            objectConnections.Add(new Tuple<int, int>(obj2.Item1, obj.Item1));
                } //for

                for (int j = 0; j < meshCount; j++)
                {
                    Tuple<int, SkinnedMeshRenderer> mesh = meshes[j];

                    if (mesh.Item2.gameObject.transform.parent.name == obj.Item2.name)
                        objectConnections.Add(new Tuple<int, int>(mesh.Item1, obj.Item1));
                } //for

                for (int j = 0; j < boneCount; j++)
                {
                    Tuple<int, Transform> bone = bones[j];

                    if (bone.Item2.parent.name == obj.Item2.name)
                        objectConnections.Add(new Tuple<int, int>(bone.Item1, obj.Item1));
                } //for
            } //for ends
        } //GetGameObjectConnections

        private static HashSet<int> GetUsedBones(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            BoneWeight[] boneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;
            int boneWeightCount = boneWeights.Length;

            HashSet<int> usedBones = new HashSet<int>();

            for (int i = 0; i < boneWeightCount; i++)
            {
                if (boneWeights[i].weight0 > 0)
                    usedBones.Add(bones.IndexOf(bones.Find(x => x.Item2 == skinnedMeshRenderer.bones[boneWeights[i].boneIndex0])));
                if (boneWeights[i].weight1 > 0)
                    usedBones.Add(bones.IndexOf(bones.Find(x => x.Item2 == skinnedMeshRenderer.bones[boneWeights[i].boneIndex1])));
                if (boneWeights[i].weight2 > 0)
                    usedBones.Add(bones.IndexOf(bones.Find(x => x.Item2 == skinnedMeshRenderer.bones[boneWeights[i].boneIndex2])));
                if (boneWeights[i].weight3 > 0)
                    usedBones.Add(bones.IndexOf(bones.Find(x => x.Item2 == skinnedMeshRenderer.bones[boneWeights[i].boneIndex3])));
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
} //namespace