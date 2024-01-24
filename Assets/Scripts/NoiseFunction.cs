using UnityEngine;

static class NoiseFunction
{
   public static float GenerateNoise(float x, float y)
    {
        return Mathf.PerlinNoise(x, y); 
    }
}
