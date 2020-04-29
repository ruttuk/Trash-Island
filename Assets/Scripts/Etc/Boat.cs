using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoiseTarget))]
public class Boat : MonoBehaviour
{
    public float turnForce = 2f;
    public float buckForce = 0.5f;
    public float slowDownForce = 5f;

    private NoiseTarget m_NoiseTarget;

    Rigidbody rb;
    float thrust = 15f;
    float maxVelocity = 17f;

    float bobiness = 0.005f;
    float bobLimit = 0.03f;
    float initialYPosition = 2.2f;
    float bobRange = 1.1f;

    Camera main;
    Transform playerTransform;
    VibeChecker vibeChecker;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_NoiseTarget = GetComponent<NoiseTarget>();
        vibeChecker = FindObjectOfType<VibeChecker>();

        main = Camera.main;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if(!vibeChecker.playerControl && !vibeChecker.ladderControl)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if(!Mathf.Approximately(x, 0))
            {
                if(rb.velocity.sqrMagnitude > 0f)
                {
                    Vector3 turnAngle = Vector3.up * Time.deltaTime * x * turnForce;
                    transform.Rotate(turnAngle);
                    Quaternion currentRot = transform.rotation;

                    // let's rotate the boat around it's right axis by turn angle

                    /*
                    Vector3 targetAngle = transform.forward;

                    if (targetAngle.z < 10f)
                    {
                        targetAngle.z += x > 0 ? -10f : 10f;
                        Quaternion targetRot = Quaternion.Euler(targetAngle);
                        transform.rotation = Quaternion.RotateTowards(currentRot, targetRot, Time.deltaTime * buckForce);
                    }
                    */

                    Vector3 velocity = rb.velocity;
                    //rb.velocity = Vector3.zero;
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

            // Boats moving, making a lotta noise.
            /*
            if (rb.velocity.sqrMagnitude <= 0.1f)
            {
                if(m_NoiseTarget.targetActive)
                {
                    m_NoiseTarget.DeactivateTarget();
                }
            }
            else
            {
                m_NoiseTarget.ActivateTarget(rb.velocity.magnitude / maxVelocity);
            }
            */
        }
        else
        {
            if(!Utility.CheckIfTransformInRange(playerTransform, transform, bobRange))
            {
                Bob();
            }
        }
    }

    public void SetInitialPositionAndVelocity(Vector3 position)
    {
        transform.position = position;
        rb.velocity = Vector3.zero;
    }

    public void DisembarkBoat()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        //m_NoiseTarget.DeactivateTarget();
    }

    public void EmbarkBoat()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
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
