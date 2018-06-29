using System.IO;
using UnityEngine;
using System;

public static class FmdlImporter
{
    public static void FmdlRead(string fmdlPath)
    {
        FileStream stream = new FileStream(fmdlPath, FileMode.Open);

        try
        {
            Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(fmdlPath));
            UnityModel model = new UnityModel();
            fmdl.Read(stream);
            model.GetDataFromFmdl(fmdl);
            stream.Close();
        } //try
        catch (Exception e)
        {
            Debug.Log($"{e.Message} The stream was at offset 0x{stream.Position.ToString("x")} when this exception occured.");
            Debug.Log($"An exception occured{e.StackTrace}");
            stream.Close();
        } //catch
    } //FmdlRead
} //class