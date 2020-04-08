using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RegionGenerator))]
public class RegionEditor : Editor
{
    private int defaultRegionSize = 1024;
    private int defaultOffsetX = 0;
    private int defaultOffsetY = 0;

    public override void OnInspectorGUI()
    {
        RegionGenerator regionGenerator = (RegionGenerator)target;

        if (DrawDefaultInspector())
        {
            //
        }

        if (GUILayout.Button("Generate Region"))
        {
            regionGenerator.CreateRegion(defaultRegionSize, defaultOffsetX , defaultOffsetY, regionGenerator.transform, FindObjectOfType<MapDisplay>());
        }

        if (GUILayout.Button("Clear Region"))
        {
            regionGenerator.ClearRegion();
        }
    }
}
