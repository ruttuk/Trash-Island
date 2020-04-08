﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public RegionGenerator regionGenerator;
    public Rigidbody playerCharacter;
    public Rigidbody boat;

    bool playerAwake;

    public bool playerControl = false;

    float interactionRange = 3f;

    float startingBoatYPosition = 2.2f;

    void Awake()
    {
        playerCharacter.isKinematic = true;
        playerCharacter.transform.SetParent(boat.transform);
        playerAwake = false;
    }

    void Update()
    {
        if(!playerAwake)
        {
            playerAwake = true;
            playerCharacter.isKinematic = true;

            Vector3 randomOceanPosition = Vector3.zero; //regionGenerator.GetRandomPointOfTerrainType(0, ocean);

            if(randomOceanPosition != Vector3.negativeInfinity)
            {
                boat.transform.position = new Vector3(randomOceanPosition.x, startingBoatYPosition, randomOceanPosition.z);
                boat.velocity = Vector3.zero;
                playerCharacter.transform.position = new Vector3(randomOceanPosition.x, startingBoatYPosition + 1f, randomOceanPosition.z);

                Debug.Log("Setting boat position at: " + randomOceanPosition);
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(playerControl)
            {
                // embark on boat
                bool boatInRange = CheckIfPlayerInRange(interactionRange);
                if (boatInRange)
                {
                    // press 'e' to embark
                    Debug.Log("Boat in range! Embarking...");
                    playerCharacter.transform.position = new Vector3(boat.transform.position.x, startingBoatYPosition + 1f, boat.transform.position.z);
                    playerCharacter.transform.SetParent(boat.transform);
                    playerCharacter.isKinematic = true;
                    playerControl = false;
                    boat.velocity = Vector3.zero;
                    boat.isKinematic = false;
                }

            }
            else
            {
                Debug.Log("Disembarking boat!");
                // disembark boat
                playerCharacter.isKinematic = false;
                playerControl = true;
                boat.isKinematic = true; ;
                playerCharacter.transform.SetParent(null);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Break();
        }
    }

    public bool CheckIfPlayerInRange(float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerCharacter.transform.position, range);

        foreach (Collider col in hitColliders)
        {
            if (col.transform.Equals(boat.transform))
            {
                return true;
            }
        }
        return false;
    }
}