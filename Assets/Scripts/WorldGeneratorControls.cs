using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
//REMOVE UI ELEMENTS

public class WorldGeneratorControls : MonoBehaviour
{
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Slider scaleSlider;

    [SerializeField] private TMP_Text frequencyText;
    [SerializeField] private TMP_Text scaleText;

    [SerializeField] private RangeWithMinMax frequencyRange;
    [SerializeField] private RangeWithMinMax scaleRange;

    private float frequencyOldValue = 0f;
    private float scaleOldValue = 0f;

    private float frequencyNewValue = 0f;
    private float scaleNewValue = 0f;

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

        //UpdateBlocksWhenSliderDropped(ref frequencyOldValue, ref frequencyNewValue);
        //UpdateBlocksWhenSliderDropped(ref scaleOldValue, ref scaleNewValue);
    }

    private void MaxMinSliderValues()
    {
        frequencySlider.minValue = frequencyRange.min;
        frequencySlider.maxValue = frequencyRange.max;

        scaleSlider.minValue = scaleRange.min;
        scaleSlider.maxValue = scaleRange.max;

        S_terrainGenerator.UpdateBlocks();
    }

    public void AdjustFrequency()
    {
        frequencyOldValue = S_terrainGenerator.frequency;
        frequencyNewValue = frequencySlider.value;

        S_terrainGenerator.frequency = frequencySlider.value;
        frequencyText.text = "Frequency: " + frequencySlider.value;

        S_terrainGenerator.UpdateBlocks();
    }

    public void AdjustScale()
    {
        scaleOldValue = S_terrainGenerator.scale;
        scaleNewValue = scaleSlider.value;

        S_terrainGenerator.scale = scaleSlider.value;
        scaleText.text = "Scale: " + scaleSlider.value;

        S_terrainGenerator.UpdateBlocks();
    }

    void UpdateBlocksWhenSliderDropped(ref float oldValue, ref float newValue)
    {
        //when slider is dropped, create one big mesh
        if(newValue != oldValue && !isSliderBeingDragged)
        {
            S_terrainGenerator.UpdateBlocks();
            oldValue = newValue;

            Debug.Log("slider dropped!");
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
}