using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomGenerateMesh))]
public class RandomGenerateMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RandomGenerateMesh myScript = (RandomGenerateMesh)target;
        // if (!myScript.gameObject.GetComponent<T>())
        // {
        //     if(GUILayout.Button("Add TerrainRoot"))
        //     {
        //         myScript.gameObject.AddComponent<T>();
        //     }
        // }

        if(GUILayout.Button("Generate"))
        {
            myScript.Generate();
        }
        
        if(GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
    }
}
