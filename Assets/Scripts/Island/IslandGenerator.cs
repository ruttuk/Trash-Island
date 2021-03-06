﻿using System;
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
    public bool saveTexureToPng;
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

            if(saveTexureToPng)
            {
                string fileName = editorBiome.name + "_p" + islandMassDataEditor.numPoints + "_s" + islandMassDataEditor.islandSize;
                string _fullPath = Application.dataPath + "/Resources/Etc/" + fileName +".jpg";
                byte[] _bytes = islandMapTexture.EncodeToJPG();
                System.IO.File.WriteAllBytes(_fullPath, _bytes);
                Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
            }
        }

        island.terrainMap = IslandData.GetTerrainsFromNoiseMap(massData.islandSize, noiseMap, biome);
        island.meshData = islandMesh;
        island.texture = islandMapTexture;

        return island;
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
