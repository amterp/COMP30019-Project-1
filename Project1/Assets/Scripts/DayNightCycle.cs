using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float timeProgression;
    public int startingHour;
    public int startingMinute;
    public int startingSecond;
    public bool startAtCurrentTime;

    private const int SecondsPerMinute = 60;
    private const int MinutesPerHour = 60;
    private const int HoursPerDay = 24;
    private const int SecondsPerDay = SecondsPerMinute * MinutesPerHour * HoursPerDay;
    private const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;

    private float gameTime;

	// Use this for initialization
	void Start ()
	{
	    if (startAtCurrentTime)
	    {
            SetTime(DateTime.Now);
	    }
	    else
	    {
            SetTime(startingHour, startingMinute, startingSecond);
	    }
	}
	
	// Update is called once per frame

    void Update ()
	{
	    UpdateTime();
        Debug.Log("Gametime: " + gameTime);
	    UpdateRotation();
	}

    public void SetTime(int hour, int minute, int second)
    {
        // Ensure the given time is valid.
        if (hour >= 24 || minute >= 60 || second >= 60 
            || hour < 0 || minute < 0 || second < 0)
        {
            Debug.LogError(string.Format("Invalid time set! Cannot set to {0}:{1}:{2}.", hour, minute, second));
            return;
        }

        gameTime = hour * SecondsPerHour + minute * SecondsPerMinute + second;
    }

    public void SetTime(DateTime time)
    {
        SetTime(time.Hour, time.Minute, time.Second);
    }

    private void UpdateTime()
    {
        // Progress time.
        gameTime += Time.deltaTime * timeProgression;

        // If a full day has passed, reset the clock.
        gameTime -= gameTime > SecondsPerDay ? SecondsPerDay : 0;
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(0f, 360f, gameTime / SecondsPerDay), Vector3.forward);
    }
}
