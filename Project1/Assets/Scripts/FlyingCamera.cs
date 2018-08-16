using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour
{
    public float xLookSensitivity;
    public float yLookSensitivity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    PerformCameraRotation();
	}

    private void PerformCameraRotation()
    {
        float xRotationAmount = Input.GetAxis("Mouse X") * xLookSensitivity;
        float yRotationAmount = Input.GetAxis("Mouse Y") * yLookSensitivity;

        transform.localEulerAngles += new Vector3(-yRotationAmount, xRotationAmount, 0) * Time.deltaTime;
    }
}
