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
    
    // Refernces to the input field.
    public InputField seedInputField;
    public InputField minCornerHeightField;
    public InputField maxCornerHeightField;
    public InputField minNoiseAdditionField;
    public InputField maxNoiseAdditionField;
    public InputField noiseReductionFactorField;
    public InputField sideLengthField;
    public InputField nField;

    
    // A reference to the seed display text.
    public Text seedDisplayText;

    // A reference to the current terrain object.
    public GameObject terrainObject;

    // A reference to the player camera.
    public Transform playerCamera;

    // The multiplier which is used to determine the
    // camera's height when it is teleported above the terrain.
    public float cameraHeightMultiplier;

    private GenerateTerrain terrainScript;

	// Use this for initialization
	void Start ()
	{
        // Define the button's functionality when pressed.
        button.onClick.AddListener(GenerateTerrain);

	    terrainScript = terrainObject.GetComponent<GenerateTerrain>();

        // Initialize the text properly.
        SetSeedText(terrainScript.seed);
	}

    /**
     * Called when the button is pressed. Requests that the terrain object generate
     * a new terrain.
     */
    private void GenerateTerrain()
    {
        // Set parameters and get/generate a seed and the terrain.
        try
        {
            SetTerrainParameters();
        }
        catch (Exception)
        {
            Debug.LogWarning("Invalid terrain parameters.");
            return;
        }
        int seed = GetSeed();
        terrainScript.Generate(seed);
        SetSeedText(seed);

        // If the player's current position is under the new terrain, teleport them above the terrain.
        if (playerCamera.position.y < terrainScript.lowestHeight)
        {
            // Set player camera position to above the top right of the terrain.
            playerCamera.position = new Vector3(
                terrainScript.sideLength,
                cameraHeightMultiplier * terrainScript.lowestHeight, // 20% above the maximum height.
                terrainScript.sideLength
            );

            // Set player camera rotation.
            playerCamera.LookAt(terrainObject.transform);
        }
    }

    private void SetTerrainParameters()
    {
        float minCornerHeight = float.Parse(minCornerHeightField.text);
        float maxCornerHeight = float.Parse(maxCornerHeightField.text);
        float minNoiseAddition = float.Parse(minNoiseAdditionField.text);
        float maxNoiseAddition = float.Parse(maxNoiseAdditionField.text);
        float noiseReductionFactor = float.Parse(noiseReductionFactorField.text);
        float sideLength = float.Parse(sideLengthField.text);
        int n = int.Parse(nField.text);

        if (minCornerHeight > maxCornerHeight
            || minNoiseAddition > maxNoiseAddition
            || noiseReductionFactor < 0 || noiseReductionFactor > 1
            || sideLength <= 0
            || n > 7 || n < 1)
        {
            throw new Exception();
        }

        terrainScript.minCornerHeight = minCornerHeight;
        terrainScript.maxCornerHeight = maxCornerHeight;
        terrainScript.minNoiseAddition = minNoiseAddition;
        terrainScript.maxNoiseAddition = maxNoiseAddition;
        terrainScript.noiseReductionFactor = noiseReductionFactor;
        terrainScript.sideLength = sideLength;
        terrainScript.n = n;
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
