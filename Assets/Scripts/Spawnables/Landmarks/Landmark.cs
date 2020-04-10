using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Landmark", menuName = "ScriptableObjects/Landmark", order = 1)]
public class Landmark : ScriptableObject
{
    public float yOffset;
    public int size;
    public Vector3 position;
    // Is the landmark spawned in open water or within the bounds of an island?
    public bool openWater;
    // Should the landmark be spawned on a flat mesh area?
    public bool flat;
    public GameObject landmarkObject;
}
