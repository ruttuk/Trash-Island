using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Region
{
    int size;
    float[,] coordinates;

    public List<Island> islands;
    public Biome biome;

    public Region(int size, Biome biome)
    {
        this.size = size;
        this.biome = biome;

        coordinates = new float[size, size];
        islands = new List<Island>();
    }
}
