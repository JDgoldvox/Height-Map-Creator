using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    float temp = 0;

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
                float scaledX = x * 0.4f; // Adjust the scale as needed
                float scaledZ = z * 0.1f; // Adjust the scale as needed

                noiseNumber = NoiseFunction.GenerateNoise(scaledX, scaledZ);
                GameObject newCubeObj = Instantiate(grassCube, new Vector3(x, noiseNumber, z), Quaternion.identity);
                worldCubes.Add(newCubeObj);

                Debug.Log($"created new cube with: {noiseNumber} as y\n");
            }
        }
    }
}