using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FoxModel : MonoBehaviour
{
    public FoxMeshGroup[] meshGroups;
    public FoxMeshDefinition[] meshDefinitions;

    private void Awake()
    {
        if (meshDefinitions == null)
        {
            List<Mesh> meshes = new List<Mesh>(0);
            List<string> meshGroupNames = new List<string>(0);

            GetMeshes(transform, meshes, meshGroupNames);

            meshGroups = new FoxMeshGroup[meshGroupNames.Count];

            for (int i = 0; i < meshGroups.Length; i++)
            {
                meshGroups[i] = new FoxMeshGroup();
                meshGroups[i].name = meshGroupNames[i];
            } //for

            meshDefinitions = new FoxMeshDefinition[meshes.Count];

            for (int i = 0; i < meshDefinitions.Length; i++)
            {
                meshDefinitions[i] = new FoxMeshDefinition();
                meshDefinitions[i].mesh = meshes[i];

                string name;

                if (char.IsDigit(meshDefinitions[i].mesh.name[0]) && meshDefinitions[i].mesh.name.Contains("-"))
                    name = meshDefinitions[i].mesh.name.Substring(meshDefinitions[i].mesh.name.IndexOf('-') + 2);
                else
                    name = meshDefinitions[i].mesh.name;

                meshDefinitions[i].meshGroup = meshGroupNames.IndexOf(name);
            } //for
        } //if
    } //Start

    private void GetMeshes(Transform transform, List<Mesh> meshes, List<string> meshGroupNames)
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
                Mesh mesh = t.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                string name;

                if (char.IsDigit(mesh.name[0]) && mesh.name.Contains("-"))
                    name = mesh.name.Substring(mesh.name.IndexOf('-') + 2);
                else
                    name = mesh.name;

                if (!meshGroupNames.Contains(name))
                    meshGroupNames.Add(name);

                meshes.Add(mesh);
            } //if
        } //foreach
    } //GetMeshes
} //class

[System.Serializable]
public class FoxMeshDefinition
{
    public enum Alpha { NoAlpha = 0, Glass = 0x10, Glass2 = 0x11, NoBackfaceCulling = 0x20, Glass3 = 0x30, Glass4 = 0x31, Decal = 0x50, Eyelash = 0x70, Parasite = 0x80, Alpha = 0xA0 }
    public enum Shadow { Shadow = 0, NoShadow = 1, InvisibleMeshVisibleShadow = 2, TintedGlass = 4, Glass = 5, Shadow2 = 0x40, NoShadow2 = 0x41 }

    public Mesh mesh;
    public int meshGroup;
    public Alpha alpha = Alpha.NoAlpha;
    public Shadow shadow = Shadow.Shadow;
} //class

[System.Serializable]
public class FoxMeshGroup
{
    public string name;
    public bool visible = true;
} //class