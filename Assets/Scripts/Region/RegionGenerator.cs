﻿using System.Collections;
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

    List<GameObject> islandMeshes;
    List<IslandData.IslandMassData> islandMassDataList = new List<IslandData.IslandMassData>();

    public Region CreateRegion(int regionSize, int worldOffsetX, int worldOffsetY, Transform regionTransform, MapDisplay mapDisplay)
    {
        Region region = new Region(regionSize, biome, worldOffsetX, worldOffsetY);

        // Important that they should happen in this order.
        SetTotalLandMass(regionSize);
        SetIslandMasses();
        SetIslandLocations(region, regionSize, worldOffsetX, worldOffsetY, mapDisplay);
        SpawnIslandMeshes(region, regionTransform, mapDisplay);

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

        for (int i = 0; i < islandMassDataList.Count; i++)
        {
            for (int j = 0; j < maxRetries; j++, retries++)
            {
                halfMass = islandMassDataList[i].islandSize / 2;
                x = prng.Next(halfMass, regionSize - halfMass) + worldOffsetX;
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

            Spawner.SpawnTerrainObjectsOnIsland(region.islands[i], biome, spawnedMesh.transform);
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
