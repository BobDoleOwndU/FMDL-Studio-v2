using System.IO;
using UnityEngine;
using UnityEditor;

public class FmdlExporter
{
    public static void FMDLWrite(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Create);

        GameObject selectedGameObject = Selection.activeGameObject;
        Fmdl fmdl = new Fmdl(selectedGameObject.name);
        fmdl.Write(selectedGameObject, stream);

        stream.Close();
    }
}