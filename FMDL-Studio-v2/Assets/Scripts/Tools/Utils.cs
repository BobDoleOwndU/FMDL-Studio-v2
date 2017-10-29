using UnityEngine;

public static class Utils
{
    public static void GetNumObjects(Transform transform, ref int count)
    {
        foreach (Transform t in transform)
        {
            count++;
            GetNumObjects(t, ref count);
        } //foreach
    } //GetNumObjects
}