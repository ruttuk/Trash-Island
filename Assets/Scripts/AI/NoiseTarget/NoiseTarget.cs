using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTarget : MonoBehaviour
{
    /***
     * 
     * NoiseTarget should be attached to any game object that makes noise.
     * This includes player character, boat, sirens, or objects with physics.
     * 
     * A NT can be switched on or off, this differs depending on the object.
     * For player or boat, NT is switched on when moving.
     * 
     ***/

    public NoiseTargetData noiseTargetData;
    public bool targetActive;
    public int currentDecibels;

    // activity level should be in range 0-1. We then set the actual decibel level of the NT clamped between max, min.
    public void ActivateTarget(float activityLevel)
    {
        activityLevel = Mathf.Clamp01(activityLevel);
        currentDecibels = Mathf.RoundToInt(noiseTargetData.minDecibels + activityLevel * (noiseTargetData.maxDecibels - noiseTargetData.minDecibels));
        targetActive = true;
        //Debug.Log($"Setting {noiseTargetData.name} to active, with dB level of {currentDecibels}!");
    }

    public void DeactivateTarget()
    {
        currentDecibels = 0;
        targetActive = false;
        //Debug.Log($"Setting {noiseTargetData.name} to inactive!");
    }
}
