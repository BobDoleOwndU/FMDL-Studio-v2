using FmdlStudio.Scripts.Classes;
using FmdlStudio.Scripts.Static;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FmdlStudio.Editor.Windows
{
    public class FmdlStudioWindow : EditorWindow
    {
        [MenuItem("FMDL Studio/Import FMDL", false, 0)]
        public static void ImportFMDLOption()
        {
            string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");

            if (!string.IsNullOrWhiteSpace(windowPath))
            {
                FmdlImporter experimentalFmdlImporter = new FmdlImporter();
                experimentalFmdlImporter.ReadWithoutAssetImportContext(windowPath);
                Debug.Log("Selected FMDL: " + windowPath);
            } //if
            else
                Debug.Log("No path selected.");
        } //ImportFMDLOption

        [MenuItem("FMDL Studio/Convert to COLLADA", false, 1)]
        public static void ExportCOLLADAOption()
        {
            if (Selection.activeGameObject != null)
            {
                string filePath = EditorUtility.SaveFilePanel("Export To COLLADA", "", Selection.activeGameObject.name, "dae");

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    COLLADAConverter.ConvertToCOLLADA(Selection.activeGameObject, filePath);

                    if (!string.IsNullOrEmpty(Globals.GetFbxConverterPath()))
                    {
                        string fbxPath = $"{Path.GetDirectoryName(filePath)}\\{Path.GetFileNameWithoutExtension(filePath)}.fbx";

                        System.Diagnostics.Process.Start(Globals.GetFbxConverterPath(), $"{filePath} {fbxPath} /sffCOLLADA /dffFBX");
                    } //if
                } //if
                else
                    Debug.Log("No path selected.");
            } //if
            else
                Debug.Log("No objects selected.");
        } //ImportFMDLOption

        [MenuItem("FMDL Studio/Export FMDL", false, 2)]
        public static void ExportFMDLOption()
        {
            if (Selection.activeGameObject != null)
            {
                string windowPath = EditorUtility.SaveFilePanel("Export To FMDL", "", Selection.activeGameObject.name, "fmdl");

                if (!string.IsNullOrWhiteSpace(windowPath))
                {
                    Fmdl fmdl = new Fmdl(Selection.activeGameObject.name);
                    if (fmdl.Write(Selection.activeGameObject, windowPath))
                        Debug.Log("Fmdl Exported to: " + windowPath);
                } //if
                else
                    Debug.Log("No path selected.");
            }
            else
                Debug.Log("No objects selected.");
        } //ExportFMDLOption

        [MenuItem("FMDL Studio/Convert to FBX (Legacy)", false, 3)]
        public static void ExportFBXOption()
        {
            if (Selection.activeGameObject != null)
            {
                string filePath = EditorUtility.SaveFilePanel("Export To FBX", "", Selection.activeGameObject.name, "fbx");

                if (!string.IsNullOrWhiteSpace(filePath))
                    FBXConverter.ConvertToFBX(Selection.activeGameObject, filePath);
                else
                    Debug.Log("No path selected.");
            } //if
            else
                Debug.Log("No objects selected.");
        } //ExportFBXOption

        [MenuItem("FMDL Studio/Generate Bounding Boxes", false, 100)]
        public static void GenerateBoundingBoxes()
        {
            if (Selection.activeGameObject != null)
            {
                BoundingBoxGenerator.GenerateBoundingBoxes(Selection.activeGameObject.transform);
            } //if
            else
                Debug.Log("No objects selected.");
        } //GenerateBoundingBoxes

        [MenuItem("FMDL Studio/Fix Tangents", false, 101)]
        public static void FixTangents()
        {
            if (Selection.activeGameObject != null)
            {
                Utils.FixTangents(Selection.activeGameObject.transform);
            } //if
            else
                Debug.Log("No objects selected.");
        } //FixTangents

        [MenuItem("FMDL Studio/Reload Dictionaries", false, 102)]
        public static void ReloadDictionaries()
        {
            Hashing.LoadDictionaries();
        } //ReloadDictionaries

        [MenuItem("FMDL Studio/Fmdl Version/2.03 (GZ\u200A\u2215\u200APES)", false, 103)]
        public static void SetFmdlVersionToGZ()
        {
            Globals.SetFmdlVersion(2.03f);
            Debug.Log("Fmdl version set to 2.03");
        } //SetFmdlVersionToGZ

        [MenuItem("FMDL Studio/Fmdl Version/2.03 (GZ\u200A\u2215\u200APES)", true)]
        public static bool ValidateSetFmdlVersionToGZ()
        {
            return Globals.GetFmdlVersion() != 2.03f;
        } //ValidateSetFmdlVersionToGZ

        [MenuItem("FMDL Studio/Fmdl Version/2.04 (TPP)", false, 104)]
        public static void SetFmdlVersionToTPP()
        {
            Globals.SetFmdlVersion(2.04f);
            Debug.Log("Fmdl version set to 2.04");
        } //SetFmdlVersionToTPP

        [MenuItem("FMDL Studio/Fmdl Version/2.04 (TPP)", true)]
        public static bool ValidateSetFmdlVersionToTPP()
        {
            return Globals.GetFmdlVersion() != 2.04f;
        } //ValidateSetFmdlVersionToGZ

        [MenuItem("FMDL Studio/Set Texture Folder", false, 200)]
        public static void SetTextureFolder()
        {
            string windowPath = EditorUtility.OpenFolderPanel("Select Texture Folder", "", "");

            if (!string.IsNullOrEmpty(windowPath))
            {
                Globals.SetTexturePath(windowPath);
            } //if
            else
                Debug.Log("No folder selected.");
        } //SetTextureFolder

        [MenuItem("FMDL Studio/Set FBX Converter Path", false, 201)]
        public static void SetFbxConverterPath()
        {
            string filePath = EditorUtility.OpenFilePanel("Select FbxConverter.exe", "", "exe");

            if (!string.IsNullOrEmpty(filePath))
            {
                Globals.SetFbxConverterPath(filePath);
            } //if
            else
                Debug.Log("No folder selected.");
        } //SetFbxConverterPath

        [MenuItem("FMDL Studio/Bounding Box Tools/Copy Bounding Boxes", false, 301)]
        public static void CopyBoundingBoxes()
        {
            if (Selection.activeGameObject != null)
            {
                BoundingBoxTools.SetSource(Selection.activeGameObject.transform);
                Debug.Log("Bounding Boxes Copied!");
            } //if
            else
                Debug.Log("No objects selected.");
        } //CopyBoundingBoxes

        [MenuItem("FMDL Studio/Bounding Box Tools/Paste Bounding Boxes", false, 302)]
        public static void PasteBoundingBoxes()
        {
            if (Selection.activeGameObject != null)
            {
                BoundingBoxTools.SetTarget(Selection.activeGameObject.transform);
                Debug.Log("Bounding Boxes Pasted!");
            } //if
            else
                Debug.Log("No objects selected.");
        } //CopyBoundingBoxes

        [MenuItem("FMDL Studio/Bounding Box Tools/Clear Bounding Boxes", false, 303)]
        public static void ClearBoundingBoxes()
        {
            if (Selection.activeGameObject != null)
            {
                BoundingBoxTools.ClearBoundingBoxes(Selection.activeGameObject.transform);
                Debug.Log("Bounding Boxes Cleared!");
            } //if
            else
                Debug.Log("No objects selected.");
        } //CopyBoundingBoxes

        [MenuItem("FMDL Studio/Debug/Print Tangents", false, 500)]
        public static void PrintTangents()
        {
            if (Selection.activeGameObject != null)
            {
                Transform transform = Selection.activeGameObject.transform;
                List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);

                foreach(Transform t in transform)
                {
                    if(t.GetComponent<SkinnedMeshRenderer>())
                        meshes.Add(t.GetComponent<SkinnedMeshRenderer>());
                } //foreach

                foreach(SkinnedMeshRenderer mesh in meshes)
                    foreach(Vector4 v in mesh.sharedMesh.tangents)
                        Debug.Log(v);
            } //if
        } //CopyBoundingBoxes

        [MenuItem("FMDL Studio/Debug/Rotate", false, 501)]
        public static void Rotate()
        {
            if (Selection.activeGameObject != null)
            {
                Transform transform = Selection.activeGameObject.transform;
                transform.position = new Vector3(0f + -0.1f/* + -0.0019072f*/, 0.154f + 0.25f/* + 0.1949148f*/, 0f + -0.25f/* + -0.1283597f*/);


                Quaternion foxQuat0 = new Quaternion(0.3560068f, 0.6455829f, -0.328964919f, 0.590138853f);
                float angle0 = 0.0f;
                Vector3 axis0 = Vector3.zero;
                foxQuat0.ToAngleAxis(out angle0, out axis0);
                axis0.x = -axis0.x;
                Quaternion quat0 = Quaternion.AngleAxis(-angle0, axis0);

                /*Quaternion foxQuat1 = new Quaternion(0.04873222f, -0.456404716f, 0.888116f, -0.0238731913f);
                float angle1 = 0.0f;
                Vector3 axis1 = Vector3.zero;
                foxQuat1.ToAngleAxis(out angle1, out axis1);
                axis1.x = -axis1.x;
                Quaternion quat1 = Quaternion.AngleAxis(-angle1, axis1);*/

                transform.rotation = foxQuat0;
            } //if
        } //CopyBoundingBoxes
    } //class
} //namespace