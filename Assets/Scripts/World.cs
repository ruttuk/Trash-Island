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

    const float seaLevel = 2f;
    const int regionSize = 1024;

    GameObject seaFloor;
    GameObject oceanPlane;
    GameObject reverseOceanPlane;

    public GameObject oceanPlanePrefab;
    public GameObject reverseOceanPlanePrefab;
    public GameObject seafloorPrefab;

    void Awake()
    {
        regionGenerator = FindObjectOfType<RegionGenerator>();
    }

    void Start()
    {
        CreateWorld();
        CreateOcean();
    }

    void CreateWorld()
    {
        int halfRegion = regionSize / 2;

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();

        regions[0] = regionGenerator.CreateRegion(regionSize, -halfRegion, -halfRegion, regionTransforms[0], mapDisplay);
        regions[1] = regionGenerator.CreateRegion(regionSize, -halfRegion, halfRegion, regionTransforms[1], mapDisplay);
        regions[2] = regionGenerator.CreateRegion(regionSize, halfRegion, -halfRegion, regionTransforms[2], mapDisplay);
        regions[3] = regionGenerator.CreateRegion(regionSize, halfRegion, halfRegion, regionTransforms[3], mapDisplay);
    }

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
}
