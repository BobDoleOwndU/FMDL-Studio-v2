using UnityEngine;

namespace FmdlStudio.Scripts.MonoBehaviours
{
    [AddComponentMenu("Fmdl Studio/FoxMesh")]
    public class FoxMesh : MonoBehaviour
    {
        public enum Alpha { NoAlpha = 0, Glass = 0x10, Glass2 = 0x11, NoBackfaceCulling = 0x20, Glass3 = 0x30, Glass4 = 0x31, Decal = 0x50, Eyelash = 0x70, Parasite = 0x80, Alpha = 0xA0, UnknownOMBS = 0xC0 }
        public enum Shadow { Shadow = 0, NoShadow = 1, InvisibleMeshVisibleShadow = 2, TintedGlass = 4, Glass = 5, LightOMBS = 0x24, GlassOMBS = 0x25, Shadow2 = 0x40, NoShadow2 = 0x41 }
        
        public int meshGroup;
        public Alpha alpha = Alpha.NoAlpha;
        public Shadow shadow = Shadow.Shadow;
    } //class
} //namespace
