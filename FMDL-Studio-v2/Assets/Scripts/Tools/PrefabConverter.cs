using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unfinished/broken class! Do not use!
/// </summary>
public static class PrefabConverter
{
    public static void ConvertToPrefab(GameObject gameObject)
    {
        List<Mesh> meshes = new List<Mesh>(0);
        List<Material> materials = new List<Material>(0);
        List<Texture> textures = new List<Texture>(0);

        GetMeshes(gameObject.transform, meshes, materials, textures);

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder($"Assets/Prefabs/{gameObject.name}"))
            AssetDatabase.CreateFolder("Assets/Prefabs", gameObject.name);

        int numMeshes = meshes.Count;
        int numMaterials = materials.Count;
        int numTextures = textures.Count;

        EditorUtility.DisplayProgressBar("Converting to Prefab!", "Starting", 0);

        for (int i = 0; i < numMeshes; i++)
        {
            EditorUtility.DisplayProgressBar("Converting to Prefab!", $"Meshes: {i}/{numMeshes}", (float)i / numMeshes);

            string meshPath = $"Assets/Prefabs/{gameObject.name}/mesh_{i}.asset";
            AssetDatabase.CreateAsset(meshes[i], meshPath);
        } //foreach

        for (int i = 0; i < numTextures; i++)
        {
            EditorUtility.DisplayProgressBar("Converting to Prefab!", $"Textures: {i}/{numTextures}", (float)i / numTextures);

            string[] textureFolders = textures[i].name.Split('/');
            int numTextureFolders = textureFolders.Length;

            for (int j = 1; j < numTextureFolders - 1; j++)
            {
                string folderName = $"Assets/Prefabs/{gameObject.name}";

                for (int h = 1; h < j; h++)
                    if (textureFolders[h][0] != '/')
                        folderName += $"/{textureFolders[h]}";
                    else
                        folderName += $"{textureFolders[h]}";

                if (!AssetDatabase.IsValidFolder($"{folderName}/{textureFolders[j]}"))
                    AssetDatabase.CreateFolder(folderName, textureFolders[j]);
            } //for

            string textureNameWithoutPath = textures[i].name.Substring(0, textures[i].name.IndexOf('.'));

            string texturePath = $"Assets/Prefabs/{gameObject.name}/{textureNameWithoutPath}.asset";
            AssetDatabase.CreateAsset(textures[i], texturePath);
        } //for

        for (int i = 0; i < numMaterials; i++)
        {
            EditorUtility.DisplayProgressBar("Converting to Prefab!", $"Materials: {i}/{numMaterials}", (float)i / numMaterials);

            string materialPath = $"Assets/Prefabs/{gameObject.name}/{materials[i].name}.asset";
            AssetDatabase.CreateAsset(materials[i], materialPath);
        } //foreach

        EditorUtility.DisplayProgressBar("Converting to Prefab!", $"Prefab: 0/1", 0);

        string prefabPath = $"Assets/Prefabs/{gameObject.name}/{gameObject.name}.prefab";
        PrefabUtility.CreatePrefab(prefabPath, gameObject, ReplacePrefabOptions.ConnectToPrefab);

        EditorUtility.ClearProgressBar();
    } //ConvertToPrefab

    private static void GetMeshes(Transform transform, List<Mesh> meshes, List<Material> materials, List<Texture> textures)
    {
        foreach (Transform t in transform)
        {
            if (t.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(t.GetComponent<SkinnedMeshRenderer>().sharedMesh);

                Material material = t.GetComponent<SkinnedMeshRenderer>().sharedMaterial;

                if (!materials.Contains(material))
                {
                    materials.Add(material);

                    int propertyCount = ShaderUtil.GetPropertyCount(material.shader);

                    for (int i = 0; i < propertyCount; i++)
                        if (ShaderUtil.GetPropertyType(material.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            Texture texture = material.GetTexture(ShaderUtil.GetPropertyName(material.shader, i));

                            if (!textures.Contains(texture))
                                textures.Add(texture);
                        } //if
                } //if
            } //if
        } //foreach
    } //GetMeshes
} //class