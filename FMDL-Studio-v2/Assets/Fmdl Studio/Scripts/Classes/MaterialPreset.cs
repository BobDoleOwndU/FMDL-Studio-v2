using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace FmdlStudio.Scripts.Classes
{
    [XmlType("MaterialPreset")]
    public class MaterialPreset
    {
        [XmlType("MaterialPresetParameter")]
        public class MaterialPresetParameter
        {
            [XmlElement]
            public string name;

            [XmlElement]
            public Vector4 values;
        } //class

        [XmlElement]
        public string name;

        [XmlElement]
        public string type;

        [XmlArray]
        public List<MaterialPresetParameter> materialParameters = new List<MaterialPresetParameter>();
    } //class
} //namespace