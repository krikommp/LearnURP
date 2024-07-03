using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomSpawnItem
{ 
    public string name;
    public int renderqueue;
    public string atlas;
    
    public Vector3 position;
    public Vector3 eulerAngle;
    public Vector3 scale;
}

public class RandomSpawnData : ScriptableObject
{
    public int totalCategories;
    public List<RandomSpawnItem> items = new List<RandomSpawnItem>();
}
