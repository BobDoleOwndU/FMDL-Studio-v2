using UnityEditor;

public class FmdlWindow : EditorWindow
{
    [MenuItem("FMDL Studio/Import FMDL", false, 0)]
    public static void ShowWindow()
    {
        string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        FmdlImporter.FMDLStream(windowPath);

        UnityEngine.Debug.Log("Selected FMDL: " + windowPath);
    }
}