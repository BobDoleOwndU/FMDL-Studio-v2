using UnityEditor;
using UnityEngine;

public class Test
{
    static int numObjects;

    [MenuItem("FMDL Studio/Convert to FBX")]
    public static void FBXDebug()
    {
        numObjects = 1;

        if (Selection.activeGameObject != null)
            FBXConverter.ConvertToFBX(Selection.activeGameObject);
        else
            Debug.Log("No objects selected.");
    } //GetNumObjects

    public static void GetNumObjects(Transform transform)
    {
        foreach (Transform t in transform)
        {
            numObjects++;
            GetNumObjects(t);
        } //foreach ends
    } //GetNumObjects
} //class
