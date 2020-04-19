using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public RegionGenerator regionGenerator;
    public Rigidbody playerCharacter;
    private Boat boat;

    bool playerAwake;

    public bool descendingLadder = false;
    public bool ladderControl = false;
    public bool playerControl = false;

    float interactionRange = 3f;
    float startingBoatYPosition = 2.2f;

    void Awake()
    {
        boat = FindObjectOfType<Boat>();

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
                boat.SetInitialPositionAndVelocity(new Vector3(randomOceanPosition.x, startingBoatYPosition, randomOceanPosition.z));
                playerCharacter.transform.position = new Vector3(randomOceanPosition.x, startingBoatYPosition + 1f, randomOceanPosition.z);

                Debug.Log("Setting boat position at: " + randomOceanPosition);
            }
        }

        if(Input.GetKeyDown(KeyCode.E) && !ladderControl)
        {
            if(playerControl)
            {
                // embark on boat
                bool boatInRange = CheckIfTransformInRange(playerCharacter.transform, boat.transform, interactionRange);
                if (boatInRange)
                {
                    // press 'e' to embark
                    Debug.Log("Boat in range! Embarking...");
                    playerCharacter.transform.position = new Vector3(boat.transform.position.x, startingBoatYPosition + 1f, boat.transform.position.z);
                    playerCharacter.transform.SetParent(boat.transform);
                    playerCharacter.isKinematic = true;
                    playerControl = false;

                    boat.EmbarkBoat();
                    //boat.velocity = Vector3.zero;
                    //boat.isKinematic = false;
                }

            }
            else
            {
                Debug.Log("Disembarking boat!");
                // disembark boat
                playerCharacter.isKinematic = false;
                playerControl = true;
                //boat.isKinematic = true; ;
                playerCharacter.transform.SetParent(null);

                boat.DisembarkBoat();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Break();
        }
    }

    public bool CheckIfTransformInRange(Transform target, Transform source, float range)
    {
        Collider[] hitColliders = Physics.OverlapSphere(target.position, range);

        foreach (Collider col in hitColliders)
        {
            if (col.transform.Equals(source))
            {
                return true;
            }
        }
        return false;
    }
}
