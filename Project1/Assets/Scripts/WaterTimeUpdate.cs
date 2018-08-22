using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTimeUpdate : MonoBehaviour {
    public GameTime gameTime;

    private Renderer waterMaterial;
    
    // Use this for initialization
    void Start ()
    {
        waterMaterial = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
		waterMaterial.material.SetFloat("_GameTime", gameTime.GetTimeAsSeconds());
	}

}
