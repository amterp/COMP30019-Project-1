
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
    public Slider tideAmplitudeSlider;
    public Slider tideFrequencySlider;

    public Text amplitudeText;
    public Text frequencyText;
    public Text wavelengthText;
    public Text tideAmplitudeText;
    public Text tideFrequencyText;

    // Use this for initialization
    void Start()
    {
        // Subscribe to slider changes.
        amplitudeSlider.onValueChanged.AddListener(OnAmplitudeChange);
        frequencySlider.onValueChanged.AddListener(OnFrequencyChange);
        wavelengthSlider.onValueChanged.AddListener(OnWavelengthChange);
        tideAmplitudeSlider.onValueChanged.AddListener(OnTideAmplitudeChange);
        tideFrequencySlider.onValueChanged.AddListener(OnTideFrequencyChange);
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
        water.amplitude = newValue;
        UpdateValueText(amplitudeText, newValue);
    }
    private void OnFrequencyChange(float newValue)
    {
        water.frequency = newValue;
        UpdateValueText(frequencyText, newValue);
    }
    private void OnWavelengthChange(float newValue)
    {
        water.wavelength = newValue;
        UpdateValueText(wavelengthText, newValue);
    }

    private void OnTideFrequencyChange(float newValue)
    {
        water.tideFrequency = newValue;
        UpdateValueText(tideFrequencyText, newValue);
    }

    private void OnTideAmplitudeChange(float newValue)
    {
        water.tideAmplitude = newValue;
        UpdateValueText(tideAmplitudeText, newValue);
    }

    /**
     * Update the text that displays the current values.
     */
    private void UpdateValueText(Text updateText, float value)
    {
        updateText.text = string.Format("{0}", System.Math.Round(value, 2));
    }
}

