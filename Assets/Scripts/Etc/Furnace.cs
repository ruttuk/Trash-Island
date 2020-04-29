using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Furnace : MonoBehaviour
{
    private TextMeshProUGUI furnaceText;
    private CanvasGroup furnaceText_CG;
    private Transform player;
    private ItemBox itemBox;
    private VibeChecker vibeChecker;

    private float playerRange = 4f;
    private string incompleteText = "more orbs required!";
    private string completeText = "press [e] to restore power.";
    private bool playerInRange = false;
    private bool collectedAllArtifacts = false;
    private bool stopChecking = false;

    void Start()
    {
        GameObject furnaceText_GO = GameObject.FindGameObjectWithTag("FURNACE_TEXT");
        furnaceText = furnaceText_GO.GetComponent<TextMeshProUGUI>();
        furnaceText_CG = furnaceText_GO.GetComponent<CanvasGroup>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        itemBox = FindObjectOfType<ItemBox>();
        vibeChecker = FindObjectOfType<VibeChecker>();
    }

    void Update()
    {
        if(!stopChecking)
        {
            CheckForPlayer();
        }
    }

    void CheckForPlayer()
    {
        playerInRange = Utility.CheckIfTransformInRange(player, transform, playerRange);
        collectedAllArtifacts = itemBox.CollectedAllArtifacts();

        if (playerInRange)
        {
            if (itemBox.CollectedAllArtifacts())
            {
                furnaceText.text = completeText;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    furnaceText.text = string.Empty;
                    furnaceText_CG.alpha = 0f;
                    vibeChecker.EndSequence();
                    stopChecking = true;
                }
            }
            else
            {
                furnaceText.text = incompleteText;
            }
        }

        furnaceText_CG.alpha = playerInRange ? 1f : 0f;
    }
}
