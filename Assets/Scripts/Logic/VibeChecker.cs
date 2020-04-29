using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibeChecker : MonoBehaviour
{
    public RegionGenerator regionGenerator;
    public Rigidbody playerCharacter;
    public Ladder ladderInUse;
    public CanvasGroup credits_CG;
    public CanvasGroup fadePanel_CG;
    private Boat boat;

    public bool fixedCamera = false;
    public bool descendingLadder = false;
    public bool ladderControl = false;
    public bool playerControl;
    private bool playerAwake;

    private const string smokeParticleTag = "SMOKE_PARTICLE";
    private const string endingCameraPointTag = "END_CAMERA_POINT";
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

    public void EndSequence()
    {
        GameObject[] smokeStacks = GameObject.FindGameObjectsWithTag(smokeParticleTag);

        for(int i = 0; i < smokeStacks.Length; i++)
        {
            smokeStacks[i].GetComponent<ParticleSystem>().Play();
        }

        fixedCamera = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Camera.main.transform.SetParent(null);
        Camera.main.transform.position = GameObject.FindGameObjectWithTag(endingCameraPointTag).transform.position;
        Camera.main.transform.LookAt(playerCharacter.transform);

        StartCoroutine(Utility.Fade(credits_CG, true, 0.01f, 3f));
        StartCoroutine(Utility.Fade(fadePanel_CG, true, 0.01f, 10f));
        StartCoroutine(Utility.Fade(credits_CG, false, 0.05f, 10f));
        StartCoroutine(Utility.Load(0, 12f));
    }
}
