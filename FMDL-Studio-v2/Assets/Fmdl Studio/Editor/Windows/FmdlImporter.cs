using System.IO;
using UnityEngine;
using System;
using FmdlStudio.Scripts.Classes;

namespace FmdlStudio.Editor.Windows
{
    public static class FmdlImporter
    {
        public static void FmdlRead(string fmdlPath)
        {
            FileStream stream = new FileStream(fmdlPath, FileMode.Open);

            try
            {
                ExperimentalFmdlImporter experimentalFmdlImporter = new ExperimentalFmdlImporter();
                experimentalFmdlImporter.ReadWithoutAssetImportContext(stream);
            } //try
            catch (Exception e)
            {
                Debug.Log($"{e.Message}");
                Debug.Log($"An exception occured{e.StackTrace}");
                stream.Close();
            } //catch

            stream.Close();
        } //FmdlRead
    } //class
} //namespace