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
