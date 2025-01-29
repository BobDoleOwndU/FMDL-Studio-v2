using FmdlStudio.Scripts.MonoBehaviours;
using UnityEditor;

namespace FmdlStudio.Editor.MonoBehaviours
{
    [CustomEditor(typeof(FoxMesh))]
    [CanEditMultipleObjects]
    public class FoxMeshEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FoxMesh foxMesh = target as FoxMesh;
            FoxModel foxModel = foxMesh.gameObject.transform.parent.GetComponent<FoxModel>();
            string[] meshGroupStrings = new string[foxModel.meshGroups.Length];

            for (int j = 0; j < foxModel.meshGroups.Length; j++)
                meshGroupStrings[j] = foxModel.meshGroups[j].name;
            
            foxMesh.meshGroup = EditorGUILayout.Popup("Mesh Group", foxMesh.meshGroup, meshGroupStrings);
            foxMesh.alpha.SetBit(0, EditorGUILayout.Toggle("Alpha Bit 0", foxMesh.alpha.GetBit(0)));
            foxMesh.alpha.SetBit(1, EditorGUILayout.Toggle("Alpha Bit 1", foxMesh.alpha.GetBit(1)));
            foxMesh.alpha.SetBit(2, EditorGUILayout.Toggle("Alpha Bit 2", foxMesh.alpha.GetBit(2)));
            foxMesh.alpha.SetBit(3, EditorGUILayout.Toggle("Alpha Bit 3", foxMesh.alpha.GetBit(3)));
            foxMesh.alpha.SetBit(4, EditorGUILayout.Toggle("Alpha Bit 4", foxMesh.alpha.GetBit(4)));
            foxMesh.alpha.SetBit(5, EditorGUILayout.Toggle("Disable Backface Culling (5)", foxMesh.alpha.GetBit(5)));
            foxMesh.alpha.SetBit(6, EditorGUILayout.Toggle("Alpha Bit 6", foxMesh.alpha.GetBit(6)));
            foxMesh.alpha.SetBit(7, EditorGUILayout.Toggle("Enable Alpha (7)", foxMesh.alpha.GetBit(7)));
            foxMesh.shadow.SetBit(0, EditorGUILayout.Toggle("Disable Shadow (0)", foxMesh.shadow.GetBit(0)));
            foxMesh.shadow.SetBit(1, EditorGUILayout.Toggle("Hide Mesh (1)", foxMesh.shadow.GetBit(1)));
            foxMesh.shadow.SetBit(2, EditorGUILayout.Toggle("Shadow Bit 2", foxMesh.shadow.GetBit(2)));
            foxMesh.shadow.SetBit(3, EditorGUILayout.Toggle("Shadow Bit 3", foxMesh.shadow.GetBit(3)));
            foxMesh.shadow.SetBit(4, EditorGUILayout.Toggle("Shadow Bit 4", foxMesh.shadow.GetBit(4)));
            foxMesh.shadow.SetBit(5, EditorGUILayout.Toggle("Shadow Bit 5", foxMesh.shadow.GetBit(5)));
            foxMesh.shadow.SetBit(6, EditorGUILayout.Toggle("Shadow Bit 6", foxMesh.shadow.GetBit(6)));
            foxMesh.shadow.SetBit(7, EditorGUILayout.Toggle("Shadow Bit 7", foxMesh.shadow.GetBit(7)));
            //foxMesh.alpha = (FoxMesh.Alpha)EditorGUILayout.EnumPopup("Alpha", foxMesh.alpha);
            //foxMesh.shadow = (FoxMesh.Shadow)EditorGUILayout.EnumPopup("Shadow", foxMesh.shadow);
        } //OnInspectorGUI
    } //class
} //namespace