using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateTerrainButton : MonoBehaviour
{
    // A reference to the actual button that will be pressed to trigger
    // script.
    public Button button;

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
}
