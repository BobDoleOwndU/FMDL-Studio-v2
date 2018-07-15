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

            foreach (Transform t in transform)
            {
                if (t.gameObject.name == "[Root]")
                {
                    BoxCollider collider = t.gameObject.AddComponent<BoxCollider>();
                    Bounds bounds = new Bounds();
                    bounds.SetMinMax(new Vector3(9999f, 9999f, 9999f), new Vector3(-9999f, -9999f, -9999f));

                    collider.center = t.InverseTransformPoint(bounds.center);
                    collider.size = bounds.size;

                    root = t;
                    break;
                } //if
            } //foreach

            InitializeBoundingBoxes(root);
            GetMeshes(transform, meshes);
            SetBoundingBoxByMeshes(meshes);
            SetBoundingBoxByChild(root);
            Debug.Log("Bounding boxes generated!");
        } //GenerateBoundingBoxes

        private static void InitializeBoundingBoxes(Transform transform)
        {
            foreach (Transform t in transform)
            {
                BoxCollider collider = t.gameObject.AddComponent<BoxCollider>();
                Bounds bounds = new Bounds();
                bounds.SetMinMax(new Vector3(9999f, 9999f, 9999f), new Vector3(-9999f, -9999f, -9999f));

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

            if (vertex.x > max.x || max.x == 9999f)
                max.x = vertex.x;
            if (vertex.y > max.y || max.y == 9999f)
                max.y = vertex.y;
            if (vertex.z > max.z || max.z == 9999f)
                max.z = vertex.z;
            if (vertex.x < min.x || min.x == -9999f)
                min.x = vertex.x;
            if (vertex.y < min.y || min.y == -9999f)
                min.y = vertex.y;
            if (vertex.z < min.z || min.z == -9999f)
                min.z = vertex.z;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            collider.center = bone.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
        } //SetBoundingBoxByVertex

        private static void SetBoundingBoxByChild(Transform transform)
        {
            BoxCollider collider = transform.gameObject.GetComponent<BoxCollider>();
            Vector3 max = collider.bounds.max;
            Vector3 min = collider.bounds.min;

            foreach (Transform t in transform)
            {
                SetBoundingBoxByChild(t);
                BoxCollider childCollider = t.gameObject.GetComponent<BoxCollider>();
                Vector3 childMax = childCollider.bounds.max;
                Vector3 childMin = childCollider.bounds.min;

                if (childMax.x > max.x || max.x == 9999f)
                    max.x = childMax.x;
                if (childMax.y > max.y || max.y == 9999f)
                    max.y = childMax.y;
                if (childMax.z > max.z || max.z == 9999f)
                    max.z = childMax.z;
                if (childMin.x < min.x || min.x == -9999f)
                    min.x = childMin.x;
                if (childMin.y < min.y || min.y == -9999f)
                    min.y = childMin.y;
                if (childMin.z < min.z || min.z == -9999f)
                    min.z = childMin.z;
            } //foreach

            if (max.x == 9999f)
                max.x = 0;
            if (max.y == 9999f)
                max.y = 0;
            if (max.z == 9999f)
                max.z = 0;
            if (min.x == -9999f)
                min.x = 0;
            if (min.y == -9999f)
                min.y = 0;
            if (min.z == -9999f)
                min.z = 0;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            collider.center = transform.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
        } //SetBoundingBoxByChild
    } //BoundingBoxGenerator
} //namespace