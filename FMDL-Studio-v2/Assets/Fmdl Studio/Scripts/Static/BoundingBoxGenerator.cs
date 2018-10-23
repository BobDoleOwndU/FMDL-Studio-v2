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

            InitializeBoundingBoxes(root);
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

        private static void InitializeBoundingBoxes(Transform transform)
        {
            foreach (Transform t in transform)
            {
                BoxCollider collider = t.gameObject.AddComponent<BoxCollider>();
                Bounds bounds = new Bounds();
                bounds.SetMinMax(new Vector3(-9999f, -9999f, -9999f), new Vector3(9999f, 9999f, 9999f));

                collider.center = t.InverseTransformPoint(bounds.center);
                collider.size = bounds.size;

                InitializeBoundingBoxes(t);
            } //foreach
        } //InitializeBoundingBoxes

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
            BoxCollider collider = bone.gameObject.GetComponent<BoxCollider>();
            Vector3 max = collider.bounds.max;
            Vector3 min = collider.bounds.min;

            if (vertex.x > max.x || max.x == 9999f || float.IsNaN(max.x))
                max.x = vertex.x;
            if (vertex.y > max.y || max.y == 9999f || float.IsNaN(max.y))
                max.y = vertex.y;
            if (vertex.z > max.z || max.z == 9999f || float.IsNaN(max.z))
                max.z = vertex.z;
            if (vertex.x < min.x || min.x == -9999f || float.IsNaN(min.x))
                min.x = vertex.x;
            if (vertex.y < min.y || min.y == -9999f || float.IsNaN(min.y))
                min.y = vertex.y;
            if (vertex.z < min.z || min.z == -9999f || float.IsNaN(min.z))
                min.z = vertex.z;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            collider.center = bone.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
        } //SetBoundingBoxByVertex

        private static void VerifyBoundingBoxes(Transform transform)
        {
            foreach(Transform t in transform)
            {
                BoxCollider collider = t.GetComponent<BoxCollider>();
                Bounds bounds = new Bounds();
                Vector3 min = new Vector3();
                Vector3 max = new Vector3();

                if (collider.bounds.max.x == 9999f)
                    max.x = 0;
                else
                    max.x = collider.bounds.max.x;
                if (collider.bounds.max.y == 9999f)
                    max.y = 0;
                else
                    max.y = collider.bounds.max.y;
                if (collider.bounds.max.z == 9999f)
                    max.z = 0;
                else
                    max.z = collider.bounds.max.z;
                if (collider.bounds.min.x == -9999f)
                    min.x = 0;
                else
                    min.x = collider.bounds.min.x;
                if (collider.bounds.min.y == -9999f)
                    min.y = 0;
                else
                    min.y = collider.bounds.min.y;
                if (collider.bounds.min.z == -9999f)
                    min.z = 0;
                else
                    min.z = collider.bounds.min.z;

                bounds.SetMinMax(min, max);
                collider.center = t.InverseTransformPoint(bounds.center);
                collider.size = bounds.size;

                VerifyBoundingBoxes(t);
            } //foreach
        } //VerifyBoundingBoxes
    } //BoundingBoxGenerator
} //namespace