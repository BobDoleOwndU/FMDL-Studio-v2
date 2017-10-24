using UnityEditor;
using UnityEngine;

public class FBXWindow
{
    static int numObjects;

    [MenuItem("FMDL Studio/Convert to FBX", false, 1)]
    public static void ShowWindow()
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
    } //ShowWindow
} //class
