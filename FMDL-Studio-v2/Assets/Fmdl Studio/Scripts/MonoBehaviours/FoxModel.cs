using System.Collections.Generic;
using UnityEngine;

namespace FmdlStudio.Scripts.MonoBehaviours
{
    [AddComponentMenu("Fmdl Studio/FoxModel")]
    [ExecuteInEditMode]
    public class FoxModel : MonoBehaviour
    {
        public FoxMeshGroup[] meshGroups;

        private void Awake()
        {
            if (meshGroups == null)
            {
                List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);
                List<string> meshGroupNames = new List<string>(0);

                GetMeshes(transform, meshes, meshGroupNames);

                meshGroups = new FoxMeshGroup[meshGroupNames.Count];

                for (int i = 0; i < meshGroups.Length; i++)
                {
                    meshGroups[i] = new FoxMeshGroup();
                    meshGroups[i].name = meshGroupNames[i];
                    if (i == 0)
                        meshGroups[i].parent = -1;
                    else
                        meshGroups[i].parent = 0;
                } //for

                int meshCount = meshes.Count;

                for (int i = 0; i < meshCount; i++)
                {
                    SkinnedMeshRenderer mesh = meshes[i];
                    FoxMesh foxMesh = mesh.gameObject.AddComponent<FoxMesh>();

                    string name;

                    if (char.IsDigit(mesh.name[0]) && mesh.name.Contains("-"))
                        name = mesh.name.Substring(mesh.name.IndexOf('-') + 2);
                    else
                        name = mesh.name;

                    foxMesh.meshGroup = meshGroupNames.IndexOf(name);
                } //for
            } //if
        } //Start

        private void GetMeshes(Transform transform, List<SkinnedMeshRenderer> meshes, List<string> meshGroupNames)
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
                    SkinnedMeshRenderer mesh = t.GetComponent<SkinnedMeshRenderer>();
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
    public class FoxMeshGroup
    {
        public string name;
        public short parent;
        public bool visible = true;
    } //class
} //namespace