using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDebug : MonoBehaviour {

	// Use this for initialization
	void DrawBox(Transform t) {
		foreach(Transform child in t)
        {
            if (child.GetComponent<SkinnedMeshRenderer>())
            {
                Gizmos.DrawWireCube(child.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds.center, child.GetComponent<SkinnedMeshRenderer>().sharedMesh.bounds.size);
                DrawBox(child);
            } //if
        } //foreach
	} //DrawBox
	
	// Update is called once per frame
	void Update() {
		
	}

    void OnDrawGizmos()
    {
        DrawBox(transform);
    } //OnDrawGizmos
}
