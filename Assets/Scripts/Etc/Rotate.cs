using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed = 3f;

    void Update()
    {
        transform.Rotate(Vector3.right, Time.deltaTime * rotateSpeed);
    }
}
