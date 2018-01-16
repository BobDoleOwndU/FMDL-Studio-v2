using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FoxModel : MonoBehaviour
{
    public FoxMeshDefinition[] meshDefinitions;
    public FoxMaterialDefinition[] materialDefinitions;

    private void Start()
    {
        if (meshDefinitions == null)
        {
            List<Mesh> meshes = new List<Mesh>(0);
            List<Material> materials = new List<Material>(0);

            GetMeshes(transform, meshes, materials);

            meshDefinitions = new FoxMeshDefinition[meshes.Count];
            materialDefinitions = new FoxMaterialDefinition[materials.Count];

            for(int i = 0; i < meshes.Count; i++)
            {
                meshDefinitions[i] = new FoxMeshDefinition();
                meshDefinitions[i].mesh = meshes[i];
                meshDefinitions[i].meshGroup = meshes[i].name.Substring(meshes[i].name.IndexOf("-") + 2);
            } //for

            for(int i = 0; i < materials.Count; i++)
            {
                materialDefinitions[i] = new FoxMaterialDefinition();
                materialDefinitions[i].materialInstance = materials[i];
                materialDefinitions[i].materialName = "fox_3ddf_skin_tension_dirty";
            } //for
        } //if
    } //Start

    private void GetMeshes(Transform transform, List<Mesh> meshes, List<Material> materials)
    {
        foreach (Transform t in transform)
        {
            if (t.GetComponent<MeshRenderer>())
            {
                MeshRenderer renderer = t.GetComponent<MeshRenderer>();
                Mesh mesh = t.gameObject.GetComponent<MeshFilter>().sharedMesh;
                Material material = renderer.sharedMaterial;

                SkinnedMeshRenderer skinnedRenderer = t.gameObject.AddComponent<SkinnedMeshRenderer>();
                skinnedRenderer.sharedMesh = mesh;
                skinnedRenderer.sharedMaterial = material;
            } //if

            if (t.GetComponent<SkinnedMeshRenderer>())
            {
                meshes.Add(t.GetComponent<SkinnedMeshRenderer>().sharedMesh);

                if (materials.IndexOf(t.GetComponent<SkinnedMeshRenderer>().sharedMaterial) == -1)
                    materials.Add(t.GetComponent<SkinnedMeshRenderer>().sharedMaterial);
            } //if
        } //foreach
    } //GetMeshes
} //class

[System.Serializable]
public class FoxMeshDefinition
{
    public Mesh mesh;
    public string meshGroup;
} //class

[System.Serializable]
public class FoxMaterialDefinition
{
    public Material materialInstance;
    public string materialName;
} //cass