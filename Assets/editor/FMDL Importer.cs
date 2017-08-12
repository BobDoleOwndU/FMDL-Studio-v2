using UnityEngine;
using System.IO;


namespace FmdlTool
{
    public static class FMDLImporter
    {
        public static void FMDLStream(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            Fmdl file = new Fmdl();
            file.Read(stream);
            GameObjCreator gameObj = new GameObjCreator();
            gameObj.Start();
            file.MeshReader();
            stream.Close();
        }
    }
}