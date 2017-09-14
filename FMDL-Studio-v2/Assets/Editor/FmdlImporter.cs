using UnityEngine;
using System.IO;

public static class FmdlImporter
{
    public static void FMDLStream(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open);

        Fmdl file = new Fmdl();
        file.Read(stream);
        file.MeshReader(path);
        stream.Close();
    }
}