using UnityEngine;
using System.Collections.Generic;

static class NoiseFunction
{
   public static float GenerateNoise(float x, float z, float numOfOctaves)
    {
        //quick exit
        if(numOfOctaves == 1)
        {
            return Mathf.PerlinNoise(x, z);
        }

        //convert from float to int
        int numberOfOctaves = (int)numOfOctaves;

        Stack<float> influence = new Stack<float>();

        //depending on how many octaves, generate that many items in influence
        int startingInfluence = 1;
        influence.Push(startingInfluence);
        for (int i = 0; i < numOfOctaves; i++)
        {
            startingInfluence /= 2;
            influence.Push(startingInfluence);
        }

        //calculate perlin noise depending on how much influence 
        float perlinCalculated = 0;
        float frequency = 1;

        int maxStack = influence.Count;
        for(int i = 0; i < maxStack; i++)
        {
            float currentInfluence = influence.Pop();
            perlinCalculated += (1/currentInfluence) * Mathf.PerlinNoise(x * frequency, z * frequency);
            frequency *= 2;
        }

        return perlinCalculated;
    }
}