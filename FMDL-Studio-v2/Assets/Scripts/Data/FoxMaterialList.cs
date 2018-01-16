using System.Collections.Generic;
using System.Xml.Serialization;

[XmlType("FoxMaterialList")]
public class FoxMaterialList
{
    [XmlArray]
    public List<FoxMaterial> foxMaterials = new List<FoxMaterial>(0);
} //FoxMaterialList
