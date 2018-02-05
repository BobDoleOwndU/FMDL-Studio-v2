using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlType("FoxMaterial")]
public class FoxMaterial
{
    [XmlType("FoxMaterialParameter")]
    public class FoxMaterialParameter
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
    public List<FoxMaterialParameter> materialParameters = new List<FoxMaterialParameter>();
} //FoxMaterial
