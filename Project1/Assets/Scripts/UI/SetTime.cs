using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTime : MonoBehaviour
{
    public InputField hourInput;
    public InputField minuteInput;
    public InputField secondInput;
    public Button setTimeButton;
    public GameTime gameTime;

	// Use this for initialization
	void Start () {
        // Subscribe to the button click event.
		setTimeButton.onClick.AddListener(OnSetTimeClick);
	}

    /**
     * Called then the "set time" button is clicked. This will read input
     * from the input fields and update the time o
     */
    private void OnSetTimeClick()
    {
        int hour = ReadInput(hourInput);
        int minute = ReadInput(minuteInput);
        int second = ReadInput(secondInput);

        // Ensure that the input is valid.
        if (hour >= 24 || hour < 0
            || minute >= 60 || minute < 0
            || second >= 60 || second < 0)
        {
            Debug.LogWarningFormat("Invalid time: '{0}:{1}:{2}'", hour, minute, second);
            return;
        }

        // Convert the input into a DateTime object.
        DateTime time = DateTime.MinValue;
        time = time.AddHours(hour);
        time = time.AddMinutes(minute);
        time = time.AddSeconds(second);

        // Update the time.
        gameTime.SetTime(time);
    }

    /**
     * Reads a value from a field. If it results in a FormatException,
     * defaults to returning 0.
     */
    private int ReadInput(InputField field)
    {
        try
        {
            int value = int.Parse(field.text);
            return value;
        }
        catch (FormatException)
        {
            // Means it's probably blank. Interpret as 0.
            return 0;
        }
    }
}
