using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomGenerateMeshWithGpuInstance : MonoBehaviour
{
    [SerializeField] private List<Terrain> terrains = new List<Terrain>();
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private int numToGenerate = 1;
    [SerializeField] private Transform root;
    [SerializeField] private Material instancedMaterial;
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Texture2D texture2;
    [SerializeField] private Texture2D texture3;
    
    private Texture2DArray spriteTextures;
    private MaterialPropertyBlock props;
    private int textureId = 0;

    public void Clear()
    {
        Debug.LogWarning("Clear sprite mesh");
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(root.GetChild(i).gameObject);
        }
    }

    private void Start()
    {
        Clear();
        Generate();
    }

    public void Generate()
    {
        if (terrains.Count == 0)
        {
            Debug.LogError("No terrains assigned.");
            return;
        }

        if (sprites.Count == 0)
        {
            Debug.LogError("No sprites assigned.");
            return;
        }
        
        if (root == null)
        {
            Debug.LogError("No root transform assigned.");
            return;
        }
        
        Texture2D tex = sprites[0].texture;
        spriteTextures = new Texture2DArray(512, 512, 128, tex.format, false);

        props = new MaterialPropertyBlock();
        
        for (int i = 0; i < sprites.Count; ++i)
        {
            tex = sprites[i].texture;
            Graphics.CopyTexture(tex, 0, 0, spriteTextures, i, 0);
        }

        for (int i = 0; i < numToGenerate; i++)
        {
            int randomSpriteCount = Random.Range(0, sprites.Count);
            textureId = randomSpriteCount;
            
            // Select a random terrain from the list
            Terrain selectedTerrain = terrains[Random.Range(0, terrains.Count)];

            // Generate a random position on the selected terrain
            Vector3 randomPosition = GetRandomPositionOnTerrain(selectedTerrain);
            randomPosition.y = GetTerrainHeight(selectedTerrain, randomPosition) + 1.5f;

            GameObject newObject = new GameObject("GeneratedMeshObject");
            newObject.transform.eulerAngles = new Vector3(-45, 180, 0);
            newObject.transform.localPosition = randomPosition;
            newObject.transform.SetParent(root, true);

            MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
            meshRenderer.enabled = true;
            meshRenderer.sharedMaterial = instancedMaterial;
            
            meshRenderer.sharedMaterial.SetTexture("_Textures", spriteTextures);
            meshRenderer.sharedMaterial.SetTexture("_MainTex2", texture2);
            meshRenderer.sharedMaterial.SetTexture("_MainTex3", texture3);
            meshFilter.sharedMesh = quadMesh;

            props.SetFloat("_TextureIndex", textureId);
            
            meshRenderer.SetPropertyBlock(props);
        }

#if UNITY_EDITOR
        SaveTextureArrayAsset(spriteTextures, "Assets/TerrainTest/MeshRender+GPUInstance/InstancedSpriteTextureArray.asset");
#endif
    }
    
    
    private Vector3 GetRandomPositionOnTerrain(Terrain terrain)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
        float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);

        return new Vector3(randomX, 0, randomZ);
    }

    private float GetTerrainHeight(Terrain terrain, Vector3 position)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float normalizedX = (position.x - terrainPosition.x) / terrainSize.x;
        float normalizedZ = (position.z - terrainPosition.z) / terrainSize.z;

        return terrain.terrainData.GetHeight(
            (int)(Mathf.Clamp01(normalizedX) * terrain.terrainData.heightmapResolution),
            (int)(Mathf.Clamp01(normalizedZ) * terrain.terrainData.heightmapResolution)) + terrainPosition.y;
    }
    
#if UNITY_EDITOR
    private void SaveTextureArrayAsset(Texture2DArray textureArray, string path)
    {
        AssetDatabase.CreateAsset(textureArray, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"Texture2DArray saved at {path}");
    }
#endif
}
