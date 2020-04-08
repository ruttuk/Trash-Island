using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IslandData
{
    public enum IslandGenerationType { Random, Mountains }
    public enum IslandTerrainType { Ocean, Beach, Lowlands, Highlands, Plateau }

    [System.Serializable]
    public struct IslandMassData
    {
        public int islandSize;
        public int numPoints;
    }

    [System.Serializable]
    public struct IslandTerrain
    {
        public IslandTerrainType type;
        public float height;
        public Color color;
        public TerrainSpawnable[] spawnableObjects;
    }

    public static IslandTerrainType[,] GetTerrainsFromNoiseMap(int size, float[,] noiseMap, Biome biome)
    {
        IslandTerrainType[,] terrainMap = new IslandTerrainType[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < biome.terrainTypes.Length; i++)
                {
                    if(currentHeight >= biome.terrainTypes[i].height)
                    {
                        terrainMap[x, y] = biome.terrainTypes[i].type;
                    }
                }
            }
        }

        return terrainMap;
    }

    public static Color[] GetColorMap(int islandSize, float[,] noiseMap, IslandData.IslandTerrain[] terrains)
    {
        Color[] colorMap = new Color[islandSize * islandSize];

        for (int y = 0; y < islandSize; y++)
        {
            for (int x = 0; x < islandSize; x++)
            {
                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < terrains.Length; i++)
                {
                    if (currentHeight >= terrains[i].height)
                    {
                        colorMap[y * islandSize + x] = terrains[i].color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return colorMap;
    }

    /** Take the difference between the noise map and falloff map **/
    public static float[,] SubtractFalloff(float[,] noiseMap, float[,] falloffMap, int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                noiseMap[x, y] = Mathf.Clamp(falloffMap[x, y] - noiseMap[x, y], 0, int.MaxValue);
            }
        }
        return noiseMap;
    }
}
