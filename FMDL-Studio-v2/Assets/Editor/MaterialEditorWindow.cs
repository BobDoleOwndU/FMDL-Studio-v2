using UnityEngine;
using UnityEditor;

public class MaterialEditorWindow : EditorWindow
{
    private Vector2 scrollPos;
    
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
            Globals.foxMaterialList.foxMaterials.Add(new FoxMaterial());
        if (GUILayout.Button("Save"))
            Globals.WriteMaterialList();

        for (int i = 0; i < Globals.foxMaterialList.foxMaterials.Count; i++)
        {
            GUILayout.Label("Material " + i, EditorStyles.boldLabel);

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            Globals.foxMaterialList.foxMaterials[i].name = EditorGUILayout.TextField("Name", Globals.foxMaterialList.foxMaterials[i].name);
            if (GUILayout.Button("Remove"))
            {
                Globals.foxMaterialList.foxMaterials.Remove(Globals.foxMaterialList.foxMaterials[i]);
                break;
            } //if
            GUILayout.EndHorizontal();

            Globals.foxMaterialList.foxMaterials[i].type = EditorGUILayout.TextField("Type", Globals.foxMaterialList.foxMaterials[i].type);

            if (GUILayout.Button("Add New Parameter"))
                Globals.foxMaterialList.foxMaterials[i].materialParameters.Add(new FoxMaterial.FoxMaterialParameter());

            for (int j = 0; j < Globals.foxMaterialList.foxMaterials[i].materialParameters.Count; j++)
            {
                GUILayout.BeginHorizontal();
                Globals.foxMaterialList.foxMaterials[i].materialParameters[j].name = EditorGUILayout.TextField("Parameter Name", Globals.foxMaterialList.foxMaterials[i].materialParameters[j].name);
                if (GUILayout.Button("Remove"))
                {
                    Globals.foxMaterialList.foxMaterials[i].materialParameters.Remove(Globals.foxMaterialList.foxMaterials[i].materialParameters[j]);
                    break;
                } //if
                GUILayout.EndHorizontal();

                for (int h = 0; h < Globals.foxMaterialList.foxMaterials[i].materialParameters[j].values.Length; h++)
                    Globals.foxMaterialList.foxMaterials[i].materialParameters[j].values[h] = EditorGUILayout.FloatField("Parameter " + h, Globals.foxMaterialList.foxMaterials[i].materialParameters[j].values[h]);
            } //for

            GUILayout.EndVertical();
        } //for
        EditorGUILayout.EndScrollView();
    } //OnGUI
} //class