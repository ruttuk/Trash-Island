using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public const int regionSize = 512;

    public static void SpawnSpawnPoints()
    {

        Transform playerSpawnTransform = GameObject.FindGameObjectWithTag("PLAYER_SPAWN").transform;
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SPAWNPOINT");

        int numSpawnPoints = spawnPoints.Length;

        float currentSpawnDistance;
        float closestSpawnDistance = float.MaxValue;
        int closestSpawnIndex = 0;

        // We want to make sure the spawn point that is closest to player spawn is active.
        for (int i = 0; i < numSpawnPoints; i++)
        {
            currentSpawnDistance = Vector3.Distance(playerSpawnTransform.position, spawnPoints[i].transform.position);

            if(currentSpawnDistance < closestSpawnDistance)
            {
                closestSpawnDistance = currentSpawnDistance;
                closestSpawnIndex = i;
            }
        }

        spawnPoints[closestSpawnIndex].AddComponent<Artifact>();

        // Now, let's set all the other spawn points inactive.
        for (int i = 0; i < numSpawnPoints; i++)
        {
            if(i != closestSpawnIndex)
            {
                spawnPoints[i].SetActive(false);
            }
        }

        int numArtifacts = 8;

        // pick 8 numbers between 0-spawnPoints.length
        // These will be artifacts. 

        System.Random prng = new System.Random();
        int index;

        for (int i = 0; i < numArtifacts - 1; i++)
        {
            index = prng.Next(0, 999999) % numSpawnPoints;

            if(!spawnPoints[index].activeSelf)
            {
                Debug.Log("Setting spawn point active at index " + index);
                spawnPoints[index].SetActive(true);

                // IMPORTANT: We can choose these spawn points to be artifacts, in which we add the artifact component.
                spawnPoints[index].AddComponent<Artifact>();
            }
            else
            {
                // basically doing this if we hit a spawn point we already activated
                i--;
            }
        }
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

                    SpawnTerrainObject(objectToSpawn, actualPos, minHeight, spawnable.matchNormalAngle, spawnable.yOffset, parent, biome.spawnAboveMinHeight);
                }
            }
        }
    }

    /***
     * Raycast to spawn at correct position.
     ***/
    static void SpawnTerrainObject(GameObject spawnableObject, Vector2 coordinates, float minHeight, bool matchNormalAngle, float yOffset, Transform parent, bool spawnAboveMinHeight)
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(coordinates.x, 999f, coordinates.y), -Vector3.up, out hit))
        {
            if(spawnAboveMinHeight && hit.point.y >= minHeight || !spawnAboveMinHeight)
            {
                var startRot = matchNormalAngle ? Quaternion.FromToRotation(Vector3.up, hit.normal) : Quaternion.identity;
                Vector3 spawnPosition = new Vector3(hit.point.x, hit.point.y - yOffset, hit.point.z);

                GameObject spawned = Instantiate(spawnableObject, spawnPosition, Quaternion.identity);
                spawned.transform.SetParent(parent);
            }
        }
    }

    /***
     * Given an island, try to sample a flat area to spawn the object.
     ***/
    public static Vector3 SpawnObjectOnFlatArea(Island island, Landmark landmark)
    {
        //float centerX = island.x / 2f;
        //float centerY = island.y / 2f;
        float half = island.size / 2f;
        int quarter = Mathf.RoundToInt(half / 2f);
        int spacing = 4;

        Vector2 actualPos;
        Vector3 spawnPos = Vector3.zero;

        // Try starting towards the center of the island, this is typically the widest and flattest.
        for (int x = quarter; x < island.size; x += spacing)
        {
            for (int y = quarter; y < island.size; y += spacing)
            {
                actualPos.x = island.x - half + x;
                actualPos.y = island.y + half - y;

                spawnPos = GetFlatArea(actualPos, landmark.boundX, landmark.boundZ, landmark.flatnessModifier);

                if (spawnPos != Vector3.zero)
                {
                    spawnPos.y += landmark.yOffset;

                    return spawnPos;
                }
            }
        }

        // if we get here, the object didn't find a good place to spawn.
        return Vector3.zero;
    }

    /***
     * Given x, y coordinates and x, z bounds determine if the area is flat.
     * 
     * Used to check for valid locations for buildings/etc.
     * 
     ***/
    public static Vector3 GetFlatArea(Vector2 coordinates, int boundX, int boundZ, float flatness)
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

                    if (hit.point.y > minHeight && diff.x < flatness && diff.y < flatness && diff.z < flatness)
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

        // Now let's loop through our regions, and for each region, let's loop through all the islands and check for a flat area.
        int flatLandmarkIndex = 0;
        Vector3 spawnPos;

        for (int i = 0; i < numRegions; i++)
        {
            for (int j = 0; j < regions[i].islands.Count; j++)
            {
                Debug.Log($"Trying to flat spawn {flatLandmarks[flatLandmarkIndex].name}...");
                spawnPos = SpawnObjectOnFlatArea(regions[i].islands[j], flatLandmarks[flatLandmarkIndex]);

                if (spawnPos != Vector3.zero)
                {
                    Debug.Log("Spawning flat landmark!");

                    GameObject spawnedLandmark = Instantiate(flatLandmarks[flatLandmarkIndex].landmarkObject, spawnPos, Quaternion.identity);
                    spawnedLandmark.transform.Rotate(new Vector3(0f, flatLandmarks[flatLandmarkIndex].yRotAngle, 0f));
                    regions[i].landmarks.Add(flatLandmarks[flatLandmarkIndex]);
                    flatLandmarkIndex++;

                    if (flatLandmarkIndex >= flatLandmarks.Count)
                    {
                        Debug.Log("We've placed all flat landmarks.");
                        // we've found a place for all the flat landmarks!
                        i = numRegions;
                        break;
                    }
                }
                else
                {
                    Debug.Log($"No flat area in region {i}, island {j}");
                }
            }
        }

        // convert all the remaining flat landmarks (that we couldnt find flat area for) to open water
        for(int i = flatLandmarkIndex; i < flatLandmarks.Count; i++)
        {
            Debug.Log($"Couldn't flat spawn {flatLandmarks[i].name}, adding to OW.");
            otherLandmarks.Add(flatLandmarks[i]);
        }

        int maxLandmarksPerRegion = 2;
        int startingRegionIndex = 0;

        // Since we spawned all the flat landmarks first, let's see which region we should start in.
        for(int i = 0; i < regions.Length; i++)
        {
            if(regions[i].landmarks.Count == maxLandmarksPerRegion)
            {
                startingRegionIndex++;
            }
        }

        bool spawned;

        // So now that we're done with the flat landmarks, go through the rest of the landmarks.
        // We spawn by region until each region is at their max # of landmarks.

        for (int i = 0; i < otherLandmarks.Count; i++)
        {
            Debug.Log($"Trying to OW spawn {otherLandmarks[i].name} in region {startingRegionIndex}");
            spawned = SpawnLandmarkInOpenWater(regions[startingRegionIndex], otherLandmarks[i]);

            if (!spawned)
            {
                Debug.Log("Failed to spawn landmark in open water.");
            }

            if (regions[startingRegionIndex].landmarks.Count >= maxLandmarksPerRegion)
            {
                startingRegionIndex++;
            }

            if (startingRegionIndex >= numRegions)
            {
                Debug.Log("All regions are maxed out with landmarks.");
                break;
            }
        }
    }

    /***
     * Try to spawn a landmark in open water. Uses the same logic to find an open island location.
     ***/
    public static bool SpawnLandmarkInOpenWater(Region region, Landmark landmark)
    {
        System.Random prng = new System.Random();
        int halfSize;
        int coorX, coorZ;

        Vector3 spawnPos;

        // Take the larger of the bounds and use that size for spawning.
        int largerBound = landmark.boundX > landmark.boundZ ? landmark.boundX : landmark.boundZ;
        halfSize = largerBound/ 2;

        int numTries = 12;

        for (int i = 0; i < numTries; i++)
        {
            coorX = prng.Next(halfSize, regionSize - halfSize) - region.worldOffsetX;
            coorZ = prng.Next(halfSize, regionSize - halfSize) + region.worldOffsetY;

            if (CheckForOverlap(region, coorX, coorZ, halfSize * 2))
            {
                spawnPos = new Vector3(coorX, landmark.yOffset, coorZ);
                GameObject spawnedLandmark = Instantiate(landmark.landmarkObject, spawnPos, Quaternion.identity);
                spawnedLandmark.transform.Rotate(new Vector3(0f, landmark.yRotAngle, 0f));

                // If we create an open water landmark like this, we're basically creating an island of size largerBound.
                Island openWaterLandmark = new Island(largerBound);
                openWaterLandmark.x = coorX;
                openWaterLandmark.y = coorZ;

                region.islands.Add(openWaterLandmark);
                region.landmarks.Add(landmark);

                Debug.Log($"Spawning {landmark.name} in open water...");

                return true;
            }
            else
            {
                Debug.Log("Couldn't find a viable OW spawn location, retrying...");
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
