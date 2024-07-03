using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnData
{
    public List<string> names;
    public int num;
    public float scale;
    public int renderqueue;
    public string atlas;
}

[CreateAssetMenu(fileName = "SpawnConfig", menuName = "Texture/SpawnConfig", order = 1)]
public class SpawnConfig : ScriptableObject
{
    public SpawnData shrubbery;
    public SpawnData forest;
    public SpawnData singleTree;
    public SpawnData grass;
    public SpawnData mountain;
    public SpawnData stone;
    public SpawnData building;
}
