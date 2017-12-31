using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FoxModel : MonoBehaviour
{
    public FoxMeshDefinition[] definitions;

    private void Start()
    {
        if (definitions == null)
        {
            List<Mesh> meshes = GetMeshes(transform);

            definitions = new FoxMeshDefinition[meshes.Count];

            for(int i = 0; i < meshes.Count; i++)
            {
                definitions[i] = new FoxMeshDefinition();
                definitions[i].mesh = meshes[i];
                definitions[i].meshGroup = meshes[i].name.Substring(4);
                definitions[i].material = "fox_3ddf_skin_tension_dirty";
                definitions[i].materialType = "fox3DDF_Skin_Tension_Dirty";
            } //for
        } //if
    } //Start

    private List<Mesh> GetMeshes(Transform transform)
    {
        List<Mesh> meshes = new List<Mesh>(0);

        foreach (Transform t in transform)
        {
            if (t.GetComponent<MeshRenderer>())
            {
                MeshRenderer renderer = t.GetComponent<MeshRenderer>();
                Mesh mesh = t.gameObject.GetComponent<MeshFilter>().sharedMesh;
                Material material = renderer.sharedMaterial;

                SkinnedMeshRenderer skinnedRenderer = t.gameObject.AddComponent<SkinnedMeshRenderer>();
                skinnedRenderer.sharedMesh = mesh;
                skinnedRenderer.material = material;
            } //if

            if (t.GetComponent<SkinnedMeshRenderer>())
                meshes.Add(t.GetComponent<SkinnedMeshRenderer>().sharedMesh);
        } //foreach

        return meshes;
    } //GetMeshes
} //class

[System.Serializable]
public class FoxMeshDefinition
{
    public Mesh mesh;
    public string meshGroup;
    public string material;
    public string materialType;
} //class