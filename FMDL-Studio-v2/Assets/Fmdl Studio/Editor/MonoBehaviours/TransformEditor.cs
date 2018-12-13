using UnityEditor;
using UnityEngine;


namespace FmdlStudio.Editor.MonoBehaviours
{
    [CustomEditor(typeof(Transform))]
    public class TransformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Transform transform = target as Transform;

            EditorGUILayout.TextField("Local Transforms", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            transform.localPosition = EditorGUILayout.Vector3Field("Position", transform.localPosition);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            transform.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", transform.localEulerAngles);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            transform.localScale = EditorGUILayout.Vector3Field("Scale", transform.localScale);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.TextField("Global Transforms", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            transform.position = EditorGUILayout.Vector3Field("Position", transform.position);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            transform.eulerAngles = EditorGUILayout.Vector3Field("Rotation", transform.eulerAngles);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Vector3Field("Scale (Read-Only)", transform.lossyScale);
            EditorGUILayout.EndHorizontal();

        } //
    } //class
} //namespace
