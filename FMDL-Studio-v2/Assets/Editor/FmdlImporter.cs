using System.IO;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System;

#if FOXKIT
[ScriptedImporter(1, "fmdl")]
public class FmdlImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        FileStream stream = new FileStream(ctx.assetPath, FileMode.Open);

        Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(ctx.assetPath));
        UnityModel model = new UnityModel();
        fmdl.Read(stream);
        GameObject fmdlGameObject = model.GetDataFromFmdl(fmdl);
        ctx.AddObjectToAsset(ctx.assetPath, fmdlGameObject);
        ctx.SetMainObject(fmdlGameObject);
        stream.Close();
    }
}
#else

public static class FmdlImporter
{
    public static void FMDLRead(string fmdlPath)
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
    }
}
#endif