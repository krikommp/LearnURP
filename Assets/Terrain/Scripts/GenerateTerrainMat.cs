using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Terrain), typeof(TerrainCollider))]
public class GenerateTerrainMat : MonoBehaviour
{
    private Terrain m_terrain;
    [SerializeField] private List<Texture2D> m_layerTextures = new List<Texture2D>();
    [SerializeField] private List<Texture2D> m_splatTextures = new List<Texture2D>();
    [SerializeField] private Shader m_terrainShader;

    private void Awake()
    {
        m_terrain = GetComponent<Terrain>();
    }

    public void CollectTerrainTex()
    {
        m_layerTextures.Clear();
        m_splatTextures.Clear();
        
        // 获取地形的Layer Texture
        TerrainLayer[] terrainLayers = m_terrain.terrainData.terrainLayers;
        for (int i = 0; i < terrainLayers.Length; i++)
        {
            m_layerTextures.Add(terrainLayers[i].diffuseTexture);
        }

        // 获取地形的Splat Texture
        var alphaMaps = m_terrain.terrainData.alphamapTextures;
        for (int i = 0; i < alphaMaps.Length; i++)
        {
            m_splatTextures.Add(alphaMaps[i]);
        }
    }
    
    
}