using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRoot : MonoBehaviour
{
    [SerializeField] public List<Terrain> terrains = new List<Terrain>();
    
    public Vector3 GetRandomPositionOnTerrain(Terrain terrain)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float randomX = Random.Range(terrainPosition.x, terrainPosition.x + terrainSize.x);
        float randomZ = Random.Range(terrainPosition.z, terrainPosition.z + terrainSize.z);

        return new Vector3(randomX, 0, randomZ);
    }

    public float GetTerrainHeight(Terrain terrain, Vector3 position)
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float normalizedX = (position.x - terrainPosition.x) / terrainSize.x;
        float normalizedZ = (position.z - terrainPosition.z) / terrainSize.z;

        return terrain.terrainData.GetHeight(
            (int)(Mathf.Clamp01(normalizedX) * terrain.terrainData.heightmapResolution),
            (int)(Mathf.Clamp01(normalizedZ) * terrain.terrainData.heightmapResolution)) + terrainPosition.y;
    }
}
