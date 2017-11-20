using UnityEditor;
using UnityEngine;

public class FmdlStudioWindow : EditorWindow
{
    [MenuItem("FMDL Studio/Import FMDL", false, 0)]
    public static void ImportFMDLOption()
    {
        FmdlStudioWindow fmdlWindow = (FmdlStudioWindow)CreateInstance("FmdlStudioWindow");

        string fmdlPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        string texturePath = EditorUtility.OpenFolderPanel("Select Texture Folder", System.IO.Path.GetDirectoryName(fmdlPath), System.IO.Path.GetFileNameWithoutExtension(fmdlPath));
        FmdlImporter.FMDLRead(fmdlPath, texturePath);

        UnityEngine.Debug.Log("Selected FMDL: " + fmdlPath);
    } //ImportFMDLOption

    [MenuItem("FMDL Studio/Convert to FBX", false, 1)]
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

    [MenuItem("FMDL Studio/Export FMDL", false, 2)]
    public static void ExportFMDLOption()
    {
        if (Selection.activeGameObject != null)
        {
            string windowPath = EditorUtility.SaveFilePanel("Export To FMDL", "", Selection.activeGameObject.name, "fmdl");
            FmdlExporter.FMDLWrite(windowPath);
        }
        else
            Debug.Log("No path selected.");
        UnityEngine.Debug.Log("Selected FMDL Name: ");
    } //ImportFMDLOption
}