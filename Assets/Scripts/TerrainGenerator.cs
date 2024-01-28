using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int underGroundHeight = 10;
    public float waterHeight = 0;
    [HideInInspector] public float landOctaves = 1;

    [HideInInspector]  public float landFrequency = 1.0f;
    [HideInInspector]  public float landScale = 1.0f;
    [HideInInspector]  public float waterFrequency = 1.0f;
    [HideInInspector]  public float waterScale = 0.0f;

    private WorldGeneratorControls S_worldGeneratorControls;

    [SerializeField] private WorldSize worldSize = new WorldSize();

    [SerializeField] private GameObject grassCube;
    [SerializeField] private GameObject dirtCube;
    [SerializeField] private GameObject shallowWaterCube;
    [SerializeField] private GameObject deepWaterCube;

    private bool showAllIndividualCubes = true;
    private bool showGiantMesh = true;

    Dictionary<GameObject, UnderGroundCore> world = new Dictionary<GameObject, UnderGroundCore>();
    List<GameObject> waterCubes = new List<GameObject>();

    List<GameObject> topLayerMegaMeshGameObject = new List<GameObject>();
    List<GameObject> bottomLayerMegaMeshGameObject = new List<GameObject>();
    List<GameObject> waterLayerMegaMeshGameObject = new List<GameObject>();

    private GameObject combinedMeshParent;

    GameObject bottomLayer;
    GameObject topLayer;
    GameObject waterLayer;

    float middleWaterHeight = 0;
    float highestTopLayerCube = float.MinValue, lowestBottomLayerCube = float.MaxValue;

    [HideInInspector] public bool enableWaves = true;
    [HideInInspector] public float totalWaveCooldown = 0.1f;
    [HideInInspector] public float currentWaveTimer = 1;
    private bool isWaveMovingPositiveFrequency = true;
    private float amountToMoveWave = 0.3f;

    private void Awake()
    {
        S_worldGeneratorControls = GetComponent<WorldGeneratorControls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bottomLayer = new GameObject("bottom layer");
        topLayer = new GameObject("top layer");
        waterLayer = new GameObject("water layer");

        combinedMeshParent = new GameObject("combined meshes");

        CreateHeightMap();
        HideAllIndividualCubes();
    }

    void Update()
    {
        //if waves are turned on, update waves to bounce between -10 to 10 in frequency
        if (enableWaves)
        {
            currentWaveTimer -= Time.deltaTime;

            //if cooldown is not on
            if (currentWaveTimer <= 0)
            {
                //ping pong wave frequency to 10 and -10
                //determine if it should move in positive or negative direction
                if (waterFrequency >= amountToMoveWave)
                {
                    //move in neg
                    isWaveMovingPositiveFrequency = true;
                }
                else if (waterFrequency >= -amountToMoveWave)
                {
                    //move in pos
                    isWaveMovingPositiveFrequency = false;
                }

                //move freq in positive direction
                if (isWaveMovingPositiveFrequency)
                {
                    waterFrequency += amountToMoveWave;
                }
                else
                {
                    waterFrequency -= amountToMoveWave;
                }

                //update wave!
                UpdateBlocks(true, true);

                //reset timer
                currentWaveTimer = totalWaveCooldown;
            }
        }
    }

    void CreateHeightMap()
    {
        //Instantiate cubes
        InstantiateCubes();
        UpdateBlocks(true, false);
    }

    void InstantiateCubes()
    {
        float noiseNumber = 0;
        List<GameObject> grassCubes = new List<GameObject>();
        
        //Top grass layer and water
        //loop through all blocks on map and create all top layer cubes
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int z = 0; z < worldSize.z; z++)
            {
                float adjustedX = (x / worldSize.x) * landFrequency;
                float adjustedZ = (z / worldSize.z) * landFrequency;

                //grass
                noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ, landOctaves);
                GameObject newCubeObj = Instantiate(grassCube, new Vector3(x, noiseNumber * landScale, z), Quaternion.identity);
                newCubeObj.transform.parent = topLayer.transform;
                grassCubes.Add(newCubeObj);

                //FIND HIGHEST BLOCK
                if(noiseNumber > highestTopLayerCube)
                {
                    highestTopLayerCube = noiseNumber;
                }

                //FIND LOWEST BLOCK
                if (noiseNumber < lowestBottomLayerCube)
                {
                    lowestBottomLayerCube = noiseNumber;
                }
            }
        }

        //underground layer
        //loop through all blocks on map
        List<GameObject> underGroundCubes = new List<GameObject>();
        List<UnderGroundCore> cores = new List<UnderGroundCore>();
        
        foreach (GameObject obj in grassCubes)
        {
            //place dirt blocks below for some height below one another
            List<GameObject> tempUnderGroundCubeList = new List<GameObject>();

            for (int y = 0; y < underGroundHeight; y++)
            {
                //create underground Cube
                var objPos = obj.transform.position;
                GameObject newCubeObj = Instantiate(dirtCube, new Vector3(objPos.x, objPos.y - 1 - (float)y, objPos.z), Quaternion.identity);
                newCubeObj.transform.parent = bottomLayer.transform;

                //add to cube array so it we can collect them
                tempUnderGroundCubeList.Add(newCubeObj);
            }

            //create new core and add the list in
            UnderGroundCore newCore = new UnderGroundCore(tempUnderGroundCubeList);

            //add the core to list
            cores.Add(newCore);
        }

        //map top layer to core cubes in world dic
        for (int i = 0; i < grassCubes.Count; i++)
        {
            //map top layer to underground layers
            world[grassCubes[i]] = cores[i];
        }

        //1. find difference, 2. find half of difference, 3. add to lowest cube height
        middleWaterHeight = highestTopLayerCube - Mathf.Abs(lowestBottomLayerCube);
        float tempH = middleWaterHeight / 2;
        middleWaterHeight = (lowestBottomLayerCube + tempH) * landScale;

        //WATER LAYER
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int z = 0; z < worldSize.z; z++)
            {
                float adjustedX = (x / worldSize.x) * waterFrequency;
                float adjustedZ = (z / worldSize.z) * waterFrequency;

                //water
                noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ, landOctaves); 
                GameObject newCubeObj2 = Instantiate(deepWaterCube, new Vector3(x, noiseNumber * waterScale + waterHeight + middleWaterHeight, z), Quaternion.identity);
                newCubeObj2.transform.parent = waterLayer.transform;
                waterCubes.Add(newCubeObj2);
            }
        }
    }

    public void UpdateBlocks(bool combine, bool updateOnlyWater)
    {
        float noiseNumber = 0;

        if (!updateOnlyWater)
        {
            //loop through all blocks on world
            foreach (var topCube in world.Keys)
            {
                //update all surface cube
                Vector3 topCubePos = topCube.transform.position;

                float adjustedX = (topCubePos.x / worldSize.x) * landFrequency;
                float adjustedZ = (topCubePos.z / worldSize.z) * landFrequency;

                noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ, landOctaves);
                topCube.transform.position = new Vector3(topCubePos.x, noiseNumber * landScale, topCubePos.z);

                //Find corrosponding core cubes
                List<GameObject> underGroundCubeList = world[topCube].cubes;

                for (int i = 0; i < underGroundCubeList.Count; i++)
                {
                    underGroundCubeList[i].transform.position = new Vector3(topCube.transform.position.x,
                        topCube.transform.position.y - i - 1,
                        topCube.transform.position.z);
                }

                //FIND HIGHEST BLOCK
                if (noiseNumber > highestTopLayerCube)
                {
                    highestTopLayerCube = noiseNumber;
                }

                //FIND LOWEST BLOCK
                if (noiseNumber < lowestBottomLayerCube)
                {
                    lowestBottomLayerCube = noiseNumber;
                }
            }
        }

        //1. find difference, 2. find half of difference, 3. add to lowest cube height
        middleWaterHeight = highestTopLayerCube - Mathf.Abs(lowestBottomLayerCube);
        float tempH = middleWaterHeight / 2;
        middleWaterHeight = (lowestBottomLayerCube + tempH) * landScale;

        //update all water blocks
        foreach (GameObject waterCube in waterCubes)
        {
            Vector3 waterLayerPos = waterCube.transform.position;

            float adjustedX = (waterLayerPos.x / worldSize.x) * waterFrequency;
            float adjustedZ = (waterLayerPos.z / worldSize.z) * waterFrequency;

            noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ, landOctaves);
            waterCube.transform.position = new Vector3(waterLayerPos.x, noiseNumber * waterScale + waterHeight + middleWaterHeight, waterLayerPos.z);
        }

        //combine top layer
        if (combine)
        {
            if (!updateOnlyWater)
            {
                List<GameObject> allTopCubes = new List<GameObject>();
                //get the list of all top cubes
                foreach (GameObject cube in world.Keys)
                {
                    allTopCubes.Add(cube);
                }

                CombineMesh(allTopCubes, topLayerMegaMeshGameObject);

                //combine underground layers

                //get the list of all cores cubes
                List<GameObject> allUnderGroundCubes = new List<GameObject>();

                foreach (UnderGroundCore core in world.Values)
                {
                    foreach (GameObject cube in core.cubes)
                    {
                        allUnderGroundCubes.Add(cube);
                    }
                }

                CombineMesh(allUnderGroundCubes, bottomLayerMegaMeshGameObject);
            }
                
            //combine water layer
            CombineMesh(waterCubes, waterLayerMegaMeshGameObject);
        }
    }
    void CombineMesh(List<GameObject> originalCubes, List<GameObject> megaMeshGameObject)
    {
        List<List<GameObject>> gameObjectLists = new List<List<GameObject>>();
        
        //making sure there is only 8192 cubes per mesh object
        if (originalCubes.Count > 2500)
        {
            List<GameObject> currentList = new List<GameObject>();

            //split original cubes into 2999 max lists and add them to gameObjectLists
            for (int i = 0; i < originalCubes.Count; i++)
            {
                //add cube
                currentList.Add(originalCubes[i]);

                //if its the last cube added or if max cube count reached
                if (currentList.Count == 2500 || i == originalCubes.Count - 1)
                {
                    //Debug.Log("list: " + currentList.Count);
                    //Maxed reached - add and reset
                    gameObjectLists.Add(currentList);
                    currentList = new List<GameObject>();
                }
            }
        }
        else //only add 1 item to list
        {
            gameObjectLists.Add(originalCubes);
        }

        //now we have lists inside a list, do something with these lists!
        ////////////////////////////////////////////////////////////////////////

        //create game objects to store these mega-meshes depending on how many lists are in the list
        //remove previous items
        if(megaMeshGameObject != null)
        {
            foreach(GameObject megaMesh in megaMeshGameObject)
            {
                Destroy(megaMesh);
            }
        }
        megaMeshGameObject.Clear();

        foreach (List<GameObject> list in gameObjectLists)
        {
            GameObject combinedMeshGameObject = new GameObject("combinedMesh");
            combinedMeshGameObject.SetActive(true); 
            megaMeshGameObject.Add(combinedMeshGameObject);
        }
        //////////////////////////////////////////////////////////////////////////

        //combine each list of game objects to make a mega mesh
        for (int i = 0; i < gameObjectLists.Count; i++)
        {
            List<GameObject> listOfCubes = gameObjectLists[i];
            GameObject firstCube = listOfCubes[0];

            //set parent
            megaMeshGameObject[i].transform.parent = combinedMeshParent.transform;

            List<MeshFilter> meshFilters = new List<MeshFilter>();

            // Make an array of combine instances
            var combinedInstance = new CombineInstance[listOfCubes.Count];

            // Store material and mesh renderer references outside the loop
            Material sharedMaterial = firstCube.GetComponent<MeshRenderer>().sharedMaterial;
            MeshRenderer meshRenderer = megaMeshGameObject[i].GetComponent<MeshRenderer>();

            // Set the meshes and transform in the combined instance
            for (int j = 0; j < listOfCubes.Count; j++)
            {
                MeshFilter meshFilter = listOfCubes[j].GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    combinedInstance[j].mesh = meshFilter.sharedMesh;
                    combinedInstance[j].transform = listOfCubes[j].transform.localToWorldMatrix;
                    meshFilters.Add(meshFilter);
                }
            }

            // Combine instances into mesh
            Mesh combinedGrassMesh = new Mesh();
            combinedGrassMesh.CombineMeshes(combinedInstance);

            // New game object
            Transform combinedTransform = megaMeshGameObject[i].transform;
            combinedTransform.position = new Vector3(0,0,0);
            combinedTransform.rotation = listOfCubes[0].transform.rotation;

            // Add or update MeshFilter
            MeshFilter combinedMeshFilter = megaMeshGameObject[i].GetComponent<MeshFilter>();
            if (combinedMeshFilter == null)
            {
                combinedMeshFilter = megaMeshGameObject[i].AddComponent<MeshFilter>();
            }
            combinedMeshFilter.mesh = combinedGrassMesh;

            // Add or update MeshRenderer
            if (meshRenderer == null)
            {
                meshRenderer = megaMeshGameObject[i].AddComponent<MeshRenderer>();
            }
            meshRenderer.material = sharedMaterial;
        }
    }

    public void ShowAllIndividualCubes()
    {
        if (showAllIndividualCubes)
        {
            return;
        }

        //enable top
        topLayer.SetActive(true);

        //enable bottom
        bottomLayer.SetActive(true);

        //enable waterLayer
        waterLayer.SetActive(true);

        showAllIndividualCubes = true;
    }

    public void HideAllIndividualCubes()
    {
        if (!showAllIndividualCubes)
        {
            return;
        }

        //disable top
        topLayer.SetActive(false);

        //disable bottom
        bottomLayer.SetActive(false);

        //disable waterLayer
        waterLayer.SetActive(false);

        showAllIndividualCubes = false;
    }

    public void ShowGiantMesh()
    {
        if (showGiantMesh)
        {
            return;
        }

        combinedMeshParent.SetActive(true);
        showGiantMesh = true;
    }

    public void HideGiantMesh()
    {
        if (!showGiantMesh)
        {
            return;
        }

        combinedMeshParent.SetActive(false);

        showGiantMesh = false;
    }
}