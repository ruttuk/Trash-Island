using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    public LineRenderer line1;
    public LineRenderer line2;

    public GameObject telephonePolePrefab;
    public int numPoles = 3;
    public float separation = 20f;
    public int poleRotation = 5;

    Transform[] receivers1;
    Transform[] receivers2;

    void Start()
    {
        line1.startWidth = 0.05f;
        line1.endWidth = 0.05f;

        line2.startWidth = 0.05f;
        line2.endWidth = 0.05f;

        receivers1 = new Transform[numPoles];
        receivers2 = new Transform[numPoles];

        line1.positionCount = 0;
        line2.positionCount = 0;

        GenerateLandline();
        ConnectTheDots();
    }

    void GenerateLandline()
    {
        Vector3 spawnPos = new Vector3(11f, 0f, 0f);
        Vector3 spawnRot = Vector3.zero;

        GameObject spawnedPole;

        System.Random prng = new System.Random();

        for(int i = 0; i < numPoles; i++)
        {
            spawnedPole = SpawnPole(spawnPos);
            spawnedPole.transform.Rotate(spawnedPole.transform.right, prng.Next(-poleRotation, poleRotation));
            spawnedPole.transform.Rotate(spawnedPole.transform.up, -45f);

            receivers1[i] = spawnedPole.transform.GetChild(0);
            receivers2[i] = spawnedPole.transform.GetChild(1);

            spawnPos.z += separation;
            spawnPos.x += separation;
        }
    }

    void ConnectTheDots()
    {
        int line1offset = 0;
        int line2offset = 0;

        for(int i = 0; i < numPoles - 1; i++)
        {
            line1offset += AddPoints(line1, receivers1[i], receivers1[i + 1], line1offset);
            line2offset += AddPoints(line2, receivers2[i], receivers2[i + 1], line2offset);
        }
    }

    // Spawn a set number of telephone poles - say 5 for now.
    
    GameObject SpawnPole(Vector3 spawnPosition)
    {
        return Instantiate(telephonePolePrefab, spawnPosition, Quaternion.identity, transform);
    }

    // First determine how many points between start and end point we need.
    // return the number of new points added
    int AddPoints(LineRenderer lineRenderer, Transform startPoint, Transform endPoint, int lineOffset)
    {
        int numPoints = Mathf.RoundToInt(Vector3.Distance(startPoint.position, endPoint.position));

        // Make sure number of points is even
        if(numPoints % 2 != 0)
        {
            numPoints++;
        }

        lineRenderer.positionCount += numPoints;

        float xDistance, yDistance, zDistance, xIncrement, yIncrement, zIncrement;

        xDistance = endPoint.position.x - startPoint.position.x;
        yDistance = endPoint.position.y - startPoint.position.y;
        zDistance = endPoint.position.z - startPoint.position.z;

        Vector3 pointPos;

        float a =  0.01f;
        float b =  0.01f;
        float slackCount = 0f;
        float slack;
        float intercept = startPoint.position.y - startPoint.position.y / 2f;

        // we want to decrease the y position towards the center to make the cable "hang" due to gravity.
        // the lowest point will be at the center point between start and end
        // ax^2 + bx
        slackCount = -numPoints / 2;

        for (int i = 0; i < numPoints; i++, slackCount++)
        {
            xIncrement = (xDistance / numPoints) * i;
            yIncrement = (yDistance / numPoints) * i;
            zIncrement = (zDistance / numPoints) * i;

            slack = a * slackCount * slackCount + b * slackCount + intercept;

            yIncrement += slack;

            pointPos = new Vector3(startPoint.position.x + xIncrement, yIncrement, startPoint.position.z + zIncrement);
            lineRenderer.SetPosition(i + lineOffset, pointPos);
        }
        
        float diff = startPoint.position.y < lineRenderer.GetPosition(lineOffset).y ? startPoint.position.y - lineRenderer.GetPosition(lineOffset).y : lineRenderer.GetPosition(lineOffset).y - startPoint.position.y;

        // correct the curve
        for(int i = 0; i < numPoints; i++)
        {
            pointPos = lineRenderer.GetPosition(i + lineOffset);
            pointPos.y -= diff;
            lineRenderer.SetPosition(i + lineOffset, pointPos);
        }

        return numPoints;
    }
}
