using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Drawing;
//REMOVE UI ELEMENTS

public class WorldGeneratorControls : MonoBehaviour
{
    //size
    [SerializeField] private TMPro.TMP_InputField xInput;
    [SerializeField] private TMPro.TMP_InputField zInput;

    //land
    [SerializeField] private Slider landFrequencySlider;
    [SerializeField] private Slider landScaleSlider;
    [SerializeField] private Slider landOctavesSlider;

    [SerializeField] private TMP_Text landFrequencyText;
    [SerializeField] private TMP_Text landScaleText;
    [SerializeField] private TMP_Text landOctavesText;

    //water
    [SerializeField] private Slider waterFrequencySlider;
    [SerializeField] private Slider waterScaleSlider;
    [SerializeField] private Slider waterHeightSlider;

    [SerializeField] private TMP_Text waterFrequencyText;
    [SerializeField] private TMP_Text waterScaleText;
    [SerializeField] private TMP_Text waterHeightText;

    //ranges
    [SerializeField] private RangeWithMinMax octaveRange;
    [SerializeField] private RangeWithMinMax frequencyRange;
    [SerializeField] private RangeWithMinMax scaleRange;
    [SerializeField] private RangeWithMinMax heightRange;

    //land
    private float landOctavesOldValue = 0;
    private float landOctavesNewValue = 0;

    private float landFrequencyOldValue = 0f;
    private float landFrequencyNewValue = 0f;

    private float landScaleOldValue = 0f;
    private float landScaleNewValue = 0f;

    //water
    private float waterScaleOldValue = 0f;
    private float waterScaleNewValue = 0f;

    private float waterFrequencyOldValue = 0f;
    private float waterFrequencyNewValue = 0f;

    private float waterHeightNewValue = 0f;
    private float waterHeightOldValue = 0f;

    TerrainGenerator S_terrainGenerator;

    private bool isSliderBeingDragged = false;
    private bool updateOnlyWater = false;

    private int xSize = 0;
    private int zSize = 0;

    private void Awake()
    {
        S_terrainGenerator = GetComponent<TerrainGenerator>();
    }

    private void Start()
    {
        MaxMinSliderValues();
    }

    private void Update()
    {
        IsMouseDragging();

        //land
        UpdateBlocksWhenSliderDropped(ref landOctavesOldValue, ref landOctavesNewValue, ref updateOnlyWater);
        UpdateBlocksWhenSliderDropped(ref landFrequencyOldValue, ref landFrequencyNewValue, ref updateOnlyWater);
        UpdateBlocksWhenSliderDropped(ref landScaleOldValue, ref landScaleNewValue, ref updateOnlyWater);
        
        //water
        UpdateBlocksWhenSliderDropped(ref waterFrequencyOldValue, ref waterFrequencyNewValue, ref updateOnlyWater);
        UpdateBlocksWhenSliderDropped(ref waterScaleOldValue, ref waterScaleNewValue, ref updateOnlyWater);
        UpdateBlocksWhenSliderDropped(ref waterHeightOldValue, ref waterHeightNewValue, ref updateOnlyWater);
    }

    private void MaxMinSliderValues()
    {
        //land
        landOctavesSlider.minValue = octaveRange.min;
        landOctavesSlider.maxValue = octaveRange.max;

        landFrequencySlider.minValue = frequencyRange.min;
        landFrequencySlider.maxValue = frequencyRange.max;

        landScaleSlider.minValue = scaleRange.min;
        landScaleSlider.maxValue = scaleRange.max;

        //water
        waterFrequencySlider.minValue = frequencyRange.min;
        waterFrequencySlider.maxValue = frequencyRange.max;

        waterScaleSlider.minValue = scaleRange.min;
        waterScaleSlider.maxValue = scaleRange.max;

        waterHeightSlider.minValue = heightRange.min;
        waterHeightSlider.maxValue = heightRange.max;
    }

    public void AdjustLandOctaves()
    {
        AdjustmentStart();

        landOctavesOldValue = S_terrainGenerator.landOctaves;
        landOctavesNewValue = landOctavesSlider.value;

        S_terrainGenerator.landOctaves = landOctavesSlider.value;
        landOctavesText.text = "Land Octaves: " + landOctavesSlider.value;

        S_terrainGenerator.UpdateBlocks(false, false);
    }

    public void AdjustLandFrequency()
    {
        AdjustmentStart();

        landFrequencyOldValue = S_terrainGenerator.landFrequency;
        landFrequencyNewValue = landFrequencySlider.value;

        S_terrainGenerator.landFrequency = landFrequencySlider.value;
        landFrequencyText.text = "Land Frequency: " + landFrequencySlider.value;

        S_terrainGenerator.UpdateBlocks(false, false);
    }

    public void AdjustLandScale()
    {
        AdjustmentStart();

        landScaleOldValue = S_terrainGenerator.landScale;
        landScaleNewValue = landScaleSlider.value;

        S_terrainGenerator.landScale = landScaleSlider.value;
        landScaleText.text = "Land Scale: " + landScaleSlider.value;

        S_terrainGenerator.UpdateBlocks(false, false);
    }

    public void AdjustWaterFrequency()
    {
        AdjustmentStart();
        updateOnlyWater = true;

        waterFrequencyOldValue = S_terrainGenerator.waterFrequency;
        waterFrequencyNewValue = waterFrequencySlider.value;

        S_terrainGenerator.waterFrequency = waterFrequencySlider.value;
        waterFrequencyText.text = "Water Frequency: " + waterFrequencySlider.value;

        S_terrainGenerator.UpdateBlocks(false, true);
    }

    public void AdjustWaterScale()
    {
        AdjustmentStart();
        updateOnlyWater = true;

        waterScaleOldValue = S_terrainGenerator.waterScale;
        waterScaleNewValue = waterScaleSlider.value;

        S_terrainGenerator.waterScale = waterScaleSlider.value;
        waterScaleText.text = "Water Scale: " + waterScaleSlider.value;

        S_terrainGenerator.UpdateBlocks(false, true);
    }

    public void AdjustWaterHeight()
    {
        AdjustmentStart();
        updateOnlyWater = true;

        waterHeightOldValue = S_terrainGenerator.waterHeight;
        waterHeightNewValue = waterHeightSlider.value;

        S_terrainGenerator.waterHeight = waterHeightSlider.value;
        waterHeightText.text = "Water Height: " + waterHeightSlider.value;

        S_terrainGenerator.UpdateBlocks(false, true);
    }

    void UpdateBlocksWhenSliderDropped(ref float oldValue, ref float newValue, ref bool updateOnlyWater)
    {
        //when slider is dropped, create one big mesh
        if(newValue != oldValue && !isSliderBeingDragged)
        {
            if (updateOnlyWater)
            {
                S_terrainGenerator.UpdateBlocks(true, true);
                updateOnlyWater = false;
            }
            else
            {
                S_terrainGenerator.UpdateBlocks(true, false);
            }
            
            oldValue = newValue;

            AdjustmentEnd();
        }
    }

    public void EnableWaves()
    {
        //turn it to opposite
        S_terrainGenerator.enableWaves = !S_terrainGenerator.enableWaves;
    }

    public void ChangeX()
    {
        if (int.TryParse(xInput.text, out xSize))
        {

        }
        else
        {
            // Parsing failed, handle the error accordingly (e.g., show an error message)
            Debug.LogError("Failed to parse input string to integer.");
        }
    }

    public void ChangeZ()
    {
        if (int.TryParse(zInput.text, out zSize))
        {

        }
        else
        {
            // Parsing failed, handle the error accordingly (e.g., show an error message)
            Debug.LogError("Failed to parse input string to integer.");
        }
    }

    public void ConfirmSizeChange()
    {
        S_terrainGenerator.ChangeWorldSize(xSize, zSize);
    }

    private void IsMouseDragging()
    {
        if (Mouse.current.leftButton.IsPressed())
        {
            isSliderBeingDragged = true;
        }
        else
        {
            isSliderBeingDragged = false;
        }
    }

    void AdjustmentStart()
    {
        S_terrainGenerator.HideGiantMesh();
        S_terrainGenerator.ShowAllIndividualCubes();
    }

    void AdjustmentEnd()
    {
        S_terrainGenerator.HideAllIndividualCubes();
        S_terrainGenerator.ShowGiantMesh();
    }
}