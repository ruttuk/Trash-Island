using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island
{
    public int id;
    public int size;
    public int x;
    public int y;
    public Texture2D texture;
    public MeshData meshData;
    public IslandData.IslandTerrainType[,] terrainMap; 

    public Island(int size)
    {
        this.size = size;
    }
}
