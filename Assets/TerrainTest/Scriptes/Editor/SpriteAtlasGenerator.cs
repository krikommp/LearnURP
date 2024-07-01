using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasGenerator : EditorWindow
{
    private string folderPath;

    [MenuItem("Tools/Sprite Atlas Generator")]
    public static void ShowWindow()
    {
        GetWindow<SpriteAtlasGenerator>("Sprite Atlas Generator");
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
            .Where(file => file.EndsWith(".png") || file.EndsWith(".jpg"))
            .Select(file => AssetDatabase.LoadAssetAtPath<Texture2D>(GetRelativeAssetPath(file)))
            .Where(texture => texture != null)
            .ToArray();

        if (textures.Length == 0)
        {
            Debug.LogWarning("No valid image files found in the selected folder.");
            return;
        }

        SpriteAtlas spriteAtlas = new SpriteAtlas();
        SpriteAtlasExtensions.Add(spriteAtlas, textures);

        string atlasPath = Path.Combine("Assets", "GeneratedSpriteAtlas.spriteatlas");
        AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"Sprite Atlas generated at {atlasPath} with {textures.Length} sprites.");
    }

    private string GetRelativeAssetPath(string absolutePath)
    {
        return "Assets" + absolutePath.Replace(Application.dataPath, "").Replace('\\', '/');
    }
}