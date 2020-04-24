using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform boat;

    void Start()
    {
        // set position at bottom corner of screen
        Vector3 desiredScreenPos = new Vector3(25, 25, Camera.main.nearClipPlane);
        Vector3 desiredWorldPos = Camera.main.ScreenToWorldPoint(desiredScreenPos);

        desiredWorldPos += Camera.main.transform.forward * 0.01f;
        transform.position = desiredWorldPos;
    }

    void Update()
    {
        transform.LookAt(boat);
    }
}
