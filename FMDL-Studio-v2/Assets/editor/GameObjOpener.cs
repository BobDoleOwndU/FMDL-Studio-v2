using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameObjCreator
{
    GameObject objToSpawn;

    public void Start()
    {
        objToSpawn = new GameObject("FMDL");

        objToSpawn.AddComponent<MeshFilter>();
        objToSpawn.AddComponent<MeshRenderer>();
    }
}