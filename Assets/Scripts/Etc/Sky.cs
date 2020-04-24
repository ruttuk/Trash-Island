using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Sky : MonoBehaviour
{
    public float speed = 0.7f;
    public float maxIntensity = 1f;
    private float runningTotal = 0f;

    private bool daytime;
    private bool rising;
    Light skylight;

    private float lightIntensityAdjuster = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        skylight = GetComponent<Light>();
        daytime = true;
        skylight.color = Color.green;
        skylight.intensity = 0;
        rising = true;
    }

    void FixedUpdate()
    {
        if(transform.rotation.eulerAngles.x > 180f)
        {
            daytime = !daytime;
            Quaternion moon = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            transform.rotation = moon;
            rising = true;
            lightIntensityAdjuster *= -1;

            if (daytime)
            {
                Debug.Log("Sunrise...");
                skylight.color = Color.yellow;
            }
            {
                Debug.Log("Moonrise...");
                skylight.color = Color.green;
            }
        }
        if(transform.rotation.eulerAngles.x == 90f)
        {
            rising = false;
            lightIntensityAdjuster *= -1;
            Debug.Log("Setting now...");
            skylight.intensity = maxIntensity - 0.001f;
        }

        float rot = Time.deltaTime * speed;
        transform.Rotate(Vector3.right, rot);
        skylight.intensity += Time.deltaTime * lightIntensityAdjuster;
    }
}
