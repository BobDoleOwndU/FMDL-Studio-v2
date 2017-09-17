using System.IO;

public static class FmdlImporter
{
    public static void FMDLStream(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open);

        Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(path));
        UnityModel model = new UnityModel();
        fmdl.Read(stream);
        model.GetDataFromFmdl(fmdl);
        stream.Close();
    }
}