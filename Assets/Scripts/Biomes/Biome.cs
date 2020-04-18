using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 1)]
public class Biome : ScriptableObject
{
    public IslandData.IslandGenerationType islandGenerationType;
    // what is the total percentage of land mass in region?
    [Range(0f, 1f)]
    public float landMassPercentage;
    // How small will the islands be?
    [Range(0f, 1f)]
    public float granularity = 0.5f;
    public int octaves;
    public float heightMultiplier;
    public AnimationCurve meshHeightCurve;
    public IslandData.IslandTerrain[] terrainTypes;
}
