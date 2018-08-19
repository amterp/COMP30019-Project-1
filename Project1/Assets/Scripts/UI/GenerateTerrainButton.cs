using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateTerrainButton : MonoBehaviour
{
    // A reference to the current terrain object.
    public GameObject currentTerrainObject;
    // A reference to the material used for the terrain.
    public Material terrainMaterial;

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
     * Removes the old terrain, instantiates a new one, and sets up the parameters
     * in the terrain script.
     */
    private void GenerateTerrain()
    {
        // Get a reference to the old script.
        GenerateTerrain oldTerrainScript = currentTerrainObject.GetComponent<GenerateTerrain>();

        // Clean up old terrain.
        Destroy(currentTerrainObject);

        // Instantiate new terrain.
        currentTerrainObject = new GameObject();
        currentTerrainObject.AddComponent<GenerateTerrain>();
        currentTerrainObject.GetComponent<MeshRenderer>().material = terrainMaterial;

        // Get and set new terrain parameters.

        GenerateTerrain terrainScript = currentTerrainObject.GetComponent<GenerateTerrain>();
        terrainScript.n = oldTerrainScript.n;
        terrainScript.sideLength = oldTerrainScript.sideLength;
        // For now, this will make the button essentially generate a random terrain.
        terrainScript.seed = Time.frameCount;
        terrainScript.minCornerHeight = oldTerrainScript.minCornerHeight;
        terrainScript.maxCornerHeight = oldTerrainScript.maxCornerHeight;
        terrainScript.minHeightAddition = oldTerrainScript.minHeightAddition;
        terrainScript.maxHeightAddition = oldTerrainScript.maxHeightAddition;
        terrainScript.heightAdditionFactor = oldTerrainScript.heightAdditionFactor;
    }
}
