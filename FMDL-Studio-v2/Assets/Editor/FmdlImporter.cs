using System.IO;

public static class FmdlImporter
{
    public static void FMDLStream(string path, bool shouldSave)
    {
        FileStream stream = new FileStream(path, FileMode.Open);

        Fmdl fmdl = new Fmdl(Path.GetFileNameWithoutExtension(path));
        UnityModel model = new UnityModel();
        fmdl.Read(stream);
        model.GetDataFromFmdl(fmdl, shouldSave);
        stream.Close();
    }
}
