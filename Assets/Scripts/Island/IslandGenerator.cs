using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MapDisplay))]
public class IslandGenerator : MonoBehaviour
{
    [Header("Island Parameters")]
    [Range(0.2f, 0.8f)]
    public float distanceFromEdge;

    public IslandData.IslandMassData islandMassDataEditor;

    [Header("Editor Settings")]
    public bool autoUpdate;
    public bool drawInEditor;
    public Biome editorBiome;

    public Island GenerateIsland(Biome biome, IslandData.IslandMassData massData, MapDisplay display)
    {
        Island island = new Island(massData.islandSize);
        Vector2[] randomPoints = TextureGenerator.GeneratePoints(massData.islandSize, massData.numPoints, distanceFromEdge, biome.islandGenerationType);

        System.Random prng = new System.Random();
        int seed = prng.Next(-999999, 999999);

        float[,] noiseMap = Noise.GenerateNoiseMap(massData.islandSize, massData.islandSize, seed, biome.octaves);
        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(massData.islandSize, randomPoints);

        noiseMap = IslandData.SubtractFalloff(noiseMap, falloffMap, massData.islandSize);

        Color[] colorMap = IslandData.GetColorMap(massData.islandSize, noiseMap, biome.terrainTypes);

        Texture2D islandMapTexture = TextureGenerator.TextureFromColorMap(colorMap, massData.islandSize, massData.islandSize);
        MeshData islandMesh = MeshGenerator.GenerateTerrainMesh(noiseMap, biome.heightMultiplier, biome.meshHeightCurve);

        if(drawInEditor)
        {
            display.DrawTexture(islandMapTexture);
            display.DrawMesh(islandMesh, islandMapTexture);
        }

        island.terrainMap = IslandData.GetTerrainsFromNoiseMap(massData.islandSize, noiseMap, biome);
        island.meshData = islandMesh;
        island.texture = islandMapTexture;

        return island;
    }

    /** How do we sample a flat area? **/
    public bool TrySpawnLandmark(Island island, GameObject spawnedObject)
    {
        float centerX = island.x / 2f;
        float centerY = island.y / 2f;
        int spacing = 3;

        Vector2 actualPos;
        Vector3 spawnPos = Vector3.zero;

        int boundX, boundY, boundZ;

        Bounds bounds = spawnedObject.GetComponentInChildren<MeshCollider>().bounds;

        boundX = Mathf.RoundToInt(bounds.size.x);
        boundY = Mathf.RoundToInt(bounds.size.y / 2f);
        boundZ = Mathf.RoundToInt(bounds.size.z);

        for(int x = 0; x < island.size; x += spacing)
        {
            for(int y = 0; y < island.size; y += spacing)
            {
                float half = island.size / 2f;
                actualPos.x = island.x - half + x;
                actualPos.y = island.y + half - y;

                spawnPos = RaycastForMeshHeight(actualPos, boundX, boundZ);

                if(spawnPos != Vector3.zero)
                {
                    Debug.Log("SPAWNING TEST LANDMARK! at " + spawnPos);
                    spawnPos.y += boundY;
                    spawnedObject.transform.localPosition = spawnPos;

                    return true;
                }
            }
        }

        // if we get here, the object didn't find a good place to spawn.
        return false;
    }

    public void SpawnObjectsOnIsland(Island island, Biome biome)
    {
        System.Random prng = new System.Random();

        List<Vector2> possibleTerrainCoordinates = new List<Vector2>();
        Vector2 coor = new Vector2(0, 0);
        Vector2 actualPos = new Vector2(0, 0);

        // spacing between points we sample for each terrain
        int spacing = 2;
        int numSpawnables;
        float minHeight;

        foreach(IslandData.IslandTerrain terrain in biome.terrainTypes)
        {
            // Get a pool of acceptable positions to spawn from
            for(int x = 0; x < island.size; x += spacing)
            {
                for(int y = 0; y < island.size; y += spacing)
                {
                    if(island.terrainMap[x, y] == terrain.type)
                    {
                        coor.x = x;
                        coor.y = y;

                        possibleTerrainCoordinates.Add(coor);
                    }
                }
            }

            minHeight = terrain.height * biome.heightMultiplier;

            // then for each spawnable item, spawn somewhere in its range
            foreach (TerrainSpawnable spawnable in terrain.spawnableObjects)
            {
                numSpawnables = prng.Next(spawnable.minPerIsland, spawnable.maxPerIsland);

                // Get a random location within the acceptable terrain to spawn at.
                for (int i = 0; i < numSpawnables; i++)
                {
                    coor = possibleTerrainCoordinates[prng.Next(0, possibleTerrainCoordinates.Count)];

                    float half = island.size / 2f;
                    actualPos.x = island.x - half + coor.x;
                    actualPos.y = island.y + half - coor.y;

                    GameObject objectToSpawn = spawnable.objectVariations[prng.Next(0, spawnable.objectVariations.Length)];

                    SpawnObject(objectToSpawn, actualPos, minHeight, spawnable.matchNormalAngle, spawnable.yOffset);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// Raycast at coordinates to put object in position.
    /// 
    /// </summary>
    /// <param name="spawnableObject"></param>
    /// <param name="coordinates"></param>
    /// <param name="minHeight"></param>
    /// <param name="matchNormalAngle"></param>
    void SpawnObject(GameObject spawnableObject, Vector2 coordinates, float minHeight, bool matchNormalAngle, float yOffset)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(coordinates.x, 999f, coordinates.y), -Vector3.up, out hit))
        {

            if(hit.point.y >= minHeight && !hit.transform.CompareTag("LANDMARK"))
            {
                var startRot = matchNormalAngle ? Quaternion.FromToRotation(Vector3.up, hit.normal) : Quaternion.identity;

                Vector3 spawnPosition = new Vector3(hit.point.x, hit.point.y - yOffset, hit.point.z);

                GameObject spawned = Instantiate(spawnableObject, spawnPosition, Quaternion.identity);
                spawned.transform.SetParent(transform);
            }
        }
    }


    // The idea should be to sample points starting
    Vector3 RaycastForMeshHeight(Vector2 coordinates, int boundX, int boundZ)
    {
        RaycastHit hit;
        Vector3 spawnPos = Vector3.zero;
        float minHeight = 3f;

        for(int x = 0; x < boundX; x++)
        {
            for(int z = 0; z < boundZ; z++)
            {
                if (Physics.Raycast(new Vector3(coordinates.x + x, 999f, coordinates.y - z), -Vector3.up, out hit))
                {
                    Vector3 diff = Vector3.up - hit.normal;

                    if (hit.point.y > minHeight && diff.x < 0.1f && diff.y < 0.1f && diff.z < 0.1f)
                    {
                        spawnPos = hit.point;
                    }
                    else
                    {
                        return Vector3.zero;
                    }
                }
            }
        }

        return spawnPos;
    }
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
