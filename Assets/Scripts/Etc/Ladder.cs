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

    Collider higherCollider;
    bool higher;

    void Start()
    {
        cooldown = false;
        m_God = FindObjectOfType<God>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        playerMass = player.mass;

        if( transform.position.y > sibling.transform.position.y)
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
        if(m_God.playerControl && !cooldown)
        {
            if (m_God.CheckIfTransformInRange(player.transform, transform, ladderRange) && !m_God.ladderControl)
            {
                player.isKinematic = true;
                m_God.ladderControl = true;

                m_God.descendingLadder = higher; // if this is the higher end of the ladder, we start descending
            }

            if (player.position.y > maxLadderHeight + 1.5f || player.position.y < minLadderHeight)
            {
                // temporarily turn on the collider on the higher ladder end, so player does not fall to ground
                if(player.position.y > maxLadderHeight + ladderRange)
                {
                    higherCollider.isTrigger = false;
                }

                player.isKinematic = false;
                cooldown = true;
                m_God.ladderControl = false;
                StartCoroutine(Cooldown());
            }
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
