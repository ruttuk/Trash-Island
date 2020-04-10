﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    public God god;
    public float turnForce = 2f;
    public float buckForce = 0.5f;

    Rigidbody rb;
    float thrust = 15f;
    float maxVelocity = 11f;

    float bobiness = 0.005f;
    float bobLimit = 0.03f;
    float initialYPosition = 2.2f;
    float bobRange = 1.1f;

    Camera main;
    Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        main = Camera.main;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if(!god.playerControl && !god.ladderControl)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if(!Mathf.Approximately(x, 0))
            {
                if(rb.velocity.magnitude > 0f)
                {
                    Vector3 turnAngle = Vector3.up * Time.deltaTime * x * turnForce;
                    transform.Rotate(turnAngle);
                    Quaternion currentRot = transform.rotation;

                    Vector3 targetAngle = transform.forward;

                    if (targetAngle.z < 10f)
                    {
                        targetAngle.z += x > 0 ? -10f : 10f;
                        Quaternion targetRot = Quaternion.Euler(targetAngle);
                        transform.rotation = Quaternion.RotateTowards(currentRot, targetRot, Time.deltaTime * buckForce);
                    }

                    Vector3 velocity = rb.velocity;
                    rb.velocity = Vector3.zero;
                    rb.velocity = transform.forward * velocity.magnitude;
                }
            }

            if (!Mathf.Approximately(y, 0))
            {
                if (rb.velocity.magnitude < maxVelocity)
                {
                    Vector3 targetDirection = transform.forward * y * thrust;
                    rb.AddForce(targetDirection);
                }
            }
        }
        else
        {
            if(!god.CheckIfTransformInRange(playerTransform, transform, bobRange))
            {
                Bob();
            }
        }
    }

    void Bob()
    {
        if(transform.position.y > initialYPosition || transform.position.y < initialYPosition - bobLimit)
        {
            bobiness *= -1;
        }

        float y = transform.position.y;
        y += bobiness * Time.deltaTime * 3.5f;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
