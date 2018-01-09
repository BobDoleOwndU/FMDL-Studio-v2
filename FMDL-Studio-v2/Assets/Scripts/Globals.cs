using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FmdlStudio
{
    public static class Globals
    {
        public static string texturePath { get; private set; }

        public static void WriteTexturePath(string path)
        {
            using (FileStream stream = new FileStream("settings.cfg", FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                writer.WriteLine(string.Format("TextureFolder: \"{0}\"", path));
                stream.Close();
            } //using

            Debug.Log("Texture path set to: " + path);
        } //WriteTexturePath

        public static void ReadTexturePath()
        {
            if (File.Exists("settings.cfg"))
            {
                using (FileStream stream = new FileStream("settings.cfg", FileMode.Open))
                {
                    StreamReader reader = new StreamReader(stream);
                    List<string> lines = new List<string>(0);

                    while (!reader.EndOfStream)
                    {
                        lines.Add(reader.ReadLine());
                    } //while

                    foreach (string s in lines)
                    {
                        if (s.Contains("TextureFolder:"))
                        {
                            int pathStart = s.IndexOf('"');
                            int pathEnd = s.LastIndexOf('"');

                            texturePath = s.Substring(pathStart + 1, pathEnd - pathStart - 1);
                            break;
                        } //if
                    } //if

                    stream.Close();
                } //using
            } //if
        } //ReadTexturePath
    } //class
}
