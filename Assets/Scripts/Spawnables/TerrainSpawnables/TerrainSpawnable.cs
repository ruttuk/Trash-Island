using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainSpawnable", menuName = "ScriptableObjects/TerrainSpawnable", order = 1)]
public class TerrainSpawnable : ScriptableObject
{
    public string spawnableName;
    public bool matchNormalAngle;
    public float yOffset;
    public int minPerIsland;
    public int maxPerIsland;
    public GameObject[] objectVariations;
}
