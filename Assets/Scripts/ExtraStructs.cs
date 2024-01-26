using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldSize
{
    public float x;
    public float z;
    public WorldSize(float x = 0, float z = 0)
    {
        this.x = x;
        this.z = z;
    }
}

[System.Serializable]
public struct RangeWithMinMax
{
    public float min;
    public float max;
    public RangeWithMinMax(float min = 0, float max = 0)
    {
        this.min = min;
        this.max = max;
    }
}

public class UnderGroundCore
{
    public List<GameObject> cubes;

    public UnderGroundCore()
    {
        cubes = new List<GameObject>();
    }
}