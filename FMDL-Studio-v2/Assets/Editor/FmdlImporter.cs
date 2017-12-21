using System.IO;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

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

        Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(fmdlPath));
        UnityModel model = new UnityModel();
        fmdl.Read(stream);
        model.GetDataFromFmdl(fmdl);
        stream.Close();
    }
}
#endif