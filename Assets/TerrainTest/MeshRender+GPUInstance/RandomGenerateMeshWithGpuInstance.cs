using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGenerateMeshWithGpuInstance : TerrainRoot
{
    [SerializeField] private RandomSpawnData randomSpawnData;
    [SerializeField] private List<TextureAtlasData> textureAtlasDatas;
    [SerializeField] private Transform root;
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Texture2D colorMask;
    [SerializeField] private Texture2D shadowMap;
    
    private MaterialPropertyBlock props;

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
        
        Shader.SetGlobalTexture("_ColorMask", colorMask);
        Shader.SetGlobalTexture("_ShadowMap", shadowMap);

        props = new MaterialPropertyBlock();
        
        
        foreach (var textureAtlasData in textureAtlasDatas)
        {
            var atlasName = textureAtlasData.atlasName;
            
            textureAtlasData.material.SetTexture("_MainTex", textureAtlasData.atlas);

            var spawnDatas = randomSpawnData.items.FindAll(x => x.atlas == atlasName).ToList();
            for (int i = 0; i < spawnDatas.Count; ++i)
            {
                var spawnData = spawnDatas[i];
                
                GameObject newObject = new GameObject($"GeneratedMeshObject({spawnData.name})");
                newObject.transform.eulerAngles = spawnData.eulerAngle;
                var position = transform.position;
                newObject.transform.localPosition = spawnData.position + position;
                newObject.transform.SetParent(root, true);
                newObject.transform.localScale = spawnData.scale;
            
                MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
                meshRenderer.enabled = true;
                meshRenderer.sharedMaterial = textureAtlasData.material;
                meshFilter.mesh = quadMesh;
                
                var idx = textureAtlasData.textureNames.IndexOf(spawnData.name);
                var rect = textureAtlasData.textureRects[idx];
                
                props.SetVector("_NewUV", new Vector4(rect.x, rect.y, rect.width, rect.height));
                meshRenderer.SetPropertyBlock(props);
            }
        }

        // for (int i = 0; i < numToGenerate; i++)
        // {
        //     int randomSpriteCount = Random.Range(0, sprites.Count);
        //     textureId = randomSpriteCount;
        //     
        //     // Select a random terrain from the list
        //     Terrain selectedTerrain = terrains[Random.Range(0, terrains.Count)];
        //
        //     // Generate a random position on the selected terrain
        //     Vector3 randomPosition = GetRandomPositionOnTerrain(selectedTerrain);
        //     randomPosition.y = GetTerrainHeight(selectedTerrain, randomPosition) + 1.5f;
        //
        //     GameObject newObject = new GameObject("GeneratedMeshObject");
        //     newObject.transform.eulerAngles = new Vector3(-45, 180, 0);
        //     newObject.transform.localPosition = randomPosition;
        //     newObject.transform.SetParent(root, true);
        //
        //     MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
        //     MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
        //     meshRenderer.enabled = true;
        //     meshRenderer.sharedMaterial = instancedMaterial;
        //     
        //     meshRenderer.sharedMaterial.SetTexture("_Textures", spriteTextures);
        //     meshRenderer.sharedMaterial.SetTexture("_MainTex2", texture2);
        //     meshRenderer.sharedMaterial.SetTexture("_MainTex3", texture3);
        //     meshFilter.sharedMesh = quadMesh;
        //
        //     props.SetFloat("_TextureIndex", textureId);
        //     
        //     meshRenderer.SetPropertyBlock(props);
        // }
    }
}
