using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using FmdlStudio.Scripts.Classes;

namespace FmdlStudio.Editor.Windows
{
    public static class FmdlExporter
    {
        public static void FmdlWrite(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create);

            try
            {
                GameObject selectedGameObject = Selection.activeGameObject;
                ExpFmdl fmdl = new ExpFmdl(selectedGameObject.name);
                fmdl.Write(selectedGameObject, stream);
            } //try
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.Log($"{e.Message} The stream was at offset 0x{stream.Position.ToString("x")} when this exception occured.");
                Debug.Log($"An exception occured{e.StackTrace}");
                stream.Close();
            } //catch

            stream.Close();
        } //FmdlWrite
    } //class
} //namespace