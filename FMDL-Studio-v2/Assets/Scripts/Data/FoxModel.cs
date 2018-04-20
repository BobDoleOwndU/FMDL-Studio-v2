using System;
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

            for (int i = 0; i < meshes.Count; i++)
            {
                meshDefinitions[i] = new FoxMeshDefinition();
                meshDefinitions[i].mesh = meshes[i];

                if (Char.IsDigit(meshes[i].name[0]) && meshes[i].name.Contains("-"))
                    meshDefinitions[i].meshGroup = meshes[i].name.Substring(meshes[i].name.IndexOf("-") + 2);
                else
                    meshDefinitions[i].meshGroup = meshes[i].name;
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
    public enum Alpha { NoAlpha = 0, Glass = 0x10, Glass2 = 0x11, Glass3 = 0x30, Unknown = 0x20, Eyelash = 0x70, Parasite = 0x80, Alpha = 0xA0 }
    public enum Shadow { Shadow = 0, NoShadow = 1, InvisibleMeshVisibleShadow = 2, TintedGlass = 4, Glass = 5, Shadow2 = 0x40 }

    public Mesh mesh;
    public string meshGroup;
    public Alpha alpha = Alpha.NoAlpha;
    public Shadow shadow = Shadow.Shadow;
} //class