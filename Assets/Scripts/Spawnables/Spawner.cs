using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    const int regionSize = 1024;

    /***
     * Given an island, try to sample a flat area to spawn the object.
     ***/
    public static Vector3 SpawnObjectOnFlatArea(Island island, GameObject spawnedObject)
    {
        float centerX = island.x / 2f;
        float centerY = island.y / 2f;
        int spacing = 4;

        Vector2 actualPos;
        Vector3 spawnPos = Vector3.zero;

        int boundX, boundY, boundZ;

        Bounds bounds = spawnedObject.GetComponentInChildren<MeshCollider>().bounds;

        boundX = Mathf.RoundToInt(bounds.size.x);
        boundY = Mathf.RoundToInt(bounds.size.y / 2f);
        boundZ = Mathf.RoundToInt(bounds.size.z);

        for (int x = 0; x < island.size; x += spacing)
        {
            for (int y = 0; y < island.size; y += spacing)
            {
                float half = island.size / 2f;
                actualPos.x = island.x - half + x;
                actualPos.y = island.y + half - y;

                spawnPos = GetFlatArea(actualPos, boundX, boundZ);

                if (spawnPos != Vector3.zero)
                {
                    Debug.Log("SPAWNING FLAT LANDMARK! at " + spawnPos);
                    //spawnPos.y += boundY;
                    //spawnedObject.transform.localPosition = spawnPos;

                    return spawnPos;
                }
            }
        }

        // if we get here, the object didn't find a good place to spawn.
        return Vector3.zero;
    }

    /***
     * Given an island and a biome, spawn a number of terrain objects (trees/rocks/etc) on island.
     ***/
    public static void SpawnTerrainObjectsOnIsland(Island island, Biome biome, Transform parent)
    {
        System.Random prng = new System.Random();

        List<Vector2> possibleTerrainCoordinates = new List<Vector2>();
        Vector2 coor = new Vector2(0, 0);
        Vector2 actualPos = new Vector2(0, 0);

        // spacing between points we sample for each terrain
        int spacing = 2;
        int numSpawnables;
        float minHeight;

        foreach (IslandData.IslandTerrain terrain in biome.terrainTypes)
        {
            // Get a pool of acceptable positions to spawn from
            for (int x = 0; x < island.size; x += spacing)
            {
                for (int y = 0; y < island.size; y += spacing)
                {
                    if (island.terrainMap[x, y] == terrain.type)
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

                    SpawnTerrainObject(objectToSpawn, actualPos, minHeight, spawnable.matchNormalAngle, spawnable.yOffset, parent);
                }
            }
        }
    }

    /***
     * Raycast to spawn at correct position.
     ***/
    static void SpawnTerrainObject(GameObject spawnableObject, Vector2 coordinates, float minHeight, bool matchNormalAngle, float yOffset, Transform parent)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(coordinates.x, 999f, coordinates.y), -Vector3.up, out hit))
        {

            if (hit.point.y >= minHeight)
            {
                var startRot = matchNormalAngle ? Quaternion.FromToRotation(Vector3.up, hit.normal) : Quaternion.identity;

                Vector3 spawnPosition = new Vector3(hit.point.x, hit.point.y - yOffset, hit.point.z);

                GameObject spawned = Instantiate(spawnableObject, spawnPosition, Quaternion.identity);
                spawned.transform.SetParent(parent);
            }
        }
    }

    /***
     * Given x, y coordinates and x, z bounds determine if the area is flat.
     * 
     * Used to check for valid locations for buildings/etc.
     * 
     ***/
    public static Vector3 GetFlatArea(Vector2 coordinates, int boundX, int boundZ)
    {
        RaycastHit hit;
        Vector3 spawnPos = Vector3.zero;
        float minHeight = 3f;

        for (int x = 0; x < boundX; x++)
        {
            for (int z = 0; z < boundZ; z++)
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

    /***
     * Will spawn landmarks in different regions with the following rules:
     * 
     * 1. Landmarks marked "flat" are given priority, and must be placed on a flat island.
     * 2. Each region has a max number of landmarks, so they should be relatively evenly distributed per region.
     * 3. Landmarks marked "open water" will be placed as if they were an island, so at a random available location in the region.
     * 
     ***/
    public static void SpawnLandmarks(Region[] regions, Landmark[] landmarks)
    {
        int numRegions = regions.Length;

        // First let's separate the flat landmarks from the rest.

        List<Landmark> flatLandmarks = new List<Landmark>();
        List<Landmark> otherLandmarks = new List<Landmark>();

        for (int i = 0; i < landmarks.Length; i++)
        {
            if (landmarks[i].flat)
            {
                flatLandmarks.Add(landmarks[i]);
            }
            else
            {
                otherLandmarks.Add(landmarks[i]);
            }
        }

        GameObject[] spawnedFlatLandmarks = new GameObject[flatLandmarks.Count];
        GameObject[] spawnedOtherLandmarks = new GameObject[otherLandmarks.Count];

        // Now let's actually spawn all the landmarks.
        // We do it this way so that we can access the instantiated collider, getting the world space bounds.

        for (int i = 0; i < spawnedFlatLandmarks.Length; i++)
        {
            spawnedFlatLandmarks[i] = Instantiate(flatLandmarks[i].landmarkObject, Vector3.zero, Quaternion.identity);
        }

        for (int i = 0; i < spawnedOtherLandmarks.Length; i++)
        {
            spawnedOtherLandmarks[i] = Instantiate(otherLandmarks[i].landmarkObject, Vector3.zero, Quaternion.identity);
        }

        // Now let's loop through our regions, and for each region, let's loop through all the islands and check for a flat area.
        int landmarkIndex = 0;
        Vector3 spawnPos;

        for (int i = 0; i < numRegions; i++)
        {
            for (int j = 0; j < regions[i].islands.Count; j++)
            {
                spawnPos = SpawnObjectOnFlatArea(regions[i].islands[j], spawnedFlatLandmarks[landmarkIndex]);

                if (spawnPos != Vector3.zero)
                {
                    Debug.Log("Spawning flat landmark!");

                    spawnedFlatLandmarks[landmarkIndex].transform.position = spawnPos;
                    flatLandmarks[landmarkIndex].position = spawnPos;
                    regions[i].landmarks.Add(flatLandmarks[landmarkIndex]);

                    landmarkIndex++;

                    if (landmarkIndex >= flatLandmarks.Count)
                    {
                        // we've found a place for all the flat landmarks!
                        i = numRegions;
                        break;
                    }
                }
            }
        }

        int maxLandmarksPerRegion = 2;
        int regionIndex = 0;
        bool spawned;

        // So now that we're done with the flat landmarks, go through the rest of the landmarks.
        // We spawn by region until each region is at their max # of landmarks.
        for (int i = 0; i < otherLandmarks.Count; i++)
        {
            if (otherLandmarks[i].openWater)
            {
                spawned = SpawnLandmarkInOpenWater(regions[regionIndex], spawnedOtherLandmarks[i], otherLandmarks[i]);

                if (!spawned)
                {
                    Debug.Log("Failed to spawn landmark in open water.");
                }
            }
            else
            {
                // TODO: address non-flat, non-open water landmarks.
            }

            if (regions[regionIndex].landmarks.Count == maxLandmarksPerRegion)
            {
                regionIndex++;
            }

            if (regionIndex >= numRegions)
            {
                break;
            }
        }
    }

    /***
     * Try to spawn a landmark in open water. Uses the same logic to find an open island location.
     ***/
    public static bool SpawnLandmarkInOpenWater(Region region, GameObject spawnedLandmark, Landmark landmark)
    {
        System.Random prng = new System.Random();
        int halfSize;
        int coorX, coorZ;

        //Bounds spawnedLandmarkBounds;
        //int boundX, boundZ;

        Vector3 spawnPos;

        //spawnedLandmarkBounds = spawnedLandmark.GetComponent<Collider>().bounds;
        //boundX = Mathf.RoundToInt(spawnedLandmarkBounds.size.x);
        //boundZ = Mathf.RoundToInt(spawnedLandmarkBounds.size.z);

        halfSize = landmark.size / 2; //boundX > boundZ ? boundX / 2 : boundZ / 2;

        int numTries = 4;

        for (int i = 0; i < numTries; i++)
        {
            coorX = prng.Next(halfSize, regionSize - halfSize) + region.worldOffsetX;
            coorZ = prng.Next(halfSize, regionSize - halfSize) + region.worldOffsetY;

            if (CheckForOverlap(region, coorX, coorZ, halfSize * 2))
            {
                spawnPos = new Vector3(coorX, landmark.yOffset, coorZ);
                landmark.position = spawnPos;
                spawnedLandmark.transform.position = spawnPos;
                Debug.Log("Spawning in open water...");

                return true;
            }
        }

        return false;
    }

    /***
     * Given a region and possible x,y coordinates and a bounding size (square), does the proposed rect overlap with any others? 
     *
     * note: this can be used for both island/landmark placement, as long as we have a viable rect size.
     ***/
    public static bool CheckForOverlap(Region region, int possibleX, int possibleY, int possibleSize)
    {
        if (region.islands.Count == 0)
        {
            return true;
        }

        foreach (Island island in region.islands)
        {
            if (Overlap(possibleX, possibleY, island.x, island.y, possibleSize, island.size))
            {
                return false;
            }
        }

        return true;
    }

    /***
     * Check that value is between min and max.
     * 
     ***/
    private static bool ValueInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /***
     * Given two sets of coordinates (x1, y1) , (x2, y2) and two sizes (size1, size2), do the areas overlap?
     * 
     ***/
    private static bool Overlap(int x1, int y1, int x2, int y2, int size1, int size2)
    {
        bool xOverlap = ValueInRange(x1, x2, x2 + size2) ||
            ValueInRange(x2, x1, x1 + size1);

        bool yOverlap = ValueInRange(y1, y2, y2 + size2) ||
            ValueInRange(y2, y1, y1 + size1);

        return xOverlap && yOverlap;
    }
}
