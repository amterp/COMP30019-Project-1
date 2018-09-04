using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTimeUpdate : MonoBehaviour {
    public GameTime gameTime;
    public float amplitude = 0.2f;
    public float wavelength = 1.0f;
    public float frequency = 1.0f;

    private Renderer waterMaterial;
    
    // Use this for initialization
    void Start ()
    {
        waterMaterial = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
		waterMaterial.material.SetFloat("_GameTime", gameTime.GetTimeAsSeconds());
        waterMaterial.material.SetFloat("_Amplitude", amplitude);
        waterMaterial.material.SetFloat("_Wavelength", wavelength);
        waterMaterial.material.SetFloat("_Frequency", frequency);
    }

}
