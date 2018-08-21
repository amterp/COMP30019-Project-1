using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateTerrainButton : MonoBehaviour
{
    // A reference to the actual button that will be pressed to trigger
    // script.
    public Button button;
    
    // A reference to the seed input field.
    public InputField seedInputField;
    
    // A reference to the seed display text.
    public Text seedDisplayText;

    // A reference to the current terrain object.
    public GameObject terrainObject;

    // A reference to the player camera.
    public Transform playerCamera;

    // The multiplier which is used to determine the
    // camera's height when it is teleported above the terrain.
    public float cameraHeightMultiplier;

	// Use this for initialization
	void Start ()
	{
        // Define the button's functionality when pressed.
        button.onClick.AddListener(GenerateTerrain);

        // Initialize the text properly.
        SetSeedText(terrainObject.GetComponent<GenerateTerrain>().seed);
	}

    /**
     * Called when the button is pressed. Requests that the terrain object generate
     * a new terrain.
     */
    private void GenerateTerrain()
    {
        // Get a reference to the old script.
        GenerateTerrain oldTerrainScript = terrainObject.GetComponent<GenerateTerrain>();

        // Generate a seed and the terrain.
        int seed = GetSeed();
        oldTerrainScript.Generate(seed);
        SetSeedText(seed);

        // If the player's current position is under the new terrain, teleport them above the terrain.
        if (playerCamera.position.y < oldTerrainScript.maxHeight)
        {
            // Set player camera position to above the top right of the terrain.
            playerCamera.position = new Vector3(
                oldTerrainScript.sideLength,
                cameraHeightMultiplier * oldTerrainScript.maxHeight, // 20% above the maximum height.
                oldTerrainScript.sideLength
            );

            // Set player camera rotation.
            playerCamera.LookAt(terrainObject.transform);
        }
    }

    /**
     * Returns a seed, utilizing the seed input field.
     */
    private int GetSeed()
    {
        String seed = seedInputField.text;

        if (seed.Length == 0)
        {
            // The seed input field is empty. Default to a "random" seed i.e.
            // the last X digits from current time in ticks where X is the max
            // number of digits for a seed.
            long ticks = DateTime.Now.Ticks;
            long modNum = (long) (Mathf.Pow(10, seedInputField.characterLimit - 1));
            return (int) (ticks % modNum);
        }
        else
        {
            // There is input in the field - use it.
            return int.Parse(seed);
        }
    }

    /**
     * Sets the text when given a seed.
     */
    private void SetSeedText(int seed)
    {
        seedDisplayText.text = "Current map seed: " + seed;
    }
}
