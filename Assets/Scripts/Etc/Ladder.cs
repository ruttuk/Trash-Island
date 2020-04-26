using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Ladder : MonoBehaviour
{
    public Ladder sibling;
    God m_God;
    Rigidbody player;

    private float maxLadderHeight, minLadderHeight;

    private float ladderRange = 0.5f;
    private bool cooldown;
    private float playerMass;
    private float groundOffset;

    Collider higherCollider;
    bool higher;

    void Start()
    {
        cooldown = false;
        m_God = FindObjectOfType<God>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerMass = player.mass;

        groundOffset = transform.parent.parent.position.y;

        if (transform.position.y > sibling.transform.position.y)
        {
            maxLadderHeight = transform.position.y;
            minLadderHeight = sibling.transform.position.y;

            higherCollider = GetComponent<BoxCollider>();
            higher = true;
        }
        else
        {
            minLadderHeight = transform.position.y;
            maxLadderHeight = sibling.transform.position.y;
            higherCollider = sibling.GetComponent<Collider>();
        }
    }

    void Update()
    {
        // We only check for ladder updates if the following is true:
        // 1. player is currently in control.
        // 2. ladder is not in cooldown state.
        // 3. ladder is either not in use, or the currently used ladder is this one or our sibling.
        if (m_God.playerControl && !cooldown && (m_God.ladderInUse == null || m_God.ladderInUse == this || m_God.ladderInUse == sibling))
        {
            CheckLadderStatus();
        }
    }

    void CheckLadderStatus()
    {
        if (Utility.CheckIfTransformInRange(player.transform, transform, ladderRange) && !m_God.ladderControl)
        {
            player.isKinematic = true;
            m_God.ladderControl = true;
            m_God.ladderInUse = this;
            m_God.descendingLadder = higher; // if this is the higher end of the ladder, we start descending
        }

        if (player.position.y > maxLadderHeight + 1.5f || player.position.y < minLadderHeight)
        {
            cooldown = true;

            // temporarily turn on the collider on the higher ladder end, so player does not fall to ground
            if (player.position.y > maxLadderHeight + ladderRange)
            {
                higherCollider.isTrigger = false;
                Debug.Log("Higher collider is no longer a trigger.");
            }

            player.isKinematic = false;
            m_God.ladderControl = false;
            m_God.ladderInUse = null;
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(2f);

        higherCollider.isTrigger = true;
        cooldown = false;
        Debug.Log("Cooldowns over...");
    }
}
