
using UnityEngine;
using FmdlStudio.Scripts.MonoBehaviours;
using FmdlStudio.Scripts.Static;

namespace FmdlStudio.Scripts.Classes
{
    public class SkinnedMeshChunk
    {
        // The GameObject of this chunk.
        public GameObject gameObject;

        // The SkinnedMeshRenderer component of this chunk.
        public SkinnedMeshRenderer skinnedMeshRenderer;

        // The FoxMesh component of this chunk.
        public FoxMesh foxMesh;

        private int bonesCountBudget;

        // The mesh associated with this chunk
        public Mesh Mesh { get { return skinnedMeshRenderer.sharedMesh; } }
        public int BonesCount { get { return skinnedMeshRenderer.bones.Length; } }

        // Is the mesh within the allowed bone limit. If not this chunk needs to be split into sub-chunks until it can be exported.
        public bool WithinBonesCountBudget { get { return BonesCount <= bonesCountBudget; } }

        public SkinnedMeshChunk(SkinnedMeshRenderer skinnedMeshRendererSource, Mesh mesh, int id, int bonesCountBudget)
        {
            string chunkName = skinnedMeshRendererSource.gameObject.name + "_c" + id;

            mesh.name = chunkName;

            this.bonesCountBudget = bonesCountBudget;

            // prepare gameObject
            this.gameObject = new GameObject(chunkName);
            this.gameObject.transform.SetParent(skinnedMeshRendererSource.gameObject.transform.parent);

            // prepare skinnedMeshRenderer
            this.skinnedMeshRenderer = this.gameObject.AddComponent<SkinnedMeshRenderer>();
            this.skinnedMeshRenderer.sharedMaterials = skinnedMeshRendererSource.sharedMaterials;
            this.skinnedMeshRenderer.bones = skinnedMeshRendererSource.bones;
            this.skinnedMeshRenderer.rootBone = skinnedMeshRendererSource.rootBone;

            this.skinnedMeshRenderer.sharedMesh = mesh;
            this.skinnedMeshRenderer.localBounds = mesh.bounds;

            if (!WithinBonesCountBudget)
            {
                string logBones = "Mesh (" + mesh.name + ") exceeds bones count budget of " + bonesCountBudget + " (" + BonesCount + ").";
                SkinnedMeshChunkGenerator.RemoveUnusedBones(this);
                Debug.Log(logBones + " After removing unused bones the mesh is " + (WithinBonesCountBudget ? "within" : "still over") + " budget (" + BonesCount + ").");
            }

            // prepare foxMesh
            FoxMesh foxMeshSource = skinnedMeshRendererSource.GetComponent<FoxMesh>();
            if (foxMeshSource)
            {
                this.foxMesh = this.gameObject.AddComponent<FoxMesh>();
                this.foxMesh.meshGroup = foxMeshSource.meshGroup;
                this.foxMesh.alpha = foxMeshSource.alpha;
                this.foxMesh.shadow = foxMeshSource.shadow;
            }
        }

        // Destroy the GameObject and clear all internal references.
        public void DestroyAndClear()
        {
            Object.DestroyImmediate(gameObject);
            gameObject = null;
            skinnedMeshRenderer = null;
            foxMesh = null;
        }
    }
}
