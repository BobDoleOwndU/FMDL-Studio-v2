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
            foxMesh.alpha = (FoxMesh.Alpha)EditorGUILayout.EnumPopup("Alpha", foxMesh.alpha);
            foxMesh.shadow = (FoxMesh.Shadow)EditorGUILayout.EnumPopup("Shadow", foxMesh.shadow);
        } //OnInspectorGUI
    } //class
} //namespace