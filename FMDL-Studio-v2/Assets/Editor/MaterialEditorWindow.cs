using UnityEngine;
using UnityEditor;

public class MaterialEditorWindow : EditorWindow
{
    private Vector2 scrollPos;
    private int selected = 0;

    [MenuItem("FMDL Studio/Edit Material Presets", false, 200)]
    static void Init()
    {
        Globals.ReadPresetList();
        MaterialEditorWindow window = (MaterialEditorWindow)GetWindow(typeof(MaterialEditorWindow));
        window.Show();
    } //Init

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);

        if (GUILayout.Button("Save"))
            Globals.WritePresetList();

        if (GUILayout.Button("Add New Preset"))
        {
            MaterialPreset f = new MaterialPreset();
            f.name = "New Preset";

            Globals.materialPresetList.materialPresets.Add(f);
        } //if

        if (GUILayout.Button("Create Preset From Selected Mesh"))
        {
            GameObject gameObject = Selection.activeGameObject;

            if (gameObject != null)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

                if(skinnedMeshRenderer != null)
                {
                    Material material = skinnedMeshRenderer.sharedMaterial;
                    Shader shader = material.shader;

                    MaterialPreset materialPreset = new MaterialPreset();
                    materialPreset.name = gameObject.name + " Material";
                    materialPreset.type = shader.name.Substring(shader.name.IndexOf('/') + 1);

                    int propertyCount = ShaderUtil.GetPropertyCount(shader);

                    for(int i = 0; i < propertyCount; i++)
                    {
                        if(ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.Vector)
                        {
                            MaterialPreset.MaterialPresetParameter materialPresetParameter = new MaterialPreset.MaterialPresetParameter();
                            materialPresetParameter.name = ShaderUtil.GetPropertyName(shader, i);
                            materialPresetParameter.values = material.GetVector(materialPresetParameter.name);

                            materialPreset.materialParameters.Add(materialPresetParameter);
                        } //if
                    } //for

                    Globals.materialPresetList.materialPresets.Add(materialPreset);
                } //if
            } //if
            else
                Debug.Log("No objects selected.");
        } //if

        if (GUILayout.Button("Apply Preset to Selected Mesh"))
            ApplyMaterialToMesh();

        string[] matNames = new string[Globals.materialPresetList.materialPresets.Count];

        for (int i = 0; i < Globals.materialPresetList.materialPresets.Count; i++)
            matNames[i] = Globals.materialPresetList.materialPresets[i].name;
        
        selected = EditorGUILayout.Popup(selected, matNames);

        EditorGUILayout.LabelField(Globals.materialPresetList.materialPresets[selected].name, EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        Globals.materialPresetList.materialPresets[selected].name = EditorGUILayout.TextField("Preset Name", Globals.materialPresetList.materialPresets[selected].name);
        if (GUILayout.Button("Remove"))
        {
            Globals.materialPresetList.materialPresets.Remove(Globals.materialPresetList.materialPresets[selected]);
            selected = 0;
        } //if
        GUILayout.EndHorizontal();

        Globals.materialPresetList.materialPresets[selected].type = EditorGUILayout.TextField("Shader Name", Globals.materialPresetList.materialPresets[selected].type);

        if (GUILayout.Button("Add New Parameter"))
            Globals.materialPresetList.materialPresets[selected].materialParameters.Add(new MaterialPreset.MaterialPresetParameter());

        for (int i = 0; i < Globals.materialPresetList.materialPresets[selected].materialParameters.Count; i++)
        {
            EditorGUILayout.LabelField(Globals.materialPresetList.materialPresets[selected].materialParameters[i].name, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            Globals.materialPresetList.materialPresets[selected].materialParameters[i].name = EditorGUILayout.TextField("Parameter Name", Globals.materialPresetList.materialPresets[selected].materialParameters[i].name);
            if (GUILayout.Button("Remove"))
            {
                Globals.materialPresetList.materialPresets[selected].materialParameters.Remove(Globals.materialPresetList.materialPresets[selected].materialParameters[i]);
                break;
            } //if
            GUILayout.EndHorizontal();

            Globals.materialPresetList.materialPresets[selected].materialParameters[i].values = EditorGUILayout.Vector4Field("Values", Globals.materialPresetList.materialPresets[selected].materialParameters[i].values);
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
                renderer.sharedMaterial.shader = Shader.Find($"FoxShaders/{Globals.materialPresetList.materialPresets[selected].type}");
                //m.name = renderer.sharedMaterial.name;

                foreach(MaterialPreset.MaterialPresetParameter f in Globals.materialPresetList.materialPresets[selected].materialParameters)
                    renderer.sharedMaterial.SetVector(f.name, new Vector4(f.values[0], f.values[1], f.values[2], f.values[3]));
            } //if
            else if(g.GetComponent<SkinnedMeshRenderer>())
            {
                SkinnedMeshRenderer renderer = g.GetComponent<SkinnedMeshRenderer>();
                Material m = new Material(Shader.Find($"FoxShaders/{Globals.materialPresetList.materialPresets[selected].type}"));
                m.name = renderer.sharedMaterial.name;

                foreach (MaterialPreset.MaterialPresetParameter f in Globals.materialPresetList.materialPresets[selected].materialParameters)
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