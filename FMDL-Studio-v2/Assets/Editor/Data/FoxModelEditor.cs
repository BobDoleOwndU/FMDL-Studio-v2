using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FoxModel))]
[CanEditMultipleObjects]
public class FoxModelEditor : Editor
{
    public int meshGroupsLength;
    public int meshDefinitionsLength;
    private int currentControlID;

    public void OnEnable()
    {
        FoxModel foxModel = target as FoxModel;
        meshGroupsLength = foxModel.meshGroups.Length;
        meshDefinitionsLength = foxModel.meshDefinitions.Length;
        currentControlID = GUIUtility.keyboardControl;
    } //OnEnable

    public override void OnInspectorGUI()
    {
        FoxModel foxModel = target as FoxModel;
        serializedObject.Update();

        serializedObject.FindProperty("meshGroups").isExpanded = EditorGUILayout.Foldout(serializedObject.FindProperty("meshGroups").isExpanded, "Mesh Groups", true);

        if (serializedObject.FindProperty("meshGroups").isExpanded)
        {
            meshGroupsLength = EditorGUILayout.IntField("Size", meshGroupsLength);

            if (currentControlID != GUIUtility.keyboardControl)
            {
                foxModel.meshGroups = SetArraySize(foxModel.meshGroups, meshGroupsLength);
                currentControlID = GUIUtility.keyboardControl;
                serializedObject.Update();
            } //if

            for (int i = 0; i < foxModel.meshGroups.Length; i++)
            {
                serializedObject.FindProperty("meshGroups").GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.Foldout(serializedObject.FindProperty("meshGroups").GetArrayElementAtIndex(i).isExpanded, $"Element {i}", true);

                if (serializedObject.FindProperty("meshGroups").GetArrayElementAtIndex(i).isExpanded)
                {
                    foxModel.meshGroups[i].name = EditorGUILayout.TextField("Name", foxModel.meshGroups[i].name);
                    foxModel.meshGroups[i].visible = EditorGUILayout.Toggle("Is Visible", foxModel.meshGroups[i].visible);
                } //if
            } //for
        } //if

        serializedObject.FindProperty("meshDefinitions").isExpanded = EditorGUILayout.Foldout(serializedObject.FindProperty("meshDefinitions").isExpanded, "Mesh Definitions", true);

        if (serializedObject.FindProperty("meshDefinitions").isExpanded)
        {
            meshDefinitionsLength = EditorGUILayout.IntField("Size", meshDefinitionsLength);

            if (currentControlID != GUIUtility.keyboardControl)
            {
                foxModel.meshDefinitions = SetArraySize(foxModel.meshDefinitions, meshDefinitionsLength);
                currentControlID = GUIUtility.keyboardControl;
                serializedObject.Update();
            } //if

            for (int i = 0; i < foxModel.meshDefinitions.Length; i++)
            {
                serializedObject.FindProperty("meshDefinitions").GetArrayElementAtIndex(i).isExpanded = EditorGUILayout.Foldout(serializedObject.FindProperty("meshDefinitions").GetArrayElementAtIndex(i).isExpanded, $"Element {i}", true);

                if (serializedObject.FindProperty("meshDefinitions").GetArrayElementAtIndex(i).isExpanded)
                {
                    string[] meshGroupStrings = new string[foxModel.meshGroups.Length];

                    for (int j = 0; j < foxModel.meshGroups.Length; j++)
                        meshGroupStrings[j] = foxModel.meshGroups[j].name;

                    foxModel.meshDefinitions[i].mesh = (Mesh)EditorGUILayout.ObjectField("Mesh", foxModel.meshDefinitions[i].mesh, typeof(Mesh), true);
                    foxModel.meshDefinitions[i].meshGroup = EditorGUILayout.Popup("Mesh Group", foxModel.meshDefinitions[i].meshGroup, meshGroupStrings);
                    foxModel.meshDefinitions[i].alpha = (FoxMeshDefinition.Alpha)EditorGUILayout.EnumPopup("Alpha", foxModel.meshDefinitions[i].alpha);
                    foxModel.meshDefinitions[i].shadow = (FoxMeshDefinition.Shadow)EditorGUILayout.EnumPopup("Shadow", foxModel.meshDefinitions[i].shadow);
                } //if
            } //for
        } //if
    } //OnInspectorGUI

    private T[] SetArraySize<T>(T[] array, int newLength)
    {
        T[] newArray = new T[newLength];

        for(int i = 0; i < array.Length && i < newLength; i++)
            newArray[i] = array[i];

        return newArray;
    } //SetArraySize
} //class
