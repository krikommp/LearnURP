using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerateMesh : MonoBehaviour
{
    [SerializeField] private List<Terrain> terrains = new List<Terrain>();
    [SerializeField] private List<Texture2D> textures = new List<Texture2D>();
    [SerializeField] private int numToGenerate = 1;
    [SerializeField] private Transform root;
    [SerializeField] private Material instancedMaterial;
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Texture2D texture2;
    [SerializeField] private Texture2D texture3;
    
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

        if (textures.Count == 0)
        {
            Debug.LogError("No sprites assigned.");
            return;
        }
        
        if (root == null)
        {
            Debug.LogError("No root transform assigned.");
            return;
        }
        
        for (int i = 0; i < numToGenerate; i++)
        {
            int randomSpriteCount = Random.Range(0, textures.Count);
            
            // Select a random terrain from the list
            Terrain selectedTerrain = terrains[Random.Range(0, terrains.Count)];

            // Generate a random position on the selected terrain
            Vector3 randomPosition = GetRandomPositionOnTerrain(selectedTerrain);
            randomPosition.y = GetTerrainHeight(selectedTerrain, randomPosition) + 1.5f;

            GameObject newObject = new GameObject("GeneratedMeshObject");
            newObject.transform.eulerAngles = new Vector3(-45, 180, 0);
            newObject.transform.localPosition = randomPosition;
            newObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newObject.transform.SetParent(root, true);

            MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
            meshRenderer.enabled = true;
            meshRenderer.material = instancedMaterial;
            
            var material = new Material(instancedMaterial);
            material.SetTexture("_MainTex", textures[randomSpriteCount]);
            material.SetTexture("_MainTex2", texture2);
            material.SetTexture("_MainTex3", texture3);
            
            meshRenderer.material = material;
            meshFilter.mesh = quadMesh;
        }
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
}
