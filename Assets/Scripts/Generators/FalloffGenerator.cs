using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    static float[,] falloffMap;

    public static float[,] GenerateFalloffMap(int size, Vector2[] randomPoints)
    {
        falloffMap = new float[size, size];

        for (int i = 0; i < size; i ++)
        {
            for (int j = 0; j < size; j += 2)
            {
                // The reason for the following is two-fold:
                // 1, it saves time. This was the original reason.
                // By sampling every other vertice, we cut the calculations in half more or less.
                // But it also solves another problem by giving the meshes a naturally stepped shape
                // This makes it easier for the player to ascend to peaks.
                falloffMap[i, j] = AggregateDistance(randomPoints, i, j, size, i * j);
                falloffMap[i, j + 1] = falloffMap[i, j];
            }
        }

        return falloffMap;
    }

    static float DistanceFromRandomPoint(Vector2 point, int x, int y, int size, float highstep, float midstep, float lowstep)
    {
        float a2, b2, res;

        a2 = (point.x - x) * (point.x - x);
        b2 = (point.y - y) * (point.y - y);

        res = Mathf.Sqrt(a2 + b2);

        res = (1 / (res + 1));

        if(res == 1)
        {
            res = 0.5f;
        }
        else if(res >= highstep)
        {
            res *= 0.8f;
        }
        else if(res >= midstep)
        {
            res *= 0.6f;
        }
        else if(res >= lowstep)
        {
            res *= 0.4f;
        }
        else
        {
            res = 0f;
        }

        return res;
    }

    static float AggregateDistance(Vector2[] points, int x, int y, int size, float seed)
    {
        float sinAdjuster = 0.02f / size;

        float aggregate = 0f;
        float highstep = 0.01f + Mathf.Sin(seed) * sinAdjuster;
        float midstep = 0.25f + Mathf.Sin(seed) * sinAdjuster;
        float lowstep = 0.3f + Mathf.Sin(seed) * sinAdjuster;

        for(int i = 0; i < points.Length; i++)
        {
            aggregate += DistanceFromRandomPoint(points[i], x, y, size, highstep, midstep, lowstep);
        }

        return aggregate;
    }
}
