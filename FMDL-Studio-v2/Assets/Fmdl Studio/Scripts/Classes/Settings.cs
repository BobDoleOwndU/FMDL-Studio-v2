using System.Xml.Serialization;

namespace FmdlStudio.Scripts.Classes
{
    [XmlType]
    public class Settings
    {
        [XmlElement(ElementName = "TexturePath")]
        public string texturePath;

        [XmlElement(ElementName = "FbxConverterPath")]
        public string fbxConverterPath;

        [XmlElement(ElementName = "fmdlVersion")]
        public float fmdlVersion = 2.04f;
    } //class
} //namespace