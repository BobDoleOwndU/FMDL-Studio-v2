using System.Collections.Generic;
using UnityEngine;

public static class BoundingBoxGenerator
{
    public static void GenerateBoundingBoxes(Transform transform)
    {
        List<SkinnedMeshRenderer> meshes = GetMeshes(transform);
        List<Transform> bones = new List<Transform>(0);

        foreach(Transform t in transform)
        {
            if(t.gameObject.name == "[Root]")
            {
                GetBones(t, bones);
            } //if
        } //foreach

        /*for(int i = 0; i < bones.Count; i++)
        {
            Bounds bounds = new Bounds();
            BoxCollider collider = new BoxCollider();
            float minX = 9999999999999999999;
            float minY = 9999999999999999999;
            float minZ = 9999999999999999999;
            float maxX = 0;
            float maxY = 0;
            float maxZ = 0;
            bool hasMesh = false;

            for (int j = 0; j < meshes.Count; j++)
            {
                for(int h = 0; h < meshes[j].bones.Length; h++)
                {
                    if(bones[i] == meshes[j].bones[h])
                    {
                        if (meshes[j].bounds.min.x < minX)
                            minX = meshes[j].bounds.min.x;
                        if (meshes[j].bounds.min.y < minY)
                            minY = meshes[j].bounds.min.y;
                        if (meshes[j].bounds.min.z < minZ)
                            minZ = meshes[j].bounds.min.z;
                        if (meshes[j].bounds.max.x > maxX)
                            maxX = meshes[j].bounds.max.x;
                        if (meshes[j].bounds.max.y > maxY)
                            maxY = meshes[j].bounds.max.y;
                        if (meshes[j].bounds.max.z > maxZ)
                            maxZ = meshes[j].bounds.max.z;
                        hasMesh = true;
                        break;
                    } //if
                } //for
            } //for

            if (hasMesh)
            {
                collider = bones[i].gameObject.AddComponent<BoxCollider>();
                bounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
                collider.center = bones[i].InverseTransformPoint(bounds.center);
                collider.size = bounds.size;
            } //if
        } //for*/

        Debug.Log("Bounding boxes generated!");
    } //GenerateBoundingBoxes

    private static List<SkinnedMeshRenderer> GetMeshes(Transform transform)
    {
        List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>(0);

        foreach (Transform t in transform)
            if (t.GetComponent<SkinnedMeshRenderer>())
                meshes.Add(t.GetComponent<SkinnedMeshRenderer>());

        return meshes;
    } //GetMeshes

    public static void GetBones(Transform transform, List<Transform> bones)
    {
        Bounds bounds = new Bounds();
        BoxCollider collider = transform.gameObject.AddComponent<BoxCollider>();
        float minX = transform.position.x;
        float minY = transform.position.y;
        float minZ = transform.position.z;
        float maxX = transform.position.x;
        float maxY = transform.position.y;
        float maxZ = transform.position.z;
        bool hasChildren = false;

        foreach (Transform t in transform)
        {
            hasChildren = true;
            if (t.position.x < minX)
                minX = t.position.x;
            if (t.position.y < minY)
                minY = t.position.y;
            if (t.position.z < minZ)
                minZ = t.position.z;
            if (t.position.x > maxX)
                maxX = t.position.x;
            if (t.position.y > maxY)
                maxY = t.position.y;
            if (t.position.z > maxZ)
                maxZ = t.position.z;

            bones.Add(t);
            GetBones(t, bones);

            if (t.GetComponent<BoxCollider>().bounds.min.x < minX)
                minX = t.GetComponent<BoxCollider>().bounds.min.x;
            if (t.GetComponent<BoxCollider>().bounds.min.y < minY)
                minY = t.GetComponent<BoxCollider>().bounds.min.y;
            if (t.GetComponent<BoxCollider>().bounds.min.z < minZ)
                minZ = t.GetComponent<BoxCollider>().bounds.min.z;
            if (t.GetComponent<BoxCollider>().bounds.max.x > maxX)
                maxX = t.GetComponent<BoxCollider>().bounds.max.x;
            if (t.GetComponent<BoxCollider>().bounds.max.y > maxY)
                maxY = t.GetComponent<BoxCollider>().bounds.max.y;
            if (t.GetComponent<BoxCollider>().bounds.max.z > maxZ)
                maxZ = t.GetComponent<BoxCollider>().bounds.max.z;
        } //foreach

        bounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        collider.center = transform.InverseTransformPoint(bounds.center);

        if (hasChildren)
        {
            collider.size = bounds.size;
        } //if
        else
            collider.size = new Vector3(0.015f, 0.015f, 0.015f);
    } //GetBones
} //BoundingBoxGenerator