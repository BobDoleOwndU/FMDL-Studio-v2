using System.IO;

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