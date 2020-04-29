using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibeChecker : MonoBehaviour
{
    public RegionGenerator regionGenerator;
    public Rigidbody playerCharacter;
    public Ladder ladderInUse;

    private Boat boat;

    public bool descendingLadder = false;
    public bool ladderControl = false;
    public bool playerControl;
    private bool playerAwake;

    private const string startingPlayerPointTag = "PLAYER_SPAWN";
    private const string startingBoatPointTag = "BOAT_SPAWN";
    private const float interactionRange = 3f;
    private const float startingBoatYPosition = 2.2f;

    void Awake()
    {
        boat = FindObjectOfType<Boat>();
        playerCharacter = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<Rigidbody>();
        playerControl = true;
    }

    void Start()
    {   
        SetInitialPositionByTag(playerCharacter.transform, startingPlayerPointTag);
        SetInitialPositionByTag(boat.transform, startingBoatPointTag);

        playerCharacter.transform.SetParent(boat.transform);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && !ladderControl)
        {
            if(playerControl)
            {
                if (Utility.CheckIfTransformInRange(playerCharacter.transform, boat.transform, interactionRange))
                {
                    PlayerEmbark();
                }
            }
            else
            {
                PlayerDisembark();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Break();
        }
    }

    private void PlayerEmbark()
    {
        Debug.Log("Boat in range! Embarking...");
        playerCharacter.transform.position = new Vector3(boat.transform.position.x, startingBoatYPosition + 1f, boat.transform.position.z);
        playerCharacter.isKinematic = true;
        playerControl = false;
        playerCharacter.GetComponent<Light>().intensity = 0f;
        boat.EmbarkBoat();
    }

    private void PlayerDisembark()
    {
        Debug.Log("Disembarking boat!");
        playerCharacter.isKinematic = false;
        playerControl = true;
        playerCharacter.GetComponent<Light>().intensity = 2f;
        boat.DisembarkBoat();
    }

    private void SetInitialPositionByTag(Transform initialTransform, string startingTag)
    {
        Transform startingPoint = GameObject.FindGameObjectWithTag(startingTag).transform;

        if(startingPoint != null)
        {
            initialTransform.position = startingPoint.position;
        }
        else
        {
            Debug.Log($"Couldn't find starting point for {initialTransform}!");
        }
    }
}
