using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Junko : MonoBehaviour
{
    private NoiseTarget[] noiseTargets;
    private NoiseTarget currentPrey;

    private bool hibernating;
    private bool stalking;
    private bool searching;

    private NavMeshAgent m_Agent;

    // How close does Junko get to target before "searching"?
    private float searchRange = 100f;

    void Start()
    {
        hibernating = true;
        currentPrey = null;

        m_Agent = GetComponent<NavMeshAgent>();

        // Get a reference to all the noise targets in scene.
        noiseTargets = FindObjectsOfType<NoiseTarget>();
        InvokeRepeating("FindNextPrey", 2, 4);
    }

    private void FindNextPrey()
    {
        int highestDecibelTarget = 0;

        for (int i = 0; i < noiseTargets.Length; i++)
        {
            if(noiseTargets[i].currentDecibels > highestDecibelTarget && noiseTargets[i].targetActive)
            {
                highestDecibelTarget = noiseTargets[i].currentDecibels;
                currentPrey = noiseTargets[i];
            }
        }

        if(currentPrey == null && !searching)
        {
            Hibernate();
        }
        else if(currentPrey == null && searching)
        {
            Search();
        }
        else
        {
            if(currentPrey.targetActive)
            {
                Stalk();
            }
            else
            {
                currentPrey = null;
            }
        }
    }

    private void Hibernate()
    {
        hibernating = true;
        m_Agent.isStopped = true;
        Debug.Log("Junko is hibernating...");
    }

    private void Stalk()
    {
        // set destination to current prey
        // if in range, stop stalking.
        if(!searching)
        {
            stalking = true;
            m_Agent.isStopped = false;
            m_Agent.SetDestination(currentPrey.transform.position);
        }

        float distanceFromPrey = (currentPrey.transform.position - transform.position).sqrMagnitude;
        Debug.Log($"Current Prey is {currentPrey.noiseTargetData.name}! Stalking! Distance from prey: {distanceFromPrey}");

        if(distanceFromPrey < searchRange && !searching)
        {
            Search();
        }
    }

    private void Search()
    {
        m_Agent.isStopped = true;
        searching = true;
        Debug.Log("Prey is in range! Searching...");
        Invoke("GiveUpSearch", 10f);
    }

    private void GiveUpSearch()
    {
        m_Agent.isStopped = false;
        searching = false;
        Debug.Log("Giving up search...");
    }
}
