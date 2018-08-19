using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    // Public inspector values.

    public GameTime gameTime;
    public ParticleSystem stars;

    // This is a completely arbitrary value that was determined through trial and 
    // error to work well in the function where it's used to alter the size/brightness
    // of the stars.
    private const long StarSizeDivisor = 3500000000000000000;

    // Private variables.

    private ParticleSystemRenderer starsRenderer;
    private float starsStartingSize;

    // Use this for initialization
    void Start ()
	{
	    starsRenderer = stars.GetComponent<ParticleSystemRenderer>();
	    starsStartingSize = starsRenderer.minParticleSize;
	}
	
	// Update is called once per frame
    void Update()
    {
        UpdateRotation();
        UpdateStarBrightness();
    }

    /**
     * Update the rotation of the skybox by rotating *this* game object.
     * The assumption is that the sun, moon, stars, etc is a child of
     * this object, and so will rotate with it.
     */
    private void UpdateRotation()
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(0f, 360f, gameTime.GetTimeAsSeconds() / GameTime.SecondsPerDay), Vector3.forward);
    }

    /**
     * Updates the size of the star particles according to a quartic function. This gives it the effect
     * that the stars' brightness changes. The function has beeen crafted so that the stars have peak brightness
     * at 00:00 i.e. midnight, while they have no brightness at 12:00 i.e. mid day. Additionally, their brightness
     * changes so that they appear/disappear as expected as the day transitions.
     */
    private void UpdateStarBrightness()
    {
        starsRenderer.minParticleSize = 
            starsStartingSize / StarSizeDivisor * Mathf.Pow(gameTime.GetTimeAsSeconds() - (float)GameTime.SecondsPerDay / 2, 4);
    }
}
