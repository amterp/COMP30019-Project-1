using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTimeUpdate : MonoBehaviour {
    public GameTime gameTime;
    public float amplitude = 0.5f;
    public float wavelength = 3.0f;
    public float frequency = 2.0f;

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
