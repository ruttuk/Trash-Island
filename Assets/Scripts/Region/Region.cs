using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Region
{
    int size;
    float[,] coordinates;

    public int worldOffsetX;
    public int worldOffsetY;
    public List<Landmark> landmarks;
    public List<Island> islands;
    public Biome biome;

    public Region(int size, Biome biome, int worldOffsetX, int worldOffsetY)
    {
        this.size = size;
        this.biome = biome;
        this.worldOffsetX = worldOffsetX;
        this.worldOffsetY = worldOffsetY;

        coordinates = new float[size, size];
        islands = new List<Island>();
        landmarks = new List<Landmark>();
    }
}
