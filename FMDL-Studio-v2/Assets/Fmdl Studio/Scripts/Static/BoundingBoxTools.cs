using System.Collections.Generic;
using UnityEngine;

namespace FmdlStudio.Scripts.Static
{
    public static class BoundingBoxTools
    {
        private static Transform source;
        private static List<Transform> sourceBones = new List<Transform>(0);
        private static List<Transform> targetBones = new List<Transform>(0);


        public static void SetSource(Transform selectedObject)
        {
            foreach (Transform t in selectedObject)
            {
                if (t.gameObject.name == "[Root]")
                {
                    sourceBones.Clear();
                    GetBones(t, sourceBones);
                    break;
                } //if
            } //foreach
        } //GetSource

        public static void SetTarget(Transform selectedObject)
        {
            foreach (Transform t in selectedObject)
            {
                if (t.gameObject.name == "[Root]")
                {
                    targetBones.Clear();
                    GetBones(t, targetBones);
                    break;
                } //if
            } //foreach

            CopyBoundingBoxes();
        } //GetTarget

        public static void ClearBoundingBoxes(Transform selectedObject)
        {
            List<Transform> bones = new List<Transform>(0);

            foreach (Transform t in selectedObject)
            {
                if (t.gameObject.name == "[Root]")
                {
                    GetBones(t, bones);
                    break;
                } //if
            } //foreach

            int boneCount = bones.Count;

            for (int i = 0; i < boneCount; i++)
            {
                Object.DestroyImmediate(bones[i].gameObject.GetComponent<BoxCollider>());
            } //for
        } //CopyBoundingBoxes

        private static void GetBones(Transform bone, List<Transform> bones)
        {
            bones.Add(bone);

            foreach(Transform t in bone)
            {
                GetBones(t, bones);
            } //foreach
        } //GetBones

        private static void CopyBoundingBoxes()
        {
            int boneCount = sourceBones.Count;

            for(int i = 0; i < boneCount; i++)
            {
                BoxCollider sourceCollider = sourceBones[i].gameObject.GetComponent<BoxCollider>();
                BoxCollider targetCollider = targetBones[i].gameObject.AddComponent<BoxCollider>();

                targetCollider.center = sourceCollider.center;
                targetCollider.size = sourceCollider.size;
            } //for
        } //CopyBoundingBoxes
    } //class
} //namespace