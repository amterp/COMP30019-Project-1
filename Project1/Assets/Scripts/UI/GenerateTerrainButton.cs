using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateTerrainButton : MonoBehaviour
{
    // A reference to the current terrain object.
    public GameObject terrainObject;

    // A reference to the actual button that will be pressed to trigger
    // script.
    private Button button;

	// Use this for initialization
	void Start ()
	{
        // Define the button's functionality when pressed.
	    button = GetComponent<Button>();
        button.onClick.AddListener(GenerateTerrain);
	}

    /**
     * Called when the button is pressed. Requests that the terrain object generate
     * a new terrain.
     */
    private void GenerateTerrain()
    {
        // Get a reference to the old script.
        GenerateTerrain oldTerrainScript = terrainObject.GetComponent<GenerateTerrain>();
        oldTerrainScript.Generate(Time.frameCount);
    }
}
