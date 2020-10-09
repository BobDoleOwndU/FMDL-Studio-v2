using FmdlStudio.Scripts.Classes;
using FmdlStudio.Scripts.Static;
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
                    fmdl.Write(Selection.activeGameObject, windowPath);
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

        [MenuItem("FMDL Studio/Set FBX Converter Path", false, 202)]
        public static void SetFbxConverterPath()
        {
            string filePath = EditorUtility.OpenFilePanel("Select FbxConverter.exe", "", "exe");

            if (!string.IsNullOrEmpty(filePath))
            {
                Globals.SetFbxConverterPath(filePath);
            } //if
            else
                Debug.Log("No folder selected.");
        } //SetTextureFolder
    } //class
} //namespace