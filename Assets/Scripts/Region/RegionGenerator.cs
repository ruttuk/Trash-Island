using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegionGenerator : MonoBehaviour
{
    const int minIslandSize = 64;
    const int maxMeshSize = 250;

    public bool autoUpdate;
    public IslandGenerator islandGenerator;
    public Biome biome;
    public GameObject islandMeshPrefab;

    private int maxIslandSize;
    private int totalLandMass;

    //Region region;
    List<GameObject> islandMeshes;

    // DATA FOR PLACING AND SIZING ISLANDS
    List<IslandData.IslandMassData> islandMassDataList = new List<IslandData.IslandMassData>();
    //Dictionary<Vector2, int> occupiedLocations = new Dictionary<Vector2, int>();

    public Region CreateRegion(int regionSize, int worldOffsetX, int worldOffsetY, Transform regionTransform, MapDisplay mapDisplay)
    {
        Region region = new Region(regionSize, biome);

        // Important that they should happen in this order.
        SetTotalLandMass(regionSize);
        SetIslandMasses();
        SetIslandLocations(region, regionSize, worldOffsetX, worldOffsetY, mapDisplay);
        SpawnIslands(region, regionTransform, mapDisplay);

        ClearRegion();

        return region;
    }

    void SetTotalLandMass(int regionSize)
    {
        System.Random prng = new System.Random();
        totalLandMass = Mathf.RoundToInt(biome.landMassPercentage * regionSize);

        if(totalLandMass % 2 != 0)
        {
            totalLandMass -= 1;
        }

        maxIslandSize = Mathf.RoundToInt(totalLandMass * biome.granularity);

        if(maxIslandSize > maxMeshSize)
        {
            maxIslandSize = maxMeshSize;
        }

        if(maxIslandSize % 2 != 0)
        {
            maxIslandSize -= 1;
        }
    }

    void SetIslandMasses()
    {
        System.Random prng = new System.Random();

        int runningTotalMass = 0;
        int randomIslandMass;

        while (runningTotalMass < totalLandMass)
        {
            if(maxIslandSize - minIslandSize < 0)
            {
                randomIslandMass = minIslandSize;
            }
            else
            {
                randomIslandMass = minIslandSize + Mathf.RoundToInt(prng.Next(maxIslandSize - minIslandSize) * biome.granularity);
            }

            // DO NOT ALLOW ODD SIZE ISLANDS
            if (randomIslandMass % 2 != 0)
            {
                randomIslandMass -= 1;
            }

            if (runningTotalMass + randomIslandMass > totalLandMass)
            {
                randomIslandMass = totalLandMass - runningTotalMass;
                if (randomIslandMass < minIslandSize)
                {
                    totalLandMass -= randomIslandMass;
                    break;
                }
            }
            IslandData.IslandMassData imd;

            imd.islandSize = randomIslandMass;
            //Debug.Log("Creating island with mass of " + randomIslandMass);

            int halfMass = randomIslandMass / 2;
            int numPoints = prng.Next(halfMass - halfMass / 2, halfMass);
            imd.numPoints = numPoints;

            islandMassDataList.Add(imd);

            runningTotalMass += randomIslandMass;
        }
    }

    void SetIslandLocations(Region region, int regionSize, int worldOffsetX, int worldOffsetY, MapDisplay mapDisplay)
    {
        System.Random prng = new System.Random();

        int x, y;
        x = y = 0;
        Island island;

        // Get a random x, y coordinate within region bounds.
        // This acts as the top left corner of the island - so check that all points within potential bounds are free.
        // If not, retry. If we can't place the island after X tries, then break.
        int retries = 0;
        int maxRetries = 12;
        int halfMass;

        //Vector2 possibleCoors;

        for (int i = 0; i < islandMassDataList.Count; i++)
        {
            for (int j = 0; j < maxRetries; j++, retries++)
            {
                halfMass = islandMassDataList[i].islandSize / 2;
                x = prng.Next(halfMass, regionSize - halfMass) + worldOffsetX;
                y = prng.Next(halfMass, regionSize - halfMass) + worldOffsetY;

                if (CheckForOverlap(region, x, y, islandMassDataList[i].islandSize))
                {
                    retries = 0;
                    break;
                }
            }

            if (retries + 1 == maxRetries)
            {
                // We shot a blank, wrap it up
                break;
            }
            else
            {
                island = islandGenerator.GenerateIsland(biome, islandMassDataList[i], mapDisplay);
                region.islands.Add(island);
                region.islands[i].x = x;
                region.islands[i].y = y;
            }
        }
    }
    
    bool CheckForOverlap(Region region, int possibleX, int possibleY, int possibleSize)
    {
        if(region.islands.Count == 0)
        {
            return true;
        }

        foreach(Island island in region.islands)
        {
            if( Overlap(possibleX, possibleY, island.x, island.y, possibleSize, island.size))
            {
                return false;
            }
        }

        return true;
    }

    bool ValueInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }
    
    bool Overlap(int x1, int y1, int x2, int y2, int size1, int size2)
    {
        bool xOverlap = ValueInRange(x1, x2, x2 + size2) ||
            ValueInRange(x2, x1, x1 + size1);

        bool yOverlap = ValueInRange(y1, y2, y2 + size2) ||
            ValueInRange(y2, y1, y1 + size1);

        return xOverlap && yOverlap;
    }

    /*
    bool CheckIfValidLocation(int islandMass, int islandID, int x, int y)
    {
        Vector2 coordinates = new Vector2(0, 0);

        int spacing = 1;

        for (int j = x; j < x + islandMass; j += spacing)
        {
            for (int k = y; k < y + islandMass; k += spacing)
            {
                coordinates.x = j;
                coordinates.y = k;

                if (occupiedLocations.ContainsKey(coordinates))
                {
                    // If it's not a valid location, remove all associated keys from dictionary.
                    foreach(var item in occupiedLocations.Where(kvp => kvp.Value == islandID).ToList())
                    {
                        occupiedLocations.Remove(item.Key);
                    }
                    return false;
                }
                else
                {
                    occupiedLocations.Add(coordinates, islandID);
                }
            }
        }
        return true;
    }
    */

    void SpawnIslands(Region region, Transform regionTransform, MapDisplay mapDisplay)
    {
        islandMeshes = new List<GameObject>();

        GameObject spawnedMesh;
        Vector3 spawnedPosition;
        MeshRenderer spawnedRenderer;
        MeshCollider spawnedCollider;

        Shader islandMatShader = islandMeshPrefab.GetComponent<Renderer>().sharedMaterial.shader;

        for (int i = 0; i < region.islands.Count; i++)
        {
            spawnedPosition = new Vector3(region.islands[i].x, 0f, region.islands[i].y);

            spawnedMesh = Instantiate(islandMeshPrefab, spawnedPosition, Quaternion.identity, regionTransform);

            spawnedRenderer = spawnedMesh.GetComponent<MeshRenderer>();
            spawnedRenderer.sharedMaterial = new Material(islandMatShader);

            spawnedCollider = spawnedMesh.GetComponent<MeshCollider>();
            spawnedCollider.sharedMesh = region.islands[i].meshData.CreateMesh();

            mapDisplay.DrawTexture(region.islands[i].texture, spawnedMesh.GetComponent<MeshFilter>(), spawnedRenderer);
            mapDisplay.DrawMesh(region.islands[i].meshData, region.islands[i].texture, spawnedMesh.GetComponent<MeshFilter>(), spawnedRenderer);

            islandMeshes.Add(spawnedMesh);
        }

        // First, instantiate all the landmarks.
        /*
        GameObject[] spawnedLandmarks = new GameObject[biome.landmarks.Length];

        for (int i = 0; i < biome.landmarks.Length; i++)
        {
            spawnedLandmarks[i] = Instantiate(biome.landmarks[i].landmarkObject, Vector3.zero, Quaternion.identity, transform);
        }
        */
        //System.Random prng = new System.Random();

        //int randomIndex;
        //bool spawned;

        //region.SortIslandsBySize();

        // Then, try spawning them on random islands.

        /*
        for(int i = 0; i < spawnedLandmarks.Length; i++)
        {
            randomIndex = prng.Next(0, region.islands.Count);
            spawned = islandGenerator.TrySpawnLandmark(region.islands[0], spawnedLandmarks[i]);
        }
        */
        /*
        Island largest = region.islands[0];

        for (int i = 0; i < region.islands.Count; i++)
        {
            if(region.islands[i].size > largest.size)
            {
                largest = region.islands[i];
            }
        }
        */

        //islandGenerator.TrySpawnLandmark(largest, spawnedLandmarks[0]);

        for (int i = 0; i < region.islands.Count; i++)
        {
            // Lastly, spawn stuff on the island.
            islandGenerator.SpawnObjectsOnIsland(region.islands[i], biome);
        }
    }

    public Vector3 GetIslandPosition(int index)
    {
        if(index < islandMeshes.Count)
        {
            return islandMeshes[index].transform.position;
        }
        return Vector3.negativeInfinity;
    }

    public Vector3 GetRandomPointOfTerrainType(Region region, int islandIndex, IslandData.IslandTerrainType terrainType)
    {
        if (islandIndex < region.islands.Count)
        {
            float half = region.islands[islandIndex].size / 2f;

            for (int x = 0; x < region.islands[islandIndex].size; x++)
            {
                for (int y = 0; y < region.islands[islandIndex].size; y++)
                {
                    if(region.islands[islandIndex].terrainMap[x, y] == terrainType)
                    {
                        return new Vector3(region.islands[islandIndex].x - half + x, 0, region.islands[islandIndex].y - half + y);
                    }
                }
            }
        }
        return Vector3.negativeInfinity;
    }

    public void ClearRegion()
    {
        /*
        foreach(GameObject plane in islandMeshes)
        {
            DestroyImmediate(plane);
        }
        */

        islandMassDataList.Clear();
        islandMeshes.Clear();
        //occupiedLocations.Clear();

        //DestroyImmediate(seaFloor);
        //DestroyImmediate(oceanPlane);
    }
}
