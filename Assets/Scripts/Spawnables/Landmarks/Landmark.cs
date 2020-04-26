using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Landmark", menuName = "ScriptableObjects/Landmark", order = 1)]
public class Landmark : ScriptableObject
{
    public float yOffset;

    // important for spawning in the right place. 
    // try placing a box collider on the mesh temporarily to get the bounds without instantiating.

    public int boundX;
    public int boundZ;

    //public Vector3 position;
    // Is the landmark spawned in open water or within the bounds of an island?
    public bool openWater;
    // Should the landmark be spawned on a flat mesh area?
    public bool flat;
    public GameObject landmarkObject;
}
