using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomGenerateMeshWithSRPBatch))]
public class RandomGenerateMeshWithSRPBatchEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RandomGenerateMeshWithSRPBatch myScript = (RandomGenerateMeshWithSRPBatch)target;
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
