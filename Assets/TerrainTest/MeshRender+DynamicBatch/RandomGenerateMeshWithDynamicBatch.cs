using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGenerateMeshWithDynamicBatch : TerrainRoot
{
    [SerializeField] private RandomSpawnData randomSpawnData;
    [SerializeField] private List<TextureAtlasData> textureAtlasDatas;
    [SerializeField] private Transform root;
    [SerializeField] private Material instancedMaterial;
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Texture2D colorMask;
    [SerializeField] private Texture2D shadowMap;
    
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
        
        foreach (var textureAtlasData in textureAtlasDatas)
        {
            var atlasName = textureAtlasData.atlasName;
            var newMat = new Material(instancedMaterial);

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
                meshRenderer.sharedMaterial = newMat;
                
                meshRenderer.sharedMaterial.SetTexture("_MainTex", textureAtlasData.atlas);
                meshRenderer.sharedMaterial.renderQueue = spawnData.renderqueue;
                
                var idx = textureAtlasData.textureNames.IndexOf(spawnData.name);
                var rect = textureAtlasData.textureRects[idx];
            
                var newMesh = new Mesh();
                newMesh.name = "newMesh";
                newMesh.vertices = quadMesh.vertices;
                newMesh.triangles = quadMesh.triangles;
            
                Vector2[] newUV = new Vector2[4];
                newUV[0] = new Vector2(rect.x, rect.y);
                newUV[1] = new Vector2(rect.x, rect.y + rect.height);
                newUV[2] = new Vector2(rect.x + rect.width, rect.y + rect.height);
                newUV[3] = new Vector2(rect.x + rect.width, rect.y);
                newMesh.uv = newUV;
            
                meshFilter.mesh = newMesh;
            }
        }
        
        //
        // for (int i = 0; i < numToGenerate; i++)
        // {
        //     float randomSpriteCount = Random.Range(0, textures.Count);
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
        //     newObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //     newObject.transform.SetParent(root, true);
        //
        //     MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
        //     MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
        //     meshRenderer.enabled = true;
        //     meshRenderer.sharedMaterial = instancedMaterial;
        //     
        //     meshRenderer.sharedMaterial.SetTexture("_MainTex", textureAtlasData.atlas);
        //     meshRenderer.sharedMaterial.SetTexture("_MainTex2", texture2);
        //     meshRenderer.sharedMaterial.SetTexture("_MainTex3", texture3);
        //
        //     var idx = textureAtlasData.textureNames.IndexOf(textures[(int)randomSpriteCount].name);
        //     
        //     var rect = textureAtlasData.textureRects[idx];
        //     
        //     var newMesh = new Mesh();
        //     newMesh.name = "newMesh";
        //     newMesh.vertices = quadMesh.vertices;
        //     newMesh.triangles = quadMesh.triangles;
        //     // newMesh.uv = quadMesh.uv;
        //     
        //     Vector2[] newUV = new Vector2[4];
        //     newUV[0] = new Vector2(rect.x, rect.y);
        //     newUV[1] = new Vector2(rect.x, rect.y + rect.height);
        //     newUV[2] = new Vector2(rect.x + rect.width, rect.y + rect.height);
        //     newUV[3] = new Vector2(rect.x + rect.width, rect.y);
        //     newMesh.uv = newUV;
        //
        //     meshFilter.mesh = newMesh;
        //     
        //     meshRenderer.SetPropertyBlock(props);
        // }
    }
}
