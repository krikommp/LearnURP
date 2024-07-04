using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomGenerateSprites))]
public class RandomGenerateSpritesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RandomGenerateSprites myScript = (RandomGenerateSprites)target;
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