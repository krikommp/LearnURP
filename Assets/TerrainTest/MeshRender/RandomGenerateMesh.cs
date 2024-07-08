using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGenerateMesh : TerrainRoot
{
    [SerializeField] private RandomSpawnData randomSpawnData;
    [SerializeField] private Transform root;
    [SerializeField] private Mesh quadMesh;
    [SerializeField] private Texture2D colorMask;
    [SerializeField] private Texture2D shadowMap;
    [SerializeField] private List<Texture2D> textures;
    [SerializeField] private Material material;
    
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
        
        if (textures == null)
        {
            Debug.LogError("No textures assigned.");
            return;
        }
        
        Shader.SetGlobalTexture("_ColorMask", colorMask);
        Shader.SetGlobalTexture("_ShadowMap", shadowMap);
        
        for (int i = 0; i < randomSpawnData.items.Count; ++i)
        {
            var spawnData = randomSpawnData.items[i];

            var tex = textures.FirstOrDefault(t => t.name == spawnData.name);

            if (tex == null)
            {
                continue;
            }

            GameObject newObject = new GameObject($"GeneratedMeshObject({spawnData.name})");
            newObject.transform.eulerAngles = spawnData.eulerAngle;
            var position = transform.position;
            newObject.transform.localPosition = spawnData.position + position;
            newObject.transform.SetParent(root, true);
            newObject.transform.localScale = spawnData.scale;
            
            MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newObject.AddComponent<MeshRenderer>();
            meshRenderer.enabled = true;

            if (!materialCache.TryGetValue(spawnData.name, out var newMaterial))
            {
                newMaterial = new Material(material.shader); 
                newMaterial.name = spawnData.name;
                newMaterial.SetTexture("_MainTex", tex);
                materialCache[spawnData.name] = newMaterial;
            }

            meshRenderer.sharedMaterial = newMaterial;
                
            meshFilter.mesh = quadMesh;
        }
    }
}
