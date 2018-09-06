using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int targetFrameRate;
    public bool enableVSync;

    // Public fields that will not appear in the inspector.

    // An event that will be triggered each time the UI is toggled.
    // Other classes may subscribe to it.
    public static event Action<bool> uiToggleEvent;

    // Public variabled to not appear in the inspector.
    [HideInInspector]
    public static bool uiEnabled = false;

    void Awake()
    {
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;
        Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update ()
	{
	    CheckUiToggle();
	}

    /**
     * Called every frame, this method will check if the button for toggling
     * the UI was called. If it was, trigger an event that other classes
     * can see occur, if subscribed.
     */
    private static void CheckUiToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiEnabled = !uiEnabled;
            uiToggleEvent(uiEnabled);
        }
    }
}
