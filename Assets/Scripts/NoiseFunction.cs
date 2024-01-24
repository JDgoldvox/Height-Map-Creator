using UnityEngine;

static class NoiseFunction
{
   public static float GenerateNoise(float x, float z)
    {
        return Mathf.PerlinNoise(x, z); 
    }
}