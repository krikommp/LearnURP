using UnityEditor;
using UnityEngine;

namespace Terrain.Scripts.Editor
{
    [UnityEditor.CustomEditor(typeof(GenerateTerrainMat))]
    public class GenerateTerrainMatEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GenerateTerrainMat generateTerrainMat = target as GenerateTerrainMat;
            if (generateTerrainMat == null) return;
            
            if (GUILayout.Button("Collect Terrain Textures"))
            {
                generateTerrainMat.CollectTerrainTex();
            }
        }
    }
    
}