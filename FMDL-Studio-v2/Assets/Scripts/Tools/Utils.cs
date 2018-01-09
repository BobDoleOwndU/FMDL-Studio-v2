using UnityEngine;
using System.Collections.Generic;

namespace FmdlStudio
{
    public static class Utils
    {
        public static void GetMeshes(Transform transform, List<SkinnedMeshRenderer> meshes)
        {
            foreach (Transform t in transform)
            {
                if (t.gameObject.GetComponent<SkinnedMeshRenderer>())
                {
                    meshes.Add(t.gameObject.GetComponent<SkinnedMeshRenderer>());
                    GetMeshes(t, meshes);
                } //if
            } //foreach
        } //GetMeshes ends

        public static void GetNumObjects(Transform transform, ref int count)
        {
            foreach (Transform t in transform)
            {
                count++;
                GetNumObjects(t, ref count);
            } //foreach
        } //GetNumObjects
    } //class
}