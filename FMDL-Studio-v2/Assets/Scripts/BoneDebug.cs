using UnityEngine;
using System.Collections;

namespace FmdlStudio
{
    public class BoneDebug : MonoBehaviour
    {
        void drawbone(Transform t)
        {
            //renderer.SetWidth(5.0, 2.0);
            foreach (Transform child in t)
            {
                float len = 0.05f;
                Vector3 loxalX = new Vector3(len, 0, 0);
                Vector3 loxalY = new Vector3(0, len, 0);
                Vector3 loxalZ = new Vector3(0, 0, len);
                loxalX = child.rotation * loxalX;
                loxalY = child.rotation * loxalY;
                loxalZ = child.rotation * loxalZ;

                Gizmos.DrawLine(t.position * 0.1f + child.position * 0.9f, t.position * 0.9f + child.position * 0.1f);

                Gizmos.DrawLine(child.position, child.position + loxalX);
                Gizmos.DrawLine(child.position, child.position + loxalY);
                Gizmos.DrawLine(child.position, child.position + loxalZ);
                drawbone(child);
            }
            //LineRenderer.SetWidth(1.0, 1.0);
        }
        void Update()
        {
        }
        void OnDrawGizmos()
        {
            drawbone(transform);
        }
    }
}