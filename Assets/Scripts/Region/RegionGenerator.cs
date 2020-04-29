using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RegionGenerator : MonoBehaviour
{
    const int minIslandSize = 64;
    const int maxMeshSize = 250;

    public bool autoUpdate;
    public IslandGenerator islandGenerator;
    //public Biome biome;
    private Biome biome;

    public GameObject islandMeshPrefab;

    private int maxIslandSize;
    private int totalLandMass;

    // editor setting
    public Biome editorBiome;

    List<GameObject> islandMeshes;
    List<IslandData.IslandMassData> islandMassDataList = new List<IslandData.IslandMassData>();

    public Region CreateRegion(int regionSize, int worldOffsetX, int worldOffsetY, Transform regionTransform, MapDisplay mapDisplay, Biome biome)
    {
        Region region = new Region(regionSize, biome, worldOffsetX, worldOffsetY);

        this.biome = biome;

        // Important that they should happen in this order.
        SetTotalLandMass(regionSize);
        SetIslandMasses();
        SetIslandLocations(region, regionSize, worldOffsetX, worldOffsetY, mapDisplay);
        SpawnIslandMeshes(region, regionTransform, mapDisplay);

        ClearRegion();

        return region;
    }

    const float landMassMaxPercentage = 0.75f;

    void SetTotalLandMass(int regionSize)
    {
        // total land mass limit should be like 3/4 of the region

        totalLandMass = Mathf.RoundToInt(regionSize * biome.landMassPercentage);

        if (totalLandMass % 2 != 0)
        {
            totalLandMass -= 1;
        }

        maxIslandSize = 250;
        /*
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
        */
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

        for (int i = 0; i < islandMassDataList.Count; i++)
        {
            for (int j = 0; j < maxRetries; j++, retries++)
            {
                halfMass = islandMassDataList[i].islandSize / 2;
                x = prng.Next(halfMass, regionSize - halfMass) - worldOffsetX;
                y = prng.Next(halfMass, regionSize - halfMass) + worldOffsetY;

                if (Spawner.CheckForOverlap(region, x, y, islandMassDataList[i].islandSize))
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

    void SpawnIslandMeshes(Region region, Transform regionTransform, MapDisplay mapDisplay)
    {
        islandMeshes = new List<GameObject>();

        Vector3 spawnedPosition;
        MeshRenderer spawnedRenderer;
        MeshCollider spawnedCollider;

        Shader islandMatShader = islandMeshPrefab.GetComponent<Renderer>().sharedMaterial.shader;

        for (int i = 0; i < region.islands.Count; i++)
        {
            spawnedPosition = new Vector3(region.islands[i].x, 0f, region.islands[i].y);

            GameObject spawnedMesh = Instantiate(islandMeshPrefab, spawnedPosition, Quaternion.identity, regionTransform);

            spawnedRenderer = spawnedMesh.GetComponent<MeshRenderer>();
            spawnedRenderer.sharedMaterial = new Material(islandMatShader);

            spawnedCollider = spawnedMesh.GetComponent<MeshCollider>();
            spawnedCollider.sharedMesh = region.islands[i].meshData.CreateMesh();

            mapDisplay.DrawTexture(region.islands[i].texture, spawnedMesh.GetComponent<MeshFilter>(), spawnedRenderer);
            mapDisplay.DrawMesh(region.islands[i].meshData, region.islands[i].texture, spawnedMesh.GetComponent<MeshFilter>(), spawnedRenderer);

            islandMeshes.Add(spawnedMesh);
            region.islands[i].islandMeshObject = spawnedMesh;

            // what if we try adding a navmeshvolumemod here?
            //SetObstacles(region.islands[i], spawnedMesh);

            Spawner.SpawnTerrainObjectsOnIsland(region.islands[i], biome, spawnedMesh.transform);
        }
    }

    private void SetObstacles(Island island, GameObject spawnedMesh)
    {
        for(int x = 0; x < island.size; x += 4)
        {
            for (int y = 0; y < island.size; y += 4)
            {
                if(island.terrainMap[x, y] == IslandData.IslandTerrainType.Highlands || island.terrainMap[x, y] == IslandData.IslandTerrainType.Plateau)
                {
                    var go = new GameObject();
                    NavMeshObstacle obstacle = go.AddComponent(typeof(NavMeshObstacle)) as NavMeshObstacle;
                    go.transform.SetParent(spawnedMesh.transform);

                    float half = island.size / 2f;
                    obstacle.center = new Vector3(island.x - half + x, 2, island.y + half - y);
                    obstacle.size = new Vector3(4, 4, 4);
                    obstacle.carving = true;
                }
            }
        }
    }

    public Vector3 GetIslandMeshWorldPosition(int index)
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
