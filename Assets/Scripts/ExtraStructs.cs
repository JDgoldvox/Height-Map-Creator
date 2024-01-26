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
    public List<MeshFilter> meshFilters;
    public GameObject coreHolder;

    public UnderGroundCore(List<GameObject> inputCubeList)
    {
        coreHolder = new GameObject("core");
        meshFilters = new List<MeshFilter>();
        cubes = inputCubeList;
    }
}