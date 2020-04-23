using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera main;

    void Start()
    {
        main = Camera.main;
    }

    void Update()
    {
        transform.forward = -main.transform.forward;
    }
}
