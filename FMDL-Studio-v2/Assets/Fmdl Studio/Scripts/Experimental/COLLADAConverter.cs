using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Fmdl_Studio.Scripts.Experimental
{
    public static class COLLADAConverter
    {
        public static void ConvertToCOLLADA(string filepath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(COLLADA));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("xmlns", "http://www.collada.org/2008/03/COLLADASchema");

            COLLADA collada = new COLLADA();

            using (FileStream stream = new FileStream(filepath, FileMode.Create))
            {
                try
                {
                    xmlSerializer.Serialize(stream, collada, namespaces);
                    Debug.Log("Export successful! Please note that this does not currently output valid COLLADA files. It is just for debugging purposes.");
                } //try
                finally
                {
                    stream.Close();
                } //finally
            } //using
        } //constructor
    } //COLLADAConverter
} //namespace