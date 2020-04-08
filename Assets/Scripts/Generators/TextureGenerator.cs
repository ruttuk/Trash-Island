using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Vector2[] GeneratePoints(int size, int numPoints, float distanceFromEdge, IslandData.IslandGenerationType islandGenerationType)
    {
        Vector2[] points = new Vector2[numPoints];
 
        float half = size / 2f;
        int lowerBound = Mathf.RoundToInt(half * distanceFromEdge);
        int upperBound = size - lowerBound;

        if(islandGenerationType == IslandData.IslandGenerationType.Mountains)
        {
            points = GeneratePointsByCurve(numPoints, lowerBound, upperBound, distanceFromEdge);
        }
        else if(islandGenerationType == IslandData.IslandGenerationType.Random)
        {
            points = GenerateRandomPoints(numPoints, lowerBound, upperBound);
        }     

        return points;
    }

    static Vector2[] GenerateRandomPoints(int numPoints, int lowerBound, int upperBound)
    {
        Vector2[] points = new Vector2[numPoints];

        System.Random prng = new System.Random();

        float seed = prng.Next(0, 999999);
        float x, y;

        for (int i = 0; i < numPoints; i++)
        {
            x = prng.Next(lowerBound, upperBound);
            y = prng.Next(lowerBound, upperBound);

            points[i] = new Vector2(x, y);
        }

        return points;
    }

    static Vector2[] GeneratePointsByCurve(int numPoints, int lowerBound, int upperBound, float distanceFromEdge)
    {
        Vector2[] points = new Vector2[numPoints];

        System.Random prng = new System.Random();
        float x, y, xt, xv, yt, yv;

        AnimationCurve mountainRangeX = new AnimationCurve();
        AnimationCurve mountainRangeY = new AnimationCurve();

        mountainRangeX.AddKey(0f, distanceFromEdge);
        mountainRangeY.AddKey(0f, distanceFromEdge);
        mountainRangeX.AddKey(1f, 1 - distanceFromEdge);
        mountainRangeY.AddKey(1f, 1 - distanceFromEdge);

        for (int i = 0; i < 6; i++)
        {
            xt = prng.Next(2, 999999) * 0.000001f;
            xv = prng.Next(2, 999999) * 0.000001f;
            yt = prng.Next(2, 999999) * 0.000001f;
            yv = prng.Next(2, 999999) * 0.000001f;

            mountainRangeX.AddKey(xt, xv);
            mountainRangeY.AddKey(yt, yv);
        }

        for (int i = 0; i < numPoints; i++)
        {
            x = Mathf.Clamp(mountainRangeX.Evaluate((float)(i + 1) / numPoints) * upperBound , lowerBound + 5, upperBound);
            y = Mathf.Clamp(mountainRangeY.Evaluate((float)(i + 1) / numPoints) * upperBound, lowerBound + 5, upperBound);

            points[i] = new Vector2(x, y);
        }

        return points;
    }

    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        //texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }


    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.blue, Color.yellow, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }
}
