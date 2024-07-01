using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomGenerateMeshWithGpuInstance))]
public class RandomGenerateMeshWithGpuInstanceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RandomGenerateMeshWithGpuInstance myScript = (RandomGenerateMeshWithGpuInstance)target;
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
