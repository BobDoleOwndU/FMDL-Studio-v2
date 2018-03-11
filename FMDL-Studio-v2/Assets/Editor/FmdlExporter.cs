using System.IO;
using UnityEngine;
using UnityEditor;
using System;

public class FmdlExporter
{
    public static void FMDLWrite(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            Fmdl fmdl = new Fmdl(selectedGameObject.name);
            fmdl.Write(selectedGameObject, stream);
        } //try
        catch (Exception e)
        {
            Debug.Log($"{e.Message} The stream was at offset 0x{stream.Position.ToString("x")} when this exception occured.");
            Debug.Log($"An exception occured{e.StackTrace}");
            stream.Close();
        } //catch

        stream.Close();
    }
}