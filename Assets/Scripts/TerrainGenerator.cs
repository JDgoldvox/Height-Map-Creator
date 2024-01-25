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

    private List<MeshFilter> grassCubeMeshes = new List<MeshFilter>();

    private GameObject combinedGrassCubes;

    Mesh combinedGrassMesh;

    private void Awake()
    {
        S_worldGeneratorControls = GetComponent<WorldGeneratorControls>();
    }

    // Start is called before the first frame update
    void Start()
    {
        combinedGrassCubes = new GameObject("combined grass cubes");
        combinedGrassMesh = new Mesh();

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

    public void UpdateBlocks(bool combine)
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

            if (combine)
            {
                CombineMesh(grassCubes, combinedGrassCubes, grassCubeMeshes);
            }
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
        combinedTransform.position = originalCubes[0].transform.position;
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

    //ShowAllIndividualCubes()
    //{

    //}
}