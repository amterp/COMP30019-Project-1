
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavePropertiesSlider : MonoBehaviour
{
    public WaterTimeUpdate water;
    public Slider amplitudeSlider;
    public Slider frequencySlider;
    public Slider wavelengthSlider;
    public Text amplitudeText;
    public Text frequencyText;
    public Text wavelengthText;

    // Use this for initialization
    void Start()
    {
        // Subscribe to slider changes.
        amplitudeSlider.onValueChanged.AddListener(OnAmplitudeChange);
        frequencySlider.onValueChanged.AddListener(OnFrequencyChange);
        wavelengthSlider.onValueChanged.AddListener(OnWavelengthChange);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * This will be called every time the slider's value changes.
     * It handles the change.
     */
    private void OnAmplitudeChange(float newValue)
    {
        // Set our water height color. If it goes higher than the mountain height, then update that too.
        water.amplitude = newValue;
        UpdateHeightText(amplitudeText, newValue);
    }
    private void OnFrequencyChange(float newValue)
    {
        // Set our mountain height color. If it goes higher than the snow height, then update that too.
        water.frequency = newValue;
        UpdateHeightText(frequencyText, newValue);
    }
    private void OnWavelengthChange(float newValue)
    {
        water.wavelength = newValue;
        UpdateHeightText(wavelengthText, newValue);
    }

    /**
     * Update the text that displays the current heights.
     */
    private void UpdateHeightText(Text updateText, float value)
    {
        updateText.text = string.Format("{0}", System.Math.Round(value, 2));
    }
}

