using System.IO;

public static class FmdlImporter
{
    public static void FMDLRead(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open);

        Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(path));
        UnityModel model = new UnityModel();
        fmdl.Read(stream);
        model.GetDataFromFmdl(fmdl);
        stream.Close();
    }
}