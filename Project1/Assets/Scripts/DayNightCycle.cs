using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    // Public inspector values.

    public float timeProgression;
    public int startingHour;
    public int startingMinute;
    public int startingSecond;
    public bool startAtCurrentTime;
    public ParticleSystem stars;

    // Constants.

    private const int SecondsPerMinute = 60;
    private const int MinutesPerHour = 60;
    private const int HoursPerDay = 24;
    private const int SecondsPerDay = SecondsPerMinute * MinutesPerHour * HoursPerDay;
    private const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;

    // This is a completely arbitrary value that was determined through trial and 
    // error to work well in the function where it's used to alter the size/brightness
    // of the stars.
    private const long StarSizeDivisor = 3500000000000000000;

    // Non-changing variables.

    private ParticleSystemRenderer starsRenderer;
    private float starsStartingSize;

    // Variables.

    // The in-game time in seconds. Resets to 0
    // once it hits 86400 seconds (24 hours).
    private float gameTime;

    // Use this for initialization
    void Start ()
	{
	    starsRenderer = stars.GetComponent<ParticleSystemRenderer>();
	    starsStartingSize = starsRenderer.minParticleSize;

        // Determine what time to start the simulation at.
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
        UpdateStarBrightness();
	}

    /**
     * Set the in-game clock to a particular time.
     */
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

    /**
     * Set the in-game clock to a particular time. Can be used as SetTime(DateTime.Now).
     */
    public void SetTime(DateTime time)
    {
        SetTime(time.Hour, time.Minute, time.Second);
    }

    /**
     * Update the in-game clock.
     */
    private void UpdateTime()
    {
        // Progress time.
        gameTime += Time.deltaTime * timeProgression;

        // If a full day has passed, reset the clock.
        gameTime -= gameTime > SecondsPerDay ? SecondsPerDay : 0;
    }

    /**
     * Update the rotation of the skybox by rotating *this* game object.
     * The assumption is that the sun, moon, stars, etc is a child of
     * this object, and so will rotate with it.
     */
    private void UpdateRotation()
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(0f, 360f, gameTime / SecondsPerDay), Vector3.forward);
    }

    /**
     * Updates the size of the star particles according to a quartic function. This gives it the effect
     * that the stars' brightness changes. The function has beeen crafted so that the stars have peak brightness
     * at 00:00 i.e. midnight, while they have no brightness at 12:00 i.e. mid day. Additionally, their brightness
     * changes so that they appear/disappear as expected as the day transitions.
     */
    private void UpdateStarBrightness()
    {
        starsRenderer.minParticleSize = starsStartingSize / StarSizeDivisor * Mathf.Pow(gameTime - (float) SecondsPerDay / 2, 4);
    }
}
