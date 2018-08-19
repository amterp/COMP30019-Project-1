using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldOfViewSlider : MonoBehaviour
{
    // The camera which will have its fov changed.
    public Camera cam;

    // The slider which will determine/change the fov.
    public Slider slider;

    // The text that displays the current fov.
    public Text fovCounterText;

    // Use this for initialization
	void Start ()
	{
        // Subscribe to slider changes.
	    slider.onValueChanged.AddListener(OnSliderChange);

        // Initiate the fov counter box to the current value.
	    UpdateFovCounterText(slider.value);
    }

    /**
     * This will be called every time the slider's value changes.
     * It handles the change.
     */
    private void OnSliderChange(float newValue)
    {
        cam.fieldOfView = newValue;
        UpdateFovCounterText(newValue);
    }

    /**
     * Update the text that displays the current fov.
     */
    private void UpdateFovCounterText(float fov)
    {
        fovCounterText.text = string.Format("{0:0}", fov);
    }
}
