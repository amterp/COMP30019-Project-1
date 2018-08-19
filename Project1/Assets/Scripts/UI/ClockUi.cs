using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockUi : MonoBehaviour
{
    public GameTime gameTime;
    public Text clockText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    UpdateClockText();
	}

    private void UpdateClockText()
    {
        DateTime now = gameTime.GetTime();

        clockText.text = string.Format("{0:00}:{1:00}:{2:00}", now.Hour, now.Minute, now.Second);
    }
}
