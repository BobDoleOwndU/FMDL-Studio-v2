using UnityEditor;
using UnityEngine;

public class FmdlWindow : EditorWindow
{
    [MenuItem("FMDL Studio/Import FMDL", false, 0)]
    public static void ImportFMDLOption()
    {
        string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        FmdlImporter.FMDLRead(windowPath);

        UnityEngine.Debug.Log("Selected FMDL: " + windowPath);
    } //ImportFMDLOption

    [MenuItem("FMDL Studio/Convert to FBX", false, 1)]
    public static void ExportFBXOption()
    {
        if (Selection.activeGameObject != null)
        {
            string filePath = EditorUtility.SaveFilePanel("Save FBX", "", Selection.activeGameObject.name + ".fbx", "fbx");

            if (!string.IsNullOrWhiteSpace(filePath))
                FBXConverter.ConvertToFBX(Selection.activeGameObject, filePath);
            else
                Debug.Log("No path selected.");
        } //if
        else
            Debug.Log("No objects selected.");
    } //ExportFBXOption
}