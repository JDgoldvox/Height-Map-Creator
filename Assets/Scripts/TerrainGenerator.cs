using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    float temp = 0;

    private float frequency = 0.1f;
    private float scale = 10f;

    [SerializeField] private WorldSize worldSize = new WorldSize();

    [SerializeField] private GameObject grassCube;
    [SerializeField] private GameObject dirtCube;
    [SerializeField] private GameObject shallowWaterCube;
    [SerializeField] private GameObject deepWaterCube;

    private List<GameObject> worldCubes = new List<GameObject>();

    private void Awake()
    {

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

        //loop through all blocks on map
        for(int x = 0; x < worldSize.x; x++)
        {
            for(int z = 0; z < worldSize.z; z++)
            {
                float adjustedX = x * frequency; // Adjust the scale as needed
                float adjustedZ = z * frequency; // Adjust the scale as needed

                noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ);
                GameObject newCubeObj = Instantiate(grassCube, new Vector3(x, noiseNumber * scale, z), Quaternion.identity);
                worldCubes.Add(newCubeObj);

                Debug.Log($"created new cube with: {noiseNumber} as y\n");
            }
        }
    }
}