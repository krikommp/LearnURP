using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSpawnGenerator : EditorWindow
{
    private SpawnConfig spawnConfig;
    private GameObject terrainRoot;
    private TerrainRoot terrainRootScript;
    private string outputFolder;
    private HashSet<string> totalCategories;

    [MenuItem("Tools/Random Spawn Generator")]
    public static void ShowWindow()
    {
        GetWindow<RandomSpawnGenerator>("Random Spawn Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Random Spawn Settings", EditorStyles.boldLabel);
        
        terrainRoot = (GameObject)EditorGUILayout.ObjectField("Terrain Root", terrainRoot, typeof(GameObject), true);
        terrainRootScript = terrainRoot?.GetComponent<TerrainRoot>();
        
        spawnConfig = (SpawnConfig)EditorGUILayout.ObjectField("Spawn Config", spawnConfig, typeof(SpawnConfig), false);

        if (GUILayout.Button("Select Output Folder"))
        {
            outputFolder = EditorUtility.OpenFolderPanel("Select Output Folder", "", "");
        }

        if (!string.IsNullOrEmpty(outputFolder))
        {
            EditorGUILayout.LabelField("Output Folder", outputFolder);
        }
        
        if (terrainRootScript != null && spawnConfig != null && !string.IsNullOrEmpty(outputFolder))
        {
            if (GUILayout.Button("Spawn Objects"))
            {
                GenerateSpawnObjects();
            }
        }
    }

    private void GenerateSpawnObjects()
    {
        totalCategories = new();
        
        var randomSpawnAsset = ScriptableObject.CreateInstance<RandomSpawnData>(); 
        randomSpawnAsset.items = new List<RandomSpawnItem>();
        
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.shrubbery));
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.forest));
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.singleTree));
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.grass));
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.mountain));
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.stone));
       randomSpawnAsset.items.AddRange( GenerateSpawnObjects_Internal(spawnConfig.building));

       randomSpawnAsset.totalCategories = totalCategories.Count;
       
       // 保存为 Asset
       string assetPath = Path.Combine(outputFolder, "RandomSpawnData" + ".asset");
       AssetDatabase.CreateAsset(randomSpawnAsset, ConvertToRelativePath(assetPath));
       AssetDatabase.SaveAssets();
    }

    private List<RandomSpawnItem> GenerateSpawnObjects_Internal(SpawnData spawnData)
    {
        var randomSpawnItems = new List<RandomSpawnItem>();
        for (int i = 0; i < spawnData.num; ++i)
        {
            var randomIdx = Random.Range(0, spawnData.names.Count);
            var newName = spawnData.names[randomIdx];

            totalCategories.Add(newName);
            
            Terrain selectedTerrain = terrainRootScript.terrains[Random.Range(0, terrainRootScript.terrains.Count)];
            
            Vector3 randomPosition = terrainRootScript.GetRandomPositionOnTerrain(selectedTerrain);
            randomPosition.y = terrainRootScript.GetTerrainHeight(selectedTerrain, randomPosition) + 1.5f;

            var newEulerAngle = new Vector3(-45, 180, 0);
            var newScale = new Vector3(spawnData.scale, spawnData.scale, spawnData.scale);
            var newPosition = randomPosition;
            

            var newRandomSpawnItem = new RandomSpawnItem();
            newRandomSpawnItem.name = newName;
            newRandomSpawnItem.renderqueue = spawnData.renderqueue;
            newRandomSpawnItem.atlas = spawnData.atlas;
            
            newRandomSpawnItem.position = newPosition;
            newRandomSpawnItem.eulerAngle = newEulerAngle;
            newRandomSpawnItem.scale = newScale;

            randomSpawnItems.Add(newRandomSpawnItem);
        }

        return randomSpawnItems;
    }
    
    private string GetRelativeAssetPath(string absolutePath)
    {
        return "Assets" + absolutePath.Replace(Application.dataPath, "").Replace('\\', '/');
    }
    
    // 将绝对路径转换为相对于 Assets 文件夹的路径
    public static string ConvertToRelativePath(string absolutePath)
    {
        // 获取项目的根目录路径
        string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

        // 检查绝对路径是否包含项目路径
        if (absolutePath.StartsWith(projectPath))
        {
            // 提取相对路径
            return absolutePath.Substring(projectPath.Length);
        }
        else
        {
            Debug.LogWarning("路径不在项目目录中: " + absolutePath);
            return absolutePath;
        }
    }
}
