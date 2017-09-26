using UnityEngine;

public class BoxDebug : MonoBehaviour {

	// Use this for initialization
	void DrawBox(Transform t) {
		foreach(Transform child in t)
        {
            if (child.GetComponent<BoxCollider>())
            {
                Gizmos.DrawWireCube(child.GetComponent<BoxCollider>().bounds.center, child.GetComponent<BoxCollider>().bounds.size);
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
