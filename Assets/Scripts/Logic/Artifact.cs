using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    private const float addToInventoryRange = 0.5f;
    private const string playerTag = "Player";

    private Transform player;
    private ItemBox itemBox;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        itemBox = FindObjectOfType<ItemBox>();
    }

    void Update()
    {
        if(Utility.CheckIfTransformInRange(player, transform, addToInventoryRange))
        {
            Debug.Log("Player is in range of artifact! Adding to item box.");
            itemBox.AddArtifactToItemBox(this);
            gameObject.SetActive(false);
        }
    }
}
