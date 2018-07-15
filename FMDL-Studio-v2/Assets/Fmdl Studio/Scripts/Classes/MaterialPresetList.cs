using System.Collections.Generic;
using System.Xml.Serialization;

namespace FmdlStudio.Scripts.Classes
{
    [XmlType("MaterialPresetList")]
    public class MaterialPresetList
    {
        [XmlArray]
        public List<MaterialPreset> materialPresets = new List<MaterialPreset>(0);
    } //MaterialPresetList
} //namespace