using FmdlStudio.Scripts.Classes;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace FmdlStudio.Scripts.Static
{
    public static class Globals
    {
        private static Settings settings = new Settings();
        public static MaterialPresetList materialPresetList = new MaterialPresetList();
        private static string FmdlPath = "Assets/Fmdl Studio/";

        static Globals()
        {
            ReadSettings();
            ReadPresetList();
        } //constructor

        public static string GetTexturePath()
        {
            return settings.texturePath;
        } //GetTexturePath

        public static void SetTexturePath(string path)
        {
            settings.texturePath = path;

            WriteSettings();
        } //SetTexturePath

        public static string GetFbxConverterPath()
        {
            return settings.fbxConverterPath;
        } //GetFbxConverterPath

        public static void SetFbxConverterPath(string path)
        {
            settings.fbxConverterPath = path;

            WriteSettings();
        } //SetFbxConverterPath

        public static float GetFmdlVersion()
        {
            return settings.fmdlVersion;
        } //GetIsTpp

        public static void SetFmdlVersion(float fmdlVersion)
        {
            settings.fmdlVersion = fmdlVersion;

            WriteSettings();
        } //GetIsTpp

        public static void ReadSettings()
        {
            if (File.Exists(FmdlPath + "settings.xml"))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

                using (FileStream stream = new FileStream(FmdlPath + "settings.xml", FileMode.Open))
                {
                    settings = (Settings)xmlSerializer.Deserialize(stream);
                    stream.Close();
                } //using
            } //if
        } //ReadSettings

        public static void WriteSettings()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));

            using (FileStream stream = new FileStream(FmdlPath + "settings.xml", FileMode.Create))
            {
                xmlSerializer.Serialize(stream, settings);
                stream.Close();
            } //using
        } //WriteSettings

        public static void ReadPresetList()
        {
            if (File.Exists(FmdlPath + "presets.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MaterialPresetList));

                using (FileStream stream = new FileStream(FmdlPath + "presets.xml", FileMode.Open))
                {
                    materialPresetList = (MaterialPresetList)serializer.Deserialize(stream);
                    stream.Close();
                } //using
            } //if
            else
                Debug.Log("Could not find presets.xml");
        } //ReadPresetList

        public static void WritePresetList()
        {
            materialPresetList.materialPresets.Sort((x, y) => x.name.CompareTo(y.name));

            XmlSerializer serializer = new XmlSerializer(typeof(MaterialPresetList));

            using (FileStream stream = new FileStream(FmdlPath + "presets.xml", FileMode.Create))
            {
                serializer.Serialize(stream, materialPresetList);
                stream.Close();
            } //using

            ReadPresetList();
        } //WritePresetList
    } //class
} //namespace