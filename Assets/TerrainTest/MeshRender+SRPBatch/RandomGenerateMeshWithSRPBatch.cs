using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGenerateMeshWithSRPBatch : TerrainRoot
{
    [SerializeField] private RandomSpawnData randomSpawnData;
    [SerializeField] private List<TextureAtlasData> textureAtlasDatas;
    [SerializeField] private Transform root;
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Texture2D colorMask;
    [SerializeField] private Texture2D shadowMap;
    [SerializeField] private Material srpBatchMaterial;
    
    private static Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
    
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

        var spawnDatas = randomSpawnData.items;
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
                
            meshFilter.mesh = quadMesh;
                
            if (!materialCache.TryGetValue(spawnData.name, out var newMaterial))
            {
                var textureAtlasData = textureAtlasDatas.Find(t => t.atlasName == spawnData.atlas);
                
                newMaterial = new Material(srpBatchMaterial.shader); 
                newMaterial.name = spawnData.name;
                    
                var idx = textureAtlasData.textureNames.IndexOf(spawnData.name);
                var rect = textureAtlasData.textureRects[idx];
                newMaterial.SetTexture("_MainTex", textureAtlasData.atlas);
                newMaterial.SetVector("_NewUV", new Vector4(rect.x, rect.y, rect.width, rect.height));
                    
                materialCache[spawnData.name] = newMaterial;
            }

            meshRenderer.sharedMaterial = newMaterial;
        }
    }
}
