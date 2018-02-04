using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FoxModel : MonoBehaviour
{
    public FoxMeshDefinition[] meshDefinitions;

    private void Start()
    {
        if (meshDefinitions == null)
        {
            List<Mesh> meshes = new List<Mesh>(0);

            GetMeshes(transform, meshes);

            meshDefinitions = new FoxMeshDefinition[meshes.Count];

            for(int i = 0; i < meshes.Count; i++)
            {
                meshDefinitions[i] = new FoxMeshDefinition();
                meshDefinitions[i].mesh = meshes[i];
                meshDefinitions[i].meshGroup = meshes[i].name.Substring(meshes[i].name.IndexOf("-") + 2);
            } //for
        } //if
    } //Start

    private void GetMeshes(Transform transform, List<Mesh> meshes)
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
                meshes.Add(t.GetComponent<SkinnedMeshRenderer>().sharedMesh);
        } //foreach
    } //GetMeshes
} //class

[System.Serializable]
public class FoxMeshDefinition
{
    public Mesh mesh;
    public string meshGroup;
} //class