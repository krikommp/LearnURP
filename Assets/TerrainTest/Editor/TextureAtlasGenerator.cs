using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class TextureAtlasGenerator : EditorWindow
{
    private string folderPath;
    private string outputFolder;
    private string atlasName;
    public int atlasWidth = 2048; // 大纹理的宽度
    public int atlasHeight = 2048; // 大纹理的高度

    [MenuItem("Tools/Texture Atlas Generator")]
    public static void ShowWindow()
    {
        GetWindow<TextureAtlasGenerator>("Texture Atlas Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Folder to Generate Sprite Atlas", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Folder"))
        {
            folderPath = EditorUtility.OpenFolderPanel("Select Folder with Images", "", "");
        }

        if (!string.IsNullOrEmpty(folderPath))
        {
            EditorGUILayout.LabelField("Selected Folder", folderPath);
        }

        if (GUILayout.Button("Select Output Folder"))
        {
            outputFolder = EditorUtility.OpenFolderPanel("Select Output Folder", "", "");
        }

        if (!string.IsNullOrEmpty(outputFolder))
        {
            EditorGUILayout.LabelField("Output Folder", outputFolder);
        }
        
        atlasName = EditorGUILayout.TextField("Atlas Name", atlasName);
        
        if (!string.IsNullOrEmpty(folderPath) && !string.IsNullOrEmpty(outputFolder) && !string.IsNullOrEmpty(atlasName))
        {
            if (GUILayout.Button("Generate Sprite Atlas"))
            {
                GenerateSpriteAtlas(folderPath);
            }
        }
        
        
    }

    private void GenerateSpriteAtlas(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Folder path is empty. Please select a valid folder.");
            return;
        }

        string[] imageFiles = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        Texture2D[] textures = imageFiles
            .Where(file => file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".tga"))
            .Select(file => AssetDatabase.LoadAssetAtPath<Texture2D>(GetRelativeAssetPath(file)))
            .Where(texture => texture != null)
            .ToArray();

        if (textures.Length == 0)
        {
            Debug.LogWarning("No valid image files found in the selected folder.");
            return;
        }
        
        Texture2D atlasTexture = new Texture2D(atlasWidth, atlasHeight);
        
        // 将小纹理合并到大纹理中，并记录偏移信息
        Rect[] rects = atlasTexture.PackTextures(textures.ToArray(), 0, atlasWidth);
        
        // 保存合并后的纹理
        File.WriteAllBytes(Path.Combine(outputFolder, atlasName + ".png"), atlasTexture.EncodeToPNG());

        var textureAtlas = ScriptableObject.CreateInstance<TextureAtlasData>();
        textureAtlas.atlasName = atlasName;
        textureAtlas.atlas = atlasTexture;
        for (int i = 0; i < textures.Length; ++i)
        {
            var name = textures[i].name;
            var rect = rects[i];
            var idx = i;
            textureAtlas.textureRects.Add(rect);
            textureAtlas.textureNames.Add(name);
            textureAtlas.textureIds.Add(idx);
        }
        
        // 保存为 Asset
        string assetPath = Path.Combine(outputFolder, atlasName + ".asset");
        AssetDatabase.CreateAsset(textureAtlas, ConvertToRelativePath(assetPath));
        AssetDatabase.SaveAssets();
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