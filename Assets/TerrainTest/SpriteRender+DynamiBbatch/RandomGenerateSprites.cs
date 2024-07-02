using UnityEngine;
using System.Collections.Generic;
using UnityEngine.U2D;

public class RandomGenerateSprites : MonoBehaviour
{
    [SerializeField] private List<Terrain> terrains = new List<Terrain>();
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private int numToGenerate = 1;
    [SerializeField] private Transform root;
    [SerializeField] private bool useSpriteAtlas = true;
    [SerializeField] private Material spriteMaterial;
    
    private void Start()
    {
        Clear();
        Generate();
    }
    
    public void Clear()
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(root.GetChild(i).gameObject);
        }
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

        if (spriteAtlas == null)
        {
            Debug.LogError("No sprite atlas assigned.");
            return;
        }

        for (int i = 0; i < numToGenerate; i++)
        {
            // Select a random terrain from the list
            Terrain selectedTerrain = terrains[Random.Range(0, terrains.Count)];

            // Generate a random position on the selected terrain
            Vector3 randomPosition = GetRandomPositionOnTerrain(selectedTerrain);
            randomPosition.y = GetTerrainHeight(selectedTerrain, randomPosition) + 1.5f;

            // Create a new GameObject and add a SpriteRenderer
            GameObject newObject = new GameObject("GeneratedSpriteObject");
            SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();

            if (useSpriteAtlas)
            {
                spriteRenderer.sprite = spriteAtlas.GetSprite(sprites[Random.Range(0, sprites.Count)].name);
            }
            else
            {
                spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
            }
            
            spriteRenderer.SetMaterials(new List<Material>() { spriteMaterial });

            // spriteRenderer.sortingOrder = i % 2;

            // Set the position of the new object
            newObject.transform.eulerAngles = new Vector3(-45, 180, 0);
            newObject.transform.localPosition = randomPosition;
            newObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            newObject.transform.SetParent(root, true);
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

        return terrain.terrainData.GetHeight((int)(Mathf.Clamp01(normalizedX) * terrain.terrainData.heightmapResolution), (int)(Mathf.Clamp01(normalizedZ) * terrain.terrainData.heightmapResolution)) + terrainPosition.y;
    }
}