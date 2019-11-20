using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseMapGenerator
{
    public static float[,] GenerateNoiseMap(float[,] weightMap, float scale)
    {
        // Find the depth and width assuming a rectangular map
        int mapWidth = weightMap.GetLength(0);
        int mapDepth = weightMap.GetLength(1);

        // Create an empty noise map with the the same size as the weightMap
        float[,] noiseMap = new float[mapDepth, mapWidth];
        float sampleXOffset = Random.value * 100;
        float sampleZOffset = Random.value * 100;

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
            float noise = 0;
            if (weightMap[zIndex, xIndex] > 0)
            {
                // Calculate sample indices based on the coordinates and the scale
                float sampleX = xIndex / scale + sampleXOffset;
                float sampleZ = zIndex / scale + sampleZOffset;
                // Generate noise value using PerlinNoise
                noise = Mathf.PerlinNoise(sampleX, sampleZ) * weightMap[zIndex, xIndex];
            }
            
            noiseMap [zIndex, xIndex] = noise;
            }
        }

        return noiseMap;
    }
}   