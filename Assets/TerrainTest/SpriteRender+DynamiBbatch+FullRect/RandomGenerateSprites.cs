using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;

[Serializable]
public class SpriteAtlasAndMat
{
    public SpriteAtlas spriteAtlas;
    public Material material;
}

public class RandomGenerateSprites : TerrainRoot
{
    [SerializeField] private RandomSpawnData randomSpawnData;
    [SerializeField] private Transform root;
    [SerializeField] private Texture2D colorMask;
    [SerializeField] private Texture2D shadowMap;
    [SerializeField] private List<SpriteAtlasAndMat> spriteAtlasAndMats;
    
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

        if (root == null)
        {
            Debug.LogError("No root transform assigned.");
            return;
        }
        
        if (randomSpawnData == null)
        {
            Debug.LogError("No random spawn data assigned.");
            return;
        }
        
        if (spriteAtlasAndMats == null)
        {
            Debug.LogError("No sprite atlases assigned.");
            return;
        }

        Shader.SetGlobalTexture("_ColorMask", colorMask);
        Shader.SetGlobalTexture("_ShadowMap", shadowMap);

        for (int i = 0; i < randomSpawnData.items.Count; ++i)
        {
            var spawnData = randomSpawnData.items[i];

            var sm = spriteAtlasAndMats.FirstOrDefault(sm => sm.spriteAtlas.GetSprite(spawnData.name) != null);
            if (sm != null)
            {
                var sprite = sm.spriteAtlas.GetSprite(spawnData.name);
            
                if (sprite == null)
                {
                    Debug.LogError($"Sprite {spawnData.name} not found in any sprite atlas.");
                    continue;
                }
            
                GameObject newObject = new GameObject($"GeneratedMeshObject({spawnData.name})");
                newObject.transform.eulerAngles = spawnData.eulerAngle;
                var position = transform.position;
                position.y += 1.5f;
                newObject.transform.localPosition = spawnData.position + position;
                newObject.transform.SetParent(root, true);
                newObject.transform.localScale = spawnData.scale;
            
                SpriteRenderer spriteRenderer = newObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sharedMaterial = sm.material;
                spriteRenderer.sprite = sprite;
            }
        }
    }
}