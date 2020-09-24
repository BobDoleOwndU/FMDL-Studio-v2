using System.Collections.Generic;
using UnityEngine;

namespace FmdlStudio.Scripts.Static
{
    public static class BoundingBoxGenerator
    {
        public static void GenerateBoundingBoxes(Transform transform)
        {
            List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);
            Transform root = transform;
            Vector3 absoluteMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 absoluteMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            int meshCount;

            foreach (Transform t in transform)
            {
                if (t.gameObject.name == "[Root]")
                {
                    root = t;
                    break;
                } //if
            } //foreach
            
            GetMeshes(transform, meshes);
            SetBoundingBoxByMeshes(meshes);
            VerifyBoundingBoxes(root);

            meshCount = meshes.Count;

            for(int i = 0; i < meshCount; i++)
            {
                Mesh mesh = meshes[i].sharedMesh;
                Vector3 max = mesh.vertices.Max();
                Vector3 min = mesh.vertices.Min();

                if (absoluteMax.x < max.x)
                    absoluteMax.x = max.x;
                if (absoluteMax.y < max.y)
                    absoluteMax.y = max.y;
                if (absoluteMax.z < max.z)
                    absoluteMax.z = max.z;
                if (absoluteMin.x > min.x)
                    absoluteMin.x = min.x;
                if (absoluteMin.y > min.y)
                    absoluteMin.y = min.y;
                if (absoluteMin.z > min.z)
                    absoluteMin.z = min.z;
            } //for

            BoxCollider collider = root.gameObject.AddComponent<BoxCollider>();
            Bounds bounds = new Bounds();
            bounds.SetMinMax(absoluteMin, absoluteMax);
            collider.center = root.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;

            Debug.Log("Bounding boxes generated!");
        } //GenerateBoundingBoxes

        private static void GetMeshes(Transform transform, List<SkinnedMeshRenderer> meshes)
        {
            foreach (Transform t in transform)
            {
                if (t.GetComponent<SkinnedMeshRenderer>())
                    meshes.Add(t.GetComponent<SkinnedMeshRenderer>());
            } //foreach
        } //GetMeshes

        private static void SetBoundingBoxByMeshes(List<SkinnedMeshRenderer> meshes)
        {
            int meshCount = meshes.Count;

            for (int i = 0; i < meshCount; i++)
            {
                SkinnedMeshRenderer mesh = meshes[i];
                BoneWeight[] weights = mesh.sharedMesh.boneWeights;
                Transform[] meshBones = mesh.bones;
                Vector3[] vertices = mesh.sharedMesh.vertices;

                int weightLength = weights.Length;

                for (int j = 0; j < weightLength; j++)
                {
                    Vector3 vertex = vertices[j];

                    SetBoundingBoxByVertex(meshBones[weights[j].boneIndex0], vertex);
                    SetBoundingBoxByVertex(meshBones[weights[j].boneIndex1], vertex);
                    SetBoundingBoxByVertex(meshBones[weights[j].boneIndex2], vertex);
                    SetBoundingBoxByVertex(meshBones[weights[j].boneIndex3], vertex);
                } //for
            } //for
        } //SetBoundingBoxByMeshes

        private static void SetBoundingBoxByVertex(Transform bone, Vector3 vertex)
        {
            BoxCollider collider;
            Vector3 max;
            Vector3 min;

            if (bone.gameObject.GetComponent<BoxCollider>())
            {
                collider = bone.gameObject.GetComponent<BoxCollider>();
                max = collider.bounds.max;
                min = collider.bounds.min;

                if (vertex.x > max.x || float.IsNaN(max.x))
                    max.x = vertex.x;
                if (vertex.y > max.y || float.IsNaN(max.y))
                    max.y = vertex.y;
                if (vertex.z > max.z || float.IsNaN(max.z))
                    max.z = vertex.z;
                if (vertex.x < min.x || float.IsNaN(min.x))
                    min.x = vertex.x;
                if (vertex.y < min.y || float.IsNaN(min.y))
                    min.y = vertex.y;
                if (vertex.z < min.z || float.IsNaN(min.z))
                    min.z = vertex.z;
            } //if
            else
            {
                collider = bone.gameObject.AddComponent<BoxCollider>();
                max = new Vector3(vertex.x, vertex.y, vertex.z);
                min = new Vector3(vertex.x, vertex.y, vertex.z);
            } // else

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            collider.center = bone.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
        } //SetBoundingBoxByVertex

        private static void VerifyBoundingBoxes(Transform transform)
        {
            foreach(Transform t in transform)
            {
                if (!t.GetComponent<BoxCollider>())
                {
                    BoxCollider collider = t.gameObject.AddComponent<BoxCollider>();
                    Bounds bounds = new Bounds();
                    Vector3 min = new Vector3(-0.01f, -0.01f, -0.01f);
                    Vector3 max = new Vector3(0.01f, 0.01f, 0.01f);

                    bounds.SetMinMax(min, max);
                    collider.center = t.InverseTransformPoint(bounds.center);
                    collider.size = bounds.size;
                } //if

                VerifyBoundingBoxes(t);
            } //foreach
        } //VerifyBoundingBoxes
    } //BoundingBoxGenerator
} //namespace