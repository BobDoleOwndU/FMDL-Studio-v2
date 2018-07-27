using UnityEngine;
using System.Collections.Generic;

namespace FmdlStudio.Scripts.Static
{
    public static class Utils
    {
        public static void FixTangents(Transform transform)
        {
            List<Mesh> meshes = new List<Mesh>(0);
            GetMeshes(transform, meshes);

            for (int i = 0; i < meshes.Count; i++)
            {
                Vector4[] tangents = meshes[i].tangents;

                for (int j = 0; j < tangents.Length; j++)
                    tangents[j].w *= -1;

                meshes[i].tangents = tangents;
            } //for
        } //FixTangents

        private static void GetMeshes(Transform transform, List<Mesh> meshes)
        {
            foreach (Transform t in transform)
            {
                if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
                {
                    meshes.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh);
                    GetMeshes(t, meshes);
                } //if
            } //foreach
        } //GetMeshes ends
    } //class
} //namespace