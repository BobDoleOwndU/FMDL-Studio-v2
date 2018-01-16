using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType("FoxMaterial")]
public class FoxMaterial
{
    [XmlType("FoxMaterialParameter")]
    public class FoxMaterialParameter
    {
        [XmlElement]
        public string name;

        [XmlElement]
        public float[] values = new float[4];
    } //class

    [XmlElement]
    public string name;

    [XmlElement]
    public string type;

    [XmlArray]
    public List<FoxMaterialParameter> materialParameters = new List<FoxMaterialParameter>();
} //FoxMaterial
