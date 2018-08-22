using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour {

    // Public inspector values.

    public float timeProgression;
    public int startingHour;
    public int startingMinute;
    public int startingSecond;
    public bool startAtCurrentTime;

    // Constants.

    [HideInInspector]
    public const int SecondsPerMinute = 60;
    public const int MinutesPerHour = 60;
    public const int HoursPerDay = 24;
    public const int SecondsPerDay = SecondsPerMinute * MinutesPerHour * HoursPerDay;
    public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;

    // Variables.

    // The in-game time in seconds. Resets to 0
    // once it hits 86400 seconds (24 hours).
    private float currentGameTime = 0f;

    // Use this for initialization
    void Start () {
        // Determine what time to start the simulation at.
        if (startAtCurrentTime) {
            SetTime(DateTime.Now);
        } else {
            SetTime(startingHour, startingMinute, startingSecond);
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
	    UpdateTime();
	}

    /**
     * Set the in-game clock to a particular time.
     */
    public void SetTime(int hour, int minute, int second) {
        // Ensure the given time is valid.
        if (hour >= 24 || minute >= 60 || second >= 60
            || hour < 0 || minute < 0 || second < 0) {
            Debug.LogError(string.Format("Invalid time set! Cannot set to {0}:{1}:{2}.", hour, minute, second));
            return;
        }

        currentGameTime = hour * SecondsPerHour + minute * SecondsPerMinute + second;
    }

    /**
     * Set the in-game clock to a particular time. Can be used as SetTime(DateTime.Now).
     */
    public void SetTime(DateTime time) {
        SetTime(time.Hour, time.Minute, time.Second);
    }

    public DateTime GetTime()
    {
//        int hour = (int) currentGameTime / SecondsPerHour;
//        int minute = (int) (currentGameTime - hour * SecondsPerHour) / SecondsPerMinute;
//        int second = (int) (currentGameTime - hour * SecondsPerHour - minute * SecondsPerMinute);
//        return new DateTime(0, 0, 0, hour, minute, second);

        DateTime time = DateTime.MinValue;
        return time.AddSeconds(currentGameTime);
    }

    public float GetTimeAsSeconds()
    {
        return currentGameTime;
    }

    /**
     * Update the in-game clock.
     */
    private void UpdateTime() {
        // Progress time.
        currentGameTime += Time.deltaTime * timeProgression;

        // If a full day has passed, reset the clock.
        currentGameTime -= currentGameTime > SecondsPerDay ? SecondsPerDay : 0;
    }
}
