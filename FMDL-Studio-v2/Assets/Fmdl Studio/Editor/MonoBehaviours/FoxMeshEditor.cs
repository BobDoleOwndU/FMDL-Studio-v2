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
            foxMesh.flags.SetBit(0, EditorGUILayout.Toggle("Blend Mode: Alpha Blend", foxMesh.flags.GetBit(0)));
            foxMesh.flags.SetBit(1, EditorGUILayout.Toggle("Blend Mode: Additive", foxMesh.flags.GetBit(1)));
            //foxMesh.flags.SetBit(2, EditorGUILayout.Toggle("Blend Mode: Unused", foxMesh.flags.GetBit(2)));
            //foxMesh.flags.SetBit(3, EditorGUILayout.Toggle("Blend Mode: Unused", foxMesh.flags.GetBit(3)));
            foxMesh.flags.SetBit(4, EditorGUILayout.Toggle("Blend", foxMesh.flags.GetBit(4)));
            foxMesh.flags.SetBit(5, EditorGUILayout.Toggle("Two Sided", foxMesh.flags.GetBit(5)));
            foxMesh.flags.SetBit(6, EditorGUILayout.Toggle("No Depth Write", foxMesh.flags.GetBit(6)));
            foxMesh.flags.SetBit(7, EditorGUILayout.Toggle("Alpha Test", foxMesh.flags.GetBit(7)));
            foxMesh.flags.SetBit(8, EditorGUILayout.Toggle("No Shadow", foxMesh.flags.GetBit(8)));
            foxMesh.flags.SetBit(9, EditorGUILayout.Toggle("Shadow Only", foxMesh.flags.GetBit(9)));
            foxMesh.flags.SetBit(10, EditorGUILayout.Toggle("Forward", foxMesh.flags.GetBit(10)));
            //foxMesh.flags.SetBit(11, EditorGUILayout.Toggle("Unused", foxMesh.flags.GetBit(11)));
            //foxMesh.flags.SetBit(12, EditorGUILayout.Toggle("Unused", foxMesh.flags.GetBit(12)));
            foxMesh.flags.SetBit(13, EditorGUILayout.Toggle("B", foxMesh.flags.GetBit(13)));
            foxMesh.flags.SetBit(14, EditorGUILayout.Toggle("Alpha Test Alt", foxMesh.flags.GetBit(14)));
            foxMesh.flags.SetBit(15, EditorGUILayout.Toggle("Alpha Sort First", foxMesh.flags.GetBit(15)));

            foxMesh.flags.SetBit(16, EditorGUILayout.Toggle("Reject Size Level 0", foxMesh.flags.GetBit(16)));
            foxMesh.flags.SetBit(17, EditorGUILayout.Toggle("Reject Size Level 1", foxMesh.flags.GetBit(17)));
            foxMesh.flags.SetBit(18, EditorGUILayout.Toggle("Reject Size Level 2", foxMesh.flags.GetBit(18)));
            foxMesh.flags.SetBit(19, EditorGUILayout.Toggle("Reject Size Level 3", foxMesh.flags.GetBit(19)));
            foxMesh.flags.SetBit(20, EditorGUILayout.Toggle("Decal Priority 1", foxMesh.flags.GetBit(20)));
            foxMesh.flags.SetBit(21, EditorGUILayout.Toggle("Decal Priority 2", foxMesh.flags.GetBit(21)));
            foxMesh.flags.SetBit(22, EditorGUILayout.Toggle("Decal Priority 3", foxMesh.flags.GetBit(22)));
            foxMesh.flags.SetBit(23, EditorGUILayout.Toggle("Decal Priority 4", foxMesh.flags.GetBit(23)));
            foxMesh.flags.SetBit(24, EditorGUILayout.Toggle("Decal Priority 5", foxMesh.flags.GetBit(24)));
            foxMesh.flags.SetBit(25, EditorGUILayout.Toggle("Decal Priority 6", foxMesh.flags.GetBit(25)));
            foxMesh.flags.SetBit(26, EditorGUILayout.Toggle("Decal Priority 7", foxMesh.flags.GetBit(26)));
            /*foxMesh.flags.SetBit(27, EditorGUILayout.Toggle("Unknown", foxMesh.flags.GetBit(27)));
            foxMesh.flags.SetBit(28, EditorGUILayout.Toggle("Unknown", foxMesh.flags.GetBit(28)));
            foxMesh.flags.SetBit(29, EditorGUILayout.Toggle("Unknown", foxMesh.flags.GetBit(29)));
            foxMesh.flags.SetBit(30, EditorGUILayout.Toggle("Unknown", foxMesh.flags.GetBit(30)));
            foxMesh.flags.SetBit(31, EditorGUILayout.Toggle("Unknown", foxMesh.flags.GetBit(31)));*/
        } //OnInspectorGUI
    } //class
} //namespace