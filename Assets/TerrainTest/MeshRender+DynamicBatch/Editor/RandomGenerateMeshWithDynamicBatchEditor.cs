using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomGenerateMeshWithDynamicBatch))]
public class RandomGenerateMeshWithDynamicBatchEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RandomGenerateMeshWithDynamicBatch myScript = (RandomGenerateMeshWithDynamicBatch)target;
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
