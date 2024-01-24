using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldGeneratorControls : MonoBehaviour
{
    [SerializeField] private Slider frequencySlider;
    [SerializeField] private Slider scaleSlider;

    [SerializeField] private TMP_Text frequencyText;
    [SerializeField] private TMP_Text scaleText;

    [SerializeField] private RangeWithMinMax frequencyRange;
    [SerializeField] private RangeWithMinMax scaleRange;

    TerrainGenerator terrainGenerator;

    private void Awake()
    {
        terrainGenerator = GetComponent<TerrainGenerator>();
    }

    private void Start()
    {
        MaxMinSliderValues();
    }

    private void MaxMinSliderValues()
    {
        frequencySlider.minValue = frequencyRange.min;
        frequencySlider.maxValue = frequencyRange.max;

        scaleSlider.minValue = scaleRange.min;
        scaleSlider.maxValue = scaleRange.max;
    }

    public void AdjustFrequency()
    {
        terrainGenerator.frequency = frequencySlider.value;
        frequencyText.text = "Frequency: " + frequencySlider.value;
    }

    public void AdjustScale()
    {
        terrainGenerator.scale = scaleSlider.value;
        frequencyText.text = "Scale: " + scaleSlider.value;
    }
}
