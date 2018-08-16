using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWithUi : MonoBehaviour
{
    // This can be set to true if this game object should be disabled
    // when the UI is enabled, and visa versa.
    public bool invert = false;

	// Use this for initialization
	void Start () {
		// Subscribe to the toggle event.
	    GameController.uiToggleEvent += OnUiToggle;
	}

    /**
     * A delegate that gets called when the Ui is toggled in GameController.
     * Toggles the game object as required.
     */
    private void OnUiToggle(bool uiEnabled)
    {
        gameObject.SetActive(uiEnabled ^ invert);
    }
}
