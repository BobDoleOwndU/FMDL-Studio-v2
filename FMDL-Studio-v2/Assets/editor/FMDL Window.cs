using UnityEditor;
using static UnityEngine.Debug;
using UnityEditor.SceneManagement;
using System.IO;

public class FMDLWindow : EditorWindow
{
    [MenuItem("FMDL Studio/Import FMDL")]
    public static void ShowWindow()
    {
        string windowPath = EditorUtility.OpenFilePanel("Select FMDL", "", "fmdl");
        FMDLImporter.FMDLStream(windowPath);

        UnityEngine.Debug.Log("Selected FMDL: " + windowPath);
    }
}