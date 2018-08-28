using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeedSlider : MonoBehaviour
{
    public GameTime gameTime;
    public Slider slider;
    public Text timeSpeedText;

    // Use this for initialization
    void Start()
    {
        // Subscribe to slider changes.
        slider.onValueChanged.AddListener(OnSliderChange);

        // Initialize the text to the current value.
        UpdateTimeSpeedText((int)gameTime.timeProgression);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * This will be called every time the slider's value changes.
     * It handles the change.
     */
    private void OnSliderChange(float newValue)
    {
        // Piecewise function.
        int newSpeed = newValue <= 2 ? (int)newValue : (int)Mathf.Pow(2, newValue);
        gameTime.timeProgression = newSpeed;
        UpdateTimeSpeedText(newSpeed);
    }

    /**
     * Update the text that displays the current time speed.
     */
    private void UpdateTimeSpeedText(int speed)
    {
        timeSpeedText.text = string.Format("{0}x", speed);
    }
}

