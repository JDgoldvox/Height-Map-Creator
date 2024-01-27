using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
//REMOVE UI ELEMENTS

public class WorldGeneratorControls : MonoBehaviour
{
    //land
    [SerializeField] private Slider landFrequencySlider;
    [SerializeField] private Slider landScaleSlider;

    [SerializeField] private TMP_Text landFrequencyText;
    [SerializeField] private TMP_Text landScaleText;

    //water
    [SerializeField] private Slider waterFrequencySlider;
    [SerializeField] private Slider waterScaleSlider;
    [SerializeField] private Slider waterHeightSlider;

    [SerializeField] private TMP_Text waterFrequencyText;
    [SerializeField] private TMP_Text waterScaleText;
    [SerializeField] private TMP_Text waterHeightText;

    //ranges
    [SerializeField] private RangeWithMinMax frequencyRange;
    [SerializeField] private RangeWithMinMax scaleRange;
    [SerializeField] private RangeWithMinMax heightRange;

    //land
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

        UpdateBlocksWhenSliderDropped(ref landFrequencyOldValue, ref landFrequencyNewValue);
        UpdateBlocksWhenSliderDropped(ref landScaleOldValue, ref landScaleNewValue);
    }

    private void MaxMinSliderValues()
    {
        landFrequencySlider.minValue = frequencyRange.min;
        landFrequencySlider.maxValue = frequencyRange.max;

        landScaleSlider.minValue = scaleRange.min;
        landScaleSlider.maxValue = scaleRange.max;

        waterFrequencySlider.minValue = frequencyRange.min;
        waterFrequencySlider.maxValue = frequencyRange.max;

        waterScaleSlider.minValue = scaleRange.min;
        waterScaleSlider.maxValue = scaleRange.max;

        waterHeightSlider.minValue = heightRange.min;
        waterHeightSlider.maxValue = heightRange.max;
    }

    public void AdjustLandFrequency()
    {
        AdjustmentStart();

        landFrequencyOldValue = S_terrainGenerator.landFrequency;
        landFrequencyNewValue = landFrequencySlider.value;

        S_terrainGenerator.landFrequency = landFrequencySlider.value;
        landFrequencyText.text = "land Frequency: " + landFrequencySlider.value;

        S_terrainGenerator.UpdateBlocks(false);
    }

    public void AdjustLandScale()
    {
        AdjustmentStart();

        landScaleOldValue = S_terrainGenerator.landScale;
        landScaleNewValue = landScaleSlider.value;

        S_terrainGenerator.landScale = landScaleSlider.value;
        landScaleText.text = "land Scale: " + landScaleSlider.value;

        S_terrainGenerator.UpdateBlocks(false);
    }

    public void AdjustWaterFrequency()
    {
        AdjustmentStart();

        waterFrequencyOldValue = S_terrainGenerator.waterFrequency;
        waterFrequencyNewValue = waterFrequencySlider.value;

        S_terrainGenerator.waterFrequency = waterFrequencySlider.value;
        waterFrequencyText.text = "land Frequency: " + waterFrequencySlider.value;

        S_terrainGenerator.UpdateBlocks(false);
    }

    public void AdjustWaterScale()
    {
        AdjustmentStart();

        waterScaleOldValue = S_terrainGenerator.waterScale;
        waterScaleNewValue = waterScaleSlider.value;

        S_terrainGenerator.waterScale = waterScaleSlider.value;
        waterScaleText.text = "land Scale: " + waterScaleSlider.value;

        S_terrainGenerator.UpdateBlocks(false);
    }

    public void AdjustWaterHeight()
    {
        AdjustmentStart();

        waterHeightOldValue = S_terrainGenerator.waterHeight;
        waterHeightNewValue = waterHeightSlider.value;

        S_terrainGenerator.waterHeight = waterHeightSlider.value;
        waterHeightText.text = "water Height: " + waterHeightSlider.value;

        S_terrainGenerator.UpdateBlocks(false);
    }

    void UpdateBlocksWhenSliderDropped(ref float oldValue, ref float newValue)
    {
        //when slider is dropped, create one big mesh
        if(newValue != oldValue && !isSliderBeingDragged)
        {
            S_terrainGenerator.UpdateBlocks(true);
            oldValue = newValue;

            Debug.Log("slider dropped!");

            AdjustmentEnd();
        }
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