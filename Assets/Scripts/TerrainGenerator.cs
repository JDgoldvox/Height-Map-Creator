using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [HideInInspector] public float frequency = 1.0f;
    [HideInInspector] public float scale = 1.0f;

    private WorldGeneratorControls S_worldGeneratorControls;

    [SerializeField] private WorldSize worldSize = new WorldSize();

    [SerializeField] private GameObject grassCube;
    [SerializeField] private GameObject dirtCube;
    [SerializeField] private GameObject shallowWaterCube;
    [SerializeField] private GameObject deepWaterCube;

    private List<GameObject> grassCubes = new List<GameObject>();
    private List<GameObject> underGroundCubes = new List<GameObject>();

    private void Awake()
    {
        S_worldGeneratorControls = GetComponent<WorldGeneratorControls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateHeightMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateHeightMap()
    {
        float noiseNumber = 0;

        //Top grass layer

        //loop through all blocks on map
        for(int x = 0; x < worldSize.x; x++)
        {
            for(int z = 0; z < worldSize.z; z++)
            {
                float adjustedX = x / worldSize.x; 
                float adjustedZ = z / worldSize.z;
                adjustedX *= frequency; 
                adjustedZ *= frequency; 

                noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ);
                GameObject newCubeObj = Instantiate(grassCube, new Vector3(x, noiseNumber * scale, z), Quaternion.identity);
                grassCubes.Add(newCubeObj);
            }
        }

        //underground layer
        //loop through all blocks on map
        //foreach(GameObject obj in grassCubes)
        //{
        //    //place dirt blocks below for some height
        //    int height = 10;
        //    for(int y = 0; y < height; y++)
        //    {
        //        var objPos = obj.transform.position;
        //        GameObject newCubeObj = Instantiate(dirtCube, new Vector3(objPos.x, objPos.y - (float)y, objPos.z), Quaternion.identity);
        //        underGroundCubes.Add(newCubeObj);
        //    }
        //}
    }

    public void UpdateBlocks()
    {
        float noiseNumber = 0;

        //loop through all blocks on map
        foreach(var obj in grassCubes)
        {
            Vector3 objPosition = obj.transform.position;

            float adjustedX = objPosition.x / worldSize.x;
            float adjustedZ = objPosition.z / worldSize.z;
            adjustedX *= frequency;
            adjustedZ *= frequency;

            noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ);
            obj.transform.position = new Vector3(objPosition.x, noiseNumber * scale, objPosition.z);
        }
    }
}