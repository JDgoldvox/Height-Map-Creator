using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    int height = 10;

    [HideInInspector] public float frequency = 1.0f;
    [HideInInspector] public float scale = 1.0f;

    private WorldGeneratorControls S_worldGeneratorControls;

    [SerializeField] private WorldSize worldSize = new WorldSize();

    [SerializeField] private GameObject grassCube;
    [SerializeField] private GameObject dirtCube;
    [SerializeField] private GameObject shallowWaterCube;
    [SerializeField] private GameObject deepWaterCube;

    private bool showAllIndividualCubes = false;
    private bool showGiantMesh = true;

    Dictionary<GameObject, UnderGroundCore> world = new Dictionary<GameObject, UnderGroundCore>();

    private GameObject combinedMeshParent;

    private void Awake()
    {
        S_worldGeneratorControls = GetComponent<WorldGeneratorControls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        combinedMeshParent = new GameObject("combined meshes");

        CreateHeightMap();
    }

    void CreateHeightMap()
    {
        //Instantiate cubes
        InstantiateCubes();
    }

    void InstantiateCubes()
    {
        float noiseNumber = 0;
        List<GameObject> grassCubes = new List<GameObject>();
        GameObject topLayer = new GameObject("top layer");

        //Top grass layer
        //loop through all blocks on map and create all top layer cubes
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int z = 0; z < worldSize.z; z++)
            {
                float adjustedX = (x / worldSize.x) * frequency;
                float adjustedZ = (z / worldSize.z) * frequency;

                noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ);
                GameObject newCubeObj = Instantiate(grassCube, new Vector3(x, noiseNumber * scale, z), Quaternion.identity);
                newCubeObj.transform.parent = topLayer.transform;
                grassCubes.Add(newCubeObj);
            }
        }

        //underground layer
        //loop through all blocks on map
        List<GameObject> underGroundCubes = new List<GameObject>();
        List<UnderGroundCore> cores = new List<UnderGroundCore>();
        GameObject bottomLayer = new GameObject("bottom layer");

        foreach (GameObject obj in grassCubes)
        {
            //place dirt blocks below for some height below one another
            List<GameObject> tempUnderGroundCubeList = new List<GameObject>();

            for (int y = 0; y < height; y++)
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
    }

    public void UpdateBlocks(bool combine)
    {
        float noiseNumber = 0;
        
        //loop through all blocks on world
        foreach (var topCube in world.Keys)
        {
            //update all surface cube
            Vector3 topCubePos = topCube.transform.position;

            float adjustedX = (topCubePos.x / worldSize.x) * frequency;
            float adjustedZ = (topCubePos.z / worldSize.z) * frequency;

            noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ);
            topCube.transform.position = new Vector3(topCubePos.x, noiseNumber * scale, topCubePos.z);

            //Find corrosponding core cubes
            List<GameObject> underGroundCubeList = world[topCube].cubes;

            for (int i = 0; i < underGroundCubeList.Count; i++)
            {
                underGroundCubeList[i].transform.position = new Vector3(topCube.transform.position.x,
                    topCube.transform.position.y - i - 1,
                    topCube.transform.position.z);
            }
        }

        //combine top layer
        if (combine)
        {
            List<GameObject> allTopCubes = new List<GameObject>();
            //get the list of all top cubes
            foreach (GameObject cube in world.Keys)
            {
                allTopCubes.Add(cube);
            }

            CombineMesh(allTopCubes);

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

            CombineMesh(allUnderGroundCubes);
        }
    }
    void CombineMesh(List<GameObject> originalCubes)
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
        List<GameObject> megaMeshGameObject = new List<GameObject>();

        foreach (List<GameObject> list in gameObjectLists)
        {
            GameObject combinedMeshGameObject = new GameObject("combinedMesh");
            megaMeshGameObject.Add(combinedMeshGameObject);
        }
        //////////////////////////////////////////////////////////////////////////

        //combine each list of game objects to make a mega mesh
        for (int i = 0; i < gameObjectLists.Count; i++)
        {
            List<GameObject> listOfCubes = gameObjectLists[i];
            GameObject firstCube = listOfCubes[0];

            //set parent
            //megaMeshGameObject[i].transform.parent = combinedMeshParent.transform;

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
        foreach (var obj in world.Keys)
        {
            obj.SetActive(true);
        }

        //enable bottom
        foreach (var core in world.Values)
        {
            foreach(GameObject c in core.cubes)
            c.SetActive(true);
        }

        showAllIndividualCubes = true;
    }

    public void HideAllIndividualCubes()
    {
        if (!showAllIndividualCubes)
        {
            return;
        }

        //disable top
        foreach (var obj in world.Keys)
        {
            obj.SetActive(false);
        }

        //disable bottom
        foreach (var core in world.Values)
        {
            foreach (GameObject c in core.cubes)
                c.SetActive(false);
        }

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

        //showGiantMesh = false;
    }
}