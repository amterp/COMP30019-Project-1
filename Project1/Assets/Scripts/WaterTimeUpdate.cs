using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTimeUpdate : MonoBehaviour {
    public GameTime gameTime;
    public float amplitude = 0.06f;
    public float wavelength = 1.5f;
    public float frequency = 2.0f;
    public float tideAmplitude = 0.1f;
    public float tideFrequency = 0.5f;


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
        waterMaterial.material.SetFloat("_TideFrequency", tideFrequency);
        waterMaterial.material.SetFloat("_TideAmplitude", tideAmplitude);
    }

}
