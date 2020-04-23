using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform boat;

    void Update()
    {
        transform.LookAt(boat);
    }
}
