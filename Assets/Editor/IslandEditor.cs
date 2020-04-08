using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IslandGenerator))]
public class IslandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        IslandGenerator islandGenerator = (IslandGenerator)target;

        if (DrawDefaultInspector())
        {
            if (islandGenerator.autoUpdate)
            {
                islandGenerator.GenerateIsland(islandGenerator.editorBiome, islandGenerator.islandMassDataEditor, FindObjectOfType<MapDisplay>());
            }
            if (islandGenerator.islandMassDataEditor.islandSize % 2 != 0)
            {
                islandGenerator.islandMassDataEditor.islandSize -= 1;
            }
            int numPoints = Mathf.RoundToInt(islandGenerator.islandMassDataEditor.islandSize / 2f);

            if (islandGenerator.islandMassDataEditor.numPoints > numPoints)
            {
                islandGenerator.islandMassDataEditor.numPoints = numPoints;
            }
        }

        if (GUILayout.Button("Generate Island"))
        {
            islandGenerator.GenerateIsland(islandGenerator.editorBiome, islandGenerator.islandMassDataEditor, FindObjectOfType<MapDisplay>());
        }
    }
}
