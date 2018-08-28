
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainColorSlider : MonoBehaviour
{
    public TerrainColor terrainColor;
    public Slider sandSlider;
    public Slider mountainSlider;
    public Slider snowSlider;
    public Text sandHeightText;
    public Text mountainHeightText;
    public Text snowHeightText;

    // Use this for initialization
    void Start()
    {
        // Subscribe to slider changes.
        sandSlider.onValueChanged.AddListener(OnSandSliderChange);
        mountainSlider.onValueChanged.AddListener(OnMountainSliderChange);
        snowSlider.onValueChanged.AddListener(OnSnowSliderChange);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * This will be called every time the slider's value changes.
     * It handles the change.
     */
    private void OnSandSliderChange(float newValue)
    {
        // Set our water height color. If it goes higher than the mountain height, then update that too.
        terrainColor.waterHeight = newValue;
        UpdateHeightText(sandHeightText, newValue);
        if (terrainColor.waterHeight > terrainColor.mountainHeight)
        {
            OnMountainSliderChange(newValue);
            mountainSlider.value = newValue;
        }
    }
    private void OnMountainSliderChange(float newValue)
    {
        // Set our mountain height color. If it goes higher than the snow height, then update that too.
        terrainColor.mountainHeight = newValue;
        UpdateHeightText(mountainHeightText, newValue);
        if (terrainColor.mountainHeight > terrainColor.snowHeight)
        {
            OnSnowSliderChange(newValue);
            snowSlider.value = newValue;
        }
        else if (terrainColor.mountainHeight < terrainColor.waterHeight)
        {
            OnSandSliderChange(newValue);
            sandSlider.value = newValue;
        }
    }
    private void OnSnowSliderChange(float newValue)
    {
        terrainColor.snowHeight = newValue;
        UpdateHeightText(snowHeightText, newValue);
        if (terrainColor.snowHeight < terrainColor.mountainHeight)
        {
            OnMountainSliderChange(newValue);
            mountainSlider.value = newValue;
        }
    }

    /**
     * Update the text that displays the current heights.
     */
    private void UpdateHeightText(Text updateText, float value)
    {
        updateText.text = string.Format("{0}", System.Math.Round(value, 2));
    }
}

