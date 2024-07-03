using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureAtlasData", menuName = "Texture/TextureAtlasData", order = 1)]
public class TextureAtlasData : ScriptableObject
{
    public string atlasName;
    public Texture2D atlas;
    public List<string> textureNames = new List<string>();
    public List<Rect> textureRects = new List<Rect>();
    public List<int> textureIds = new List<int>();
    public Material material;
}
