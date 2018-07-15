using FmdlStudio.Scripts.Classes;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace FmdlStudio.Scripts.Static
{
    public static class Globals
    {
        public static string texturePath { get; private set; }
        public static MaterialPresetList materialPresetList = new MaterialPresetList();

        static Globals()
        {
            ReadTexturePath();
            ReadPresetList();
        } //constructor

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

        public static void WriteTexturePath(string path)
        {
            using (FileStream stream = new FileStream("settings.cfg", FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                writer.WriteLine(string.Format("TextureFolder: \"{0}\"", path));
                stream.Close();
            } //using

            ReadTexturePath();
            Debug.Log("Texture path set to: " + path);
        } //WriteTexturePath

        public static void ReadPresetList()
        {
            if (File.Exists("Assets/Fmdl Studio/presets.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MaterialPresetList));

                using (FileStream stream = new FileStream("Assets/Fmdl Studio/presets.xml", FileMode.Open))
                {
                    materialPresetList = (MaterialPresetList)serializer.Deserialize(stream);
                    stream.Close();
                } //using
            } //if
            else
                Debug.Log("Could not find presets.xml");
        } //ReadMaterialList

        public static void WritePresetList()
        {
            materialPresetList.materialPresets.Sort((x, y) => x.name.CompareTo(y.name));

            XmlSerializer serializer = new XmlSerializer(typeof(MaterialPresetList));

            using (FileStream stream = new FileStream("Assets/Fmdl Studio/presets.xml", FileMode.Create))
            {
                serializer.Serialize(stream, materialPresetList);
                stream.Close();
            } //using

            ReadPresetList();
        } //WriteMaterialList
    } //class
} //namespace