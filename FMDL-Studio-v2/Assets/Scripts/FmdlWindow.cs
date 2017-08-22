using UnityEditor;

public class FmdlWindow : EditorWindow
{
    [MenuItem("FMDL Studio/Import FMDL")]
    public static void ShowWindow()
    {
        string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        FmdlImporter.FMDLStream(windowPath);

        UnityEngine.Debug.Log("Selected FMDL: " + windowPath);
    }
}