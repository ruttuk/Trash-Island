using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Landmark", menuName = "ScriptableObjects/Landmark", order = 1)]
public class Landmark : ScriptableObject
{
    // Is the landmark spawned in open water or within the bounds of an island?
    public bool openWater;
    public GameObject landmarkObject;
}
