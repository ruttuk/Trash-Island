using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseTargetData", menuName = "ScriptableObjects/NoiseTargetData", order = 1)]
public class NoiseTargetData : ScriptableObject
{
    public string targetName;
    public int minDecibels;
    public int maxDecibels;
}
