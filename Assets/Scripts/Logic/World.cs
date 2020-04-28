using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class World : MonoBehaviour
{
    private const int numRegions = 4;
    private Region[] regions = new Region[numRegions];
    private RegionGenerator regionGenerator;

    public Landmark[] landmarks;
    public Transform[] regionTransforms;
    public Biome[] biomes = new Biome[numRegions];

    const float seaLevel = 2f;

    void Awake()
    {
        regionGenerator = FindObjectOfType<RegionGenerator>();
        CreateWorld();
    }
    
    void Start()
    {
        Spawner.SpawnLandmarks(regions, landmarks);
        Spawner.SpawnSpawnPoints();
    }

    void CreateWorld()
    {
        int halfRegion = Spawner.regionSize / 2;
        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        regions[0] = regionGenerator.CreateRegion(Spawner.regionSize, -halfRegion, -halfRegion, regionTransforms[0], mapDisplay, biomes[0]);
        regions[1] = regionGenerator.CreateRegion(Spawner.regionSize, -halfRegion, halfRegion, regionTransforms[1], mapDisplay, biomes[1]);
        regions[2] = regionGenerator.CreateRegion(Spawner.regionSize, halfRegion, -halfRegion, regionTransforms[2], mapDisplay, biomes[2]);
        regions[3] = regionGenerator.CreateRegion(Spawner.regionSize, halfRegion, halfRegion, regionTransforms[3], mapDisplay, biomes[3]);
    }

    /*
    void CreateOcean()
    {
        Vector3 regionScale = new Vector3(regionSize / 5f, regionSize / 5f, regionSize / 5f);

        seaFloor = Instantiate(seafloorPrefab, transform.position, Quaternion.identity, transform);
        seaFloor.transform.parent = transform;

        seaFloor.transform.localScale = regionScale;
        seaFloor.transform.position = new Vector3(regionSize / 2, 0.0001f, regionSize / 2);

        oceanPlane = Instantiate(oceanPlanePrefab, transform.position, Quaternion.identity, transform);
        oceanPlane.transform.parent = transform;

        oceanPlane.transform.localScale = regionScale;
        oceanPlane.transform.position = new Vector3(regionSize / 2, seaLevel, regionSize / 2);

        reverseOceanPlane = Instantiate(reverseOceanPlanePrefab, transform.position, Quaternion.identity, transform);
        reverseOceanPlane.transform.parent = oceanPlane.transform;

        reverseOceanPlane.transform.localScale = regionScale;
        reverseOceanPlane.transform.position = new Vector3(regionSize / 2, seaLevel - 0.0001f, regionSize / 2);
        reverseOceanPlane.transform.Rotate(Vector3.right, 180f);
    }
    */
}
