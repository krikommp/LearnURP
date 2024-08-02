using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using AAA;

public class CloudShadowController : EditorWindow
{
    [MenuItem("Tools/Cloud Shadow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<CloudShadowController>();
    }

    private List<Material> m_CloudShadowMaterials = new List<Material>();
    private CloudShadowParameters m_CloudShadowParameters;

    private void OnEnable()
    {
    }

    private void CollectCloudShadowShaders()
    {
        m_CloudShadowMaterials.Clear();

        var terrains = Terrain.activeTerrains;
        foreach (var terrain in terrains)
        {
            if (terrain.materialTemplate.shader.keywordSpace.FindKeyword("_CLOUD_SHADOW_ON").isValid)
            {
                m_CloudShadowMaterials.Add(terrain.materialTemplate);
            }
        }
            
        var renders = FindObjectsOfType<Renderer>();    
        
        if (renders == null)
        {
            Debug.LogError("No renderer found in the scene.");
            return;
        }
        
        var targetRenders = renders.Where(r => r.sharedMaterial.shader.keywordSpace.FindKeyword("_CLOUD_SHADOW_ON").isValid);
        
        foreach (var render in targetRenders)
        {
            var materials = render.sharedMaterials;
            // if array already contains the material, skip it
            foreach (var material in materials)
            {
                if (m_CloudShadowMaterials.Contains(material))
                {
                    continue;
                }
                
                m_CloudShadowMaterials.Add(material);
            }
        }
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("Collect Cloud Shadow Shaders"))
        {
            CollectCloudShadowShaders();
        }
        // Draw the cloud shadow parameters
        m_CloudShadowParameters = EditorGUILayout.ObjectField("Cloud Shadow Parameters", m_CloudShadowParameters, typeof(CloudShadowParameters), false) as CloudShadowParameters;
        
        if (m_CloudShadowParameters == null)
        {
            return;
        }
        
        EditorGUI.BeginChangeCheck();
        
        m_CloudShadowParameters.m_CloudShadowTexture = EditorGUILayout.ObjectField("Cloud Shadow Texture", m_CloudShadowParameters.m_CloudShadowTexture, typeof(Texture2D), false) as Texture2D;
        
        // make tiling name and value in one line
        m_CloudShadowParameters.m_CloudShadowTiling = EditorGUILayout.Vector2Field("Cloud Shadow Tiling", m_CloudShadowParameters.m_CloudShadowTiling);
        m_CloudShadowParameters.m_CloudShadowOffset = EditorGUILayout.Vector2Field("Cloud Shadow Offset", m_CloudShadowParameters.m_CloudShadowOffset);
        
        m_CloudShadowParameters.m_cloudShadowSpeed = EditorGUILayout.Vector2Field("Cloud Shadow Speed", m_CloudShadowParameters.m_cloudShadowSpeed);
        
        // split m_cloudShadowParams info 4 float fields
        // x for Cloud Coverage
        // y for Cloud Softness
        // z for Cloud Opacity
        // w for Cloud Scale
        var cloudCoverage = m_CloudShadowParameters.m_cloudShadowParams.x;
        var cloudSoftness = m_CloudShadowParameters.m_cloudShadowParams.y;
        var cloudOpacity = m_CloudShadowParameters.m_cloudShadowParams.z;
        var cloudScale = m_CloudShadowParameters.m_cloudShadowParams.w;
        
        // change floatfield to slider
        m_CloudShadowParameters.m_cloudShadowParams.x = EditorGUILayout.Slider("Cloud Coverage", cloudCoverage, 0.0f, 1.0f);
        m_CloudShadowParameters.m_cloudShadowParams.y = EditorGUILayout.Slider("Cloud Softness", cloudSoftness, 0.0f, 1.0f);
        m_CloudShadowParameters.m_cloudShadowParams.z = EditorGUILayout.Slider("Cloud Opacity", cloudOpacity, 0.0f, 1.0f);
        m_CloudShadowParameters.m_cloudShadowParams.w = EditorGUILayout.Slider("Cloud Scale", cloudScale, 0.0f, 1.0f);
        
        if (EditorGUI.EndChangeCheck())
        {
            // apply the changes to all the materials
            ApplyCloudShadowParameters();
        }
        
        if (GUILayout.Button("Apply Cloud Shadow Parameters"))
        {
            ApplyCloudShadowParameters();
        }
    }
    
    private void ApplyCloudShadowParameters()
    {
        // apply the changes to all the materials
        foreach (var material in m_CloudShadowMaterials)
        {
            material.SetTexture("_CloudTex", m_CloudShadowParameters.m_CloudShadowTexture);
            material.SetVector("_CloudTex_ST", new Vector4(m_CloudShadowParameters.m_CloudShadowTiling.x, m_CloudShadowParameters.m_CloudShadowTiling.y, m_CloudShadowParameters.m_CloudShadowOffset.x, m_CloudShadowParameters.m_CloudShadowOffset.y));
            material.SetFloat("_SpeedX", m_CloudShadowParameters.m_cloudShadowSpeed.x);
            material.SetFloat("_SpeedY", m_CloudShadowParameters.m_cloudShadowSpeed.y);
            material.SetVector("_CloudShadowParams", m_CloudShadowParameters.m_cloudShadowParams);
        }
    }
}
