using UnityEngine;

namespace FmdlStudio.Scripts.MonoBehaviours
{
    [AddComponentMenu("Fmdl Studio/FoxMesh")]
    public class FoxMesh : MonoBehaviour
    {
        //public enum Alpha { NoAlpha = 0, Glass = 0x10, Glass2 = 0x11, NoBackfaceCulling = 0x20, Glass3 = 0x30, Glass4 = 0x31, Decal = 0x50, Eyelash = 0x70, Parasite = 0x80, Alpha = 0xA0, UnknownOMBS = 0xC0 }
        //public enum Shadow { Shadow = 0, NoShadow = 1, InvisibleMeshVisibleShadow = 2, TintedGlass = 4, Glass = 5, LightOMBS = 0x24, GlassOMBS = 0x25, Shadow2 = 0x40, NoShadow2 = 0x41 }
        
        public int meshGroup;
        public BitField alpha;
        public BitField shadow;
        //public Alpha alpha = Alpha.NoAlpha;
        //public Shadow shadow = Shadow.Shadow;
        public bool alphaBit0;
        public bool alphaBit1;
        public bool alphaBit2;
        public bool alphaBit3;
        public bool alphaBit4;
        public bool alphaBit5;
        public bool alphaBit6;
        public bool alphaBit7;

        public bool shadowBit0;
        public bool shadowBit1;
        public bool shadowBit2;
        public bool shadowBit3;
        public bool shadowBit4;
        public bool shadowBit5;
        public bool shadowBit6;
        public bool shadowBit7;
    } //class
} //namespace
