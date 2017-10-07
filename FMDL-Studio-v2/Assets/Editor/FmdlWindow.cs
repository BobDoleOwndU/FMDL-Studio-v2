using UnityEditor;

public class FmdlWindow : EditorWindow
{
    [MenuItem("FMDL Studio/Import FMDL")]
    public static void LoadFMDL()
    {
        string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        FmdlImporter.FMDLStream(windowPath, false);

        UnityEngine.Debug.Log("Selected FMDL: " + windowPath);
    }

    [MenuItem("FMDL Studio/Import FMDL and save into Assets")]
    public static void LoadFMDLAndSave()
    {
        string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        FmdlImporter.FMDLStream(windowPath, true);

        UnityEngine.Debug.Log("Selected FMDL: " + windowPath);
    }
}
