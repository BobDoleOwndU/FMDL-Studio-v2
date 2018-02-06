using UnityEngine;
using UnityEditor;

public class MaterialEditorWindow : EditorWindow
{
    private Vector2 scrollPos;
    private int selected = 0;

    [MenuItem("FMDL Studio/Edit Materials", false, 200)]
    static void Init()
    {
        Globals.ReadMaterialList();
        MaterialEditorWindow window = (MaterialEditorWindow)GetWindow(typeof(MaterialEditorWindow));
        window.Show();
    } //Init

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);

        if (GUILayout.Button("Add New Material"))
        {
            FoxMaterial f = new FoxMaterial();
            f.name = "New Material";

            Globals.foxMaterialList.foxMaterials.Add(f);
        } //if

        if (GUILayout.Button("Save"))
            Globals.WriteMaterialList();

        if (GUILayout.Button("Apply Material to Selected Mesh"))
            ApplyMaterialToMesh();

        string[] matNames = new string[Globals.foxMaterialList.foxMaterials.Count];

        for (int i = 0; i < Globals.foxMaterialList.foxMaterials.Count; i++)
            matNames[i] = Globals.foxMaterialList.foxMaterials[i].name;
        
        selected = EditorGUILayout.Popup(selected, matNames);

        EditorGUILayout.LabelField(Globals.foxMaterialList.foxMaterials[selected].name, EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        Globals.foxMaterialList.foxMaterials[selected].name = EditorGUILayout.TextField("Name", Globals.foxMaterialList.foxMaterials[selected].name);
        if (GUILayout.Button("Remove"))
        {
            Globals.foxMaterialList.foxMaterials.Remove(Globals.foxMaterialList.foxMaterials[selected]);
            selected = 0;
        } //if
        GUILayout.EndHorizontal();

        Globals.foxMaterialList.foxMaterials[selected].type = EditorGUILayout.TextField("Type", Globals.foxMaterialList.foxMaterials[selected].type);

        if (GUILayout.Button("Add New Parameter"))
            Globals.foxMaterialList.foxMaterials[selected].materialParameters.Add(new FoxMaterial.FoxMaterialParameter());

        for (int i = 0; i < Globals.foxMaterialList.foxMaterials[selected].materialParameters.Count; i++)
        {
            EditorGUILayout.LabelField(Globals.foxMaterialList.foxMaterials[selected].materialParameters[i].name, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            Globals.foxMaterialList.foxMaterials[selected].materialParameters[i].name = EditorGUILayout.TextField("Parameter Name", Globals.foxMaterialList.foxMaterials[selected].materialParameters[i].name);
            if (GUILayout.Button("Remove"))
            {
                Globals.foxMaterialList.foxMaterials[selected].materialParameters.Remove(Globals.foxMaterialList.foxMaterials[selected].materialParameters[i]);
                break;
            } //if
            GUILayout.EndHorizontal();

            Globals.foxMaterialList.foxMaterials[selected].materialParameters[i].values = EditorGUILayout.Vector4Field("Values", Globals.foxMaterialList.foxMaterials[selected].materialParameters[i].values);
        } //for

        EditorGUILayout.EndScrollView();
    } //OnGUI

    private void ApplyMaterialToMesh()
    {
        GameObject g = Selection.activeGameObject;

        if (g != null)
        {
            if(g.GetComponent<MeshRenderer>())
            {
                MeshRenderer renderer = g.GetComponent<MeshRenderer>();
                renderer.sharedMaterial.shader = Shader.Find($"FoxShaders/{Globals.foxMaterialList.foxMaterials[selected].type}");
                //m.name = renderer.sharedMaterial.name;

                foreach(FoxMaterial.FoxMaterialParameter f in Globals.foxMaterialList.foxMaterials[selected].materialParameters)
                    renderer.sharedMaterial.SetVector(f.name, new Vector4(f.values[0], f.values[1], f.values[2], f.values[3]));
            } //if
            else if(g.GetComponent<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer renderer = g.GetComponent<SkinnedMeshRenderer>();
                Material m = new Material(Shader.Find($"FoxShaders/{Globals.foxMaterialList.foxMaterials[selected].type}"));
                m.name = renderer.sharedMaterial.name;

                foreach (FoxMaterial.FoxMaterialParameter f in Globals.foxMaterialList.foxMaterials[selected].materialParameters)
                {
                    m.SetVector(f.name, new Vector4(f.values[0], f.values[1], f.values[2], f.values[3]));
                } //foreach

                renderer.sharedMaterial = m;
            } //if
            else
                Debug.Log("Game object does not contain a MeshRenderer or SkinnedMeshRenderer.");
        } //if
        else
            Debug.Log("No game object selected.");
    } //ApplyMaterialToMesh
} //class