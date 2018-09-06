using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsUi : MonoBehaviour
{

    public float updatesPerSecond;
    public Text fpsText;

    private float maxWaitTime;
    private float currWaitTime;

    void Start()
    {
        maxWaitTime = 1 / updatesPerSecond;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    currWaitTime += Time.deltaTime;

	    if (currWaitTime > maxWaitTime)
	    {
	        currWaitTime = 0;
	        fpsText.text = string.Format("FPS: {0:###}", 1 / Time.smoothDeltaTime);
        }
	}
}
