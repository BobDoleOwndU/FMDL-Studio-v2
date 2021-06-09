using FmdlStudio.Scripts.MonoBehaviours;
using UnityEditor;
using UnityEngine;

namespace FmdlStudio.Editor.MonoBehaviours
{
    [CustomEditor(typeof(FoxModel))]
    [CanEditMultipleObjects]
    public class FoxModelEditor : UnityEditor.Editor
    {
        public int meshGroupsLength;
        public int meshDefinitionsLength;
        private int currentControlID;

        public void OnEnable()
        {
            FoxModel foxModel = target as FoxModel;
            meshGroupsLength = foxModel.meshGroups.Length;
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
                        string[] meshGroupStrings = new string[i + 1];

                        meshGroupStrings[0] = "No Parent";

                        for (int j = 0; j < i; j++)
                            meshGroupStrings[j + 1] = foxModel.meshGroups[j].name;

                        foxModel.meshGroups[i].name = EditorGUILayout.TextField("Name", foxModel.meshGroups[i].name);
                        foxModel.meshGroups[i].parent = (short)(EditorGUILayout.Popup("Parent", foxModel.meshGroups[i].parent + 1, meshGroupStrings) - 1);

                        GUILayout.BeginHorizontal();
                        foxModel.meshGroups[i].visible = EditorGUILayout.Toggle("Is Visible", foxModel.meshGroups[i].visible);

                        if (GUILayout.Button("+", GUILayout.Width(25)))
                        {
                            meshGroupsLength++;
                            foxModel.meshGroups = SetArraySize(foxModel.meshGroups, meshGroupsLength);

                            for(int j = meshGroupsLength - 1; j > i; j--)
                            {
                                foxModel.meshGroups[j] = foxModel.meshGroups[j - 1];

                                if (foxModel.meshGroups[j].parent > i)
                                    foxModel.meshGroups[j].parent++;
                            } //for

                            foxModel.meshGroups[i + 1] = new FoxMeshGroup();

                            serializedObject.Update();
                        } //if

                        if (GUILayout.Button("-", GUILayout.Width(25)))
                        {
                            for (int j = i; j < meshGroupsLength - 1; j++)
                            {
                                foxModel.meshGroups[j] = foxModel.meshGroups[j + 1];

                                if (foxModel.meshGroups[j].parent > i)
                                    foxModel.meshGroups[j].parent--;
                            } //for

                            meshGroupsLength--;
                            foxModel.meshGroups = SetArraySize(foxModel.meshGroups, meshGroupsLength);
                            serializedObject.Update();
                        } //if

                        if (i != 0)
                        {
                            if (GUILayout.Button("▲", GUILayout.Width(25)))
                            {
                                FoxMeshGroup temp = foxModel.meshGroups[i];
                                foxModel.meshGroups[i] = foxModel.meshGroups[i - 1];
                                foxModel.meshGroups[i - 1] = temp;

                                for (int j = 0; j < meshGroupsLength; j++)
                                    if (foxModel.meshGroups[j].parent == i)
                                        foxModel.meshGroups[j].parent--;
                                    else if(foxModel.meshGroups[j].parent == i - 1)
                                        foxModel.meshGroups[j].parent++;

                            } //if
                        } //if

                        if (i != foxModel.meshGroups.Length - 1)
                        {
                            if (GUILayout.Button("▼", GUILayout.Width(25)))
                            {
                                FoxMeshGroup temp = foxModel.meshGroups[i];
                                foxModel.meshGroups[i] = foxModel.meshGroups[i + 1];
                                foxModel.meshGroups[i + 1] = temp;

                                for (int j = 0; j < meshGroupsLength; j++)
                                    if (foxModel.meshGroups[j].parent == i)
                                        foxModel.meshGroups[j].parent++;
                                    else if (foxModel.meshGroups[j].parent == i + 1)
                                        foxModel.meshGroups[j].parent--;
                            } //if
                        } //if
                        GUILayout.EndHorizontal();

                    } //if
                } //for
            } //if
        } //OnInspectorGUI

        private T[] SetArraySize<T>(T[] array, int newLength)
        {
            T[] newArray = new T[newLength];

            for (int i = 0; i < array.Length && i < newLength; i++)
                newArray[i] = array[i];

            return newArray;
        } //SetArraySize
    } //class
} //namespace