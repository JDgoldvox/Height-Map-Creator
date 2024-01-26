using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class TerrainGenerator : MonoBehaviour
{
    int height = 3;

    [HideInInspector] public float frequency = 1.0f;
    [HideInInspector] public float scale = 1.0f;

    private WorldGeneratorControls S_worldGeneratorControls;

    [SerializeField] private WorldSize worldSize = new WorldSize();

    [SerializeField] private GameObject grassCube;
    [SerializeField] private GameObject dirtCube;
    [SerializeField] private GameObject shallowWaterCube;
    [SerializeField] private GameObject deepWaterCube;

    private GameObject combinedGrassCubes;
    private List<MeshFilter> grassCubeMeshes = new List<MeshFilter>();

    private GameObject combinedUnderGroundCubes;
    private List<MeshFilter> underGroundCubeMeshes = new List<MeshFilter>();

    private bool showAllIndividualCubes = false;
    private bool showGiantMesh = true;

    Dictionary<GameObject, UnderGroundCore> world = new Dictionary<GameObject, UnderGroundCore>();

    private void Awake()
    {
        S_worldGeneratorControls = GetComponent<WorldGeneratorControls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        combinedGrassCubes = new GameObject("combined grass cubes");
        combinedUnderGroundCubes = new GameObject("combined under ground cubes");

        CreateHeightMap();
    }

    // Update is called once per frame
    void Update()
    {

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

        //Top grass layer
        //loop through all blocks on map and create all top layer cubes
        for (int x = 0; x < worldSize.x; x++)
        {
            for (int z = 0; z < worldSize.z; z++)
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

        List<GameObject> underGroundCubes = new List<GameObject>();
        List<UnderGroundCore> cores = new List<UnderGroundCore>();

        foreach (GameObject obj in grassCubes)
        {
            //place dirt blocks below for some height below one another
            List<GameObject> tempUnderGroundCubeList = new List<GameObject>();

            for (int y = 0; y < height; y++)
            {
                //create underground Cube
                var objPos = obj.transform.position;
                GameObject newCubeObj = Instantiate(dirtCube, new Vector3(objPos.x, objPos.y - (float)y, objPos.z), Quaternion.identity);

                //add to cube array so it we can collect them
                tempUnderGroundCubeList.Add(newCubeObj);
            }

            //create new core and add the list in
            UnderGroundCore newCore = new UnderGroundCore(tempUnderGroundCubeList);

            //add the core to list
            cores.Add(newCore);
        }

        //map top layer to core cubes in world dic
        for(int i = 0; i < grassCubes.Count; i++)
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
            Vector3 topCubePos = topCube.transform.position;

            float adjustedX = topCubePos.x / worldSize.x;
            float adjustedZ = topCubePos.z / worldSize.z;
            adjustedX *= frequency;
            adjustedZ *= frequency;

            noiseNumber = NoiseFunction.GenerateNoise(adjustedX, adjustedZ);
            topCube.transform.position = new Vector3(topCubePos.x, noiseNumber * scale, topCubePos.z);

            //combine top layer
            if (combine)
            {
                List<GameObject> allTopCubes = new List<GameObject>();
                //get the list of all top cubes
                foreach(GameObject cube in world.Keys)
                {
                    allTopCubes.Add(cube);
                }

                CombineMesh(allTopCubes, combinedGrassCubes, grassCubeMeshes);
            }

            //Find corrosponding core
            List<GameObject> underGroundCubeList = world[topCube].cubes;

            for(int i = 0; i < underGroundCubeList.Count; i++)
            {
                underGroundCubeList[i].transform.position = new Vector3(topCube.transform.position.x,
                    topCube.transform.position.y - i,
                    topCube.transform.position.z);
            }

            //combine cores

            if (combine)
            {
                //get the list of all cores cubes
                List<GameObject> allUnderGroundCubes = new List<GameObject>();

                foreach (UnderGroundCore core in world.Values)
                {
                    CombineMesh(core.cubes, combinedUnderGroundCubes, underGroundCubeMeshes);
                }
            }

            ////get the list of all cores cubes
            //List<GameObject> allUnderGroundCubes = new List<GameObject>();

            //foreach (UnderGroundCore core in world.Values)
            //{
            //    foreach (GameObject cube in core.cubes)
            //    {
            //        CombineMesh(allUnderGroundCubes, combinedUnderGroundCubes, underGroundCubeMeshes);
            //    }
            //}
        }
    }
    void CombineMesh(List<GameObject> originalCubes, GameObject combinedMeshGameObject, List<MeshFilter> meshFilters)
    {
        meshFilters.Clear();

        // Make an array of combine instances
        var combinedInstance = new CombineInstance[originalCubes.Count];

        // Store material and mesh renderer references outside the loop
        Material sharedMaterial = originalCubes[0].GetComponent<MeshRenderer>().sharedMaterial;
        MeshRenderer meshRenderer = combinedMeshGameObject.GetComponent<MeshRenderer>();

        // Set the meshes and transform in the combined instance
        for (int i = 0; i < originalCubes.Count; i++)
        {
            MeshFilter meshFilter = originalCubes[i].GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                combinedInstance[i].mesh = meshFilter.sharedMesh;
                combinedInstance[i].transform = originalCubes[i].transform.localToWorldMatrix;
                meshFilters.Add(meshFilter);
            }
        }

        // Combine instances into mesh
        Mesh combinedGrassMesh = new Mesh();
        combinedGrassMesh.CombineMeshes(combinedInstance);

        // New game object
        Transform combinedTransform = combinedMeshGameObject.transform;
        combinedTransform.position = new Vector3(originalCubes[0].transform.position.x, originalCubes[0].transform.position.x, originalCubes[0].transform.position.z);
        combinedTransform.rotation = originalCubes[0].transform.rotation;

        // Add or update MeshFilter
        MeshFilter combinedMeshFilter = combinedMeshGameObject.GetComponent<MeshFilter>();
        if (combinedMeshFilter == null)
        {
            combinedMeshFilter = combinedMeshGameObject.AddComponent<MeshFilter>();
        }
        combinedMeshFilter.mesh = combinedGrassMesh;

        // Add or update MeshRenderer
        if (meshRenderer == null)
        {
            meshRenderer = combinedMeshGameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.material = sharedMaterial;
    }

    public void ShowAllIndividualCubes()
    {
        //if(showAllIndividualCubes) {
        //    return;
        //}

        //foreach(var obj in grassCubes)
        //{
        //    obj.SetActive(true); 
        //}

        //foreach (var obj in underGroundCubes)
        //{
        //    obj.SetActive(true);
        //}

        //showAllIndividualCubes = true;
    }

    public void HideAllIndividualCubes()
    {
        //if (!showAllIndividualCubes)
        //{
        //    return;
        //}

        //foreach (var obj in grassCubes)
        //{
        //    obj.SetActive(false);
        //}

        //foreach (var obj in underGroundCubes)
        //{
        //    obj.SetActive(false);
        //}

        //showAllIndividualCubes = false;
    }

    public void ShowGiantMesh()
    {
        //if (showGiantMesh)
        //{
        //    return;
        //}

        //combinedGrassCubes.SetActive(true);
        //combinedUnderGroundCubes.SetActive(true);
        //showGiantMesh = true;
    }

    public void HideGiantMesh()
    {
        //if (!showGiantMesh)
        //{
        //    return;
        //}

        //combinedGrassCubes.SetActive(false);
        //combinedUnderGroundCubes.SetActive(false);

        //showGiantMesh = false;
    }
 }