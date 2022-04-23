using FmdlStudio.Scripts.MonoBehaviours;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CopyMaterialsWindow : EditorWindow
{
    private const float WIDTH = 300f;
    private const float HEIGHT = 75f;

    private Vector2 scrollPos;
    private GameObject sourceModel;
    private GameObject targetModel;

    [MenuItem("FMDL Studio/Copy Mesh Settings", false, 400)]
    static void Init()
    {
        CopyMaterialsWindow window = (CopyMaterialsWindow)GetWindow(typeof(CopyMaterialsWindow));
        window.Show();
        window.position = new Rect((Screen.currentResolution.width - WIDTH) / 2, (Screen.currentResolution.height - HEIGHT) / 2, WIDTH, HEIGHT);
    } //Init

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Source Model"))
            if (Selection.activeGameObject != null)
                sourceModel = Selection.activeGameObject;

        EditorGUILayout.LabelField(sourceModel == null ? "" : sourceModel.name, EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Target Model"))
            if (Selection.activeGameObject != null)
                targetModel = Selection.activeGameObject;

        EditorGUILayout.LabelField(targetModel == null ? "" : targetModel.name, EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Copy Settings", GUILayout.Width(WIDTH / 2)))
            if (sourceModel != null && sourceModel != null)
                CopySettings();

        EditorGUILayout.EndScrollView();
    } //OnGUI

    private void CopySettings()
    {
        List<SkinnedMeshRenderer> sourceMeshes = GetMeshes(sourceModel);
        List<SkinnedMeshRenderer> targetMeshes = GetMeshes(targetModel);
        int count = Mathf.Min(sourceMeshes.Count, targetMeshes.Count);

        FoxModel foxModel = targetModel.AddComponent<FoxModel>();
        foxModel.meshGroups = sourceModel.GetComponent<FoxModel>().meshGroups;

        for(int i = 0; i < count; i++)
        {
            targetMeshes[i].sharedMaterial = sourceMeshes[i].sharedMaterial;
            FoxMesh targetFoxMesh = targetMeshes[i].gameObject.GetComponent<FoxMesh>();
            FoxMesh sourceFoxMesh = sourceMeshes[i].gameObject.GetComponent<FoxMesh>();

            targetFoxMesh.meshGroup = sourceFoxMesh.meshGroup;
            targetFoxMesh.alpha = sourceFoxMesh.alpha;
            targetFoxMesh.shadow = sourceFoxMesh.shadow;
        } //for
    } //CopySettings

    private List<SkinnedMeshRenderer> GetMeshes(GameObject obj)
    {
        Transform transform = obj.transform;
        List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);

        foreach(Transform t in transform)
        {
            if(t.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(t.GetComponent<SkinnedMeshRenderer>());
            } //if
        } //foreach

        return meshes;
    } //GetMeshes
} //class
