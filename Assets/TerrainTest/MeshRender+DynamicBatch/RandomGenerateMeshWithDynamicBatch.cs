using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGenerateMeshWithDynamicBatch : TerrainRoot
{
    [SerializeField] private RandomSpawnData randomSpawnData;
    [SerializeField] private List<TextureAtlasData> textureAtlasDatas;
    [SerializeField] private Transform root;
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
        Debug.Log("Generate mesh with dynamic batch.");
        
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
            Debug.Log($"Generate mesh with dynamic batch: {textureAtlasData.atlasName}");
            
            var atlasName = textureAtlasData.atlasName;
            
            textureAtlasData.material.SetTexture("_MainTex", textureAtlasData.atlas);

            var spawnDatas = atlasName == "SingleAtlas" ? randomSpawnData.items : randomSpawnData.items.FindAll(x => x.atlas == atlasName).ToList();
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
    }
}
