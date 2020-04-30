using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemBox : MonoBehaviour
{
    public TextMeshProUGUI collectedArtifactsText;

    List<Artifact> artifacts;
    int totalArtifacts;

    private const int neccessaryArtifactCount = 7;

    void Start()
    {
        artifacts = new List<Artifact>();
        totalArtifacts = FindObjectsOfType<Artifact>().Length;

        Debug.Log($"Item Box found a total of {totalArtifacts} artifacts in the world.");
        UpdateText();
    }

    public bool CollectedAllArtifacts()
    {
        return artifacts.Count == neccessaryArtifactCount;
    }

    public void AddArtifactToItemBox(Artifact artifact)
    {
        artifacts.Add(artifact);
        UpdateText();
    }

    void UpdateText()
    {
        collectedArtifactsText.text = artifacts.Count.ToString() + " / " + neccessaryArtifactCount.ToString();
    }
}
