using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCamera : MonoBehaviour {

    public float movementSpeed;
    public float xLookSensitivity;
    public float yLookSensitivity;

    void Start() {
        // Lock the cursor to the application and hide the cursor.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        PerformCameraRotation();
        PerformCameraTranslation();
    }

    /**
     * Takes care of the camera rotation, based on mouse movement.
     */
    private void PerformCameraRotation() {
        // Get values for how much the axes have moved this frame.
        float xRotationAmount = Input.GetAxis("Mouse X") * xLookSensitivity;
        float yRotationAmount = Input.GetAxis("Mouse Y") * yLookSensitivity;

        // Translate those values and rotate the camera accordingly.
        transform.localEulerAngles += new Vector3(-yRotationAmount, xRotationAmount, 0) * Time.deltaTime;
    }

    /**
     * Takes care of the camera translation/movement, using WASD.
     */
    private void PerformCameraTranslation()
    {
        // Prepare a vector to hold the movement direction.
        Vector3 movement = Vector3.zero;

        // Add vectors if the appropriate keys are pressed.
        movement += Input.GetKey(KeyCode.W) ? transform.forward : Vector3.zero;
        movement += Input.GetKey(KeyCode.A) ? -transform.right : Vector3.zero;
        movement += Input.GetKey(KeyCode.S) ? -transform.forward : Vector3.zero;
        movement += Input.GetKey(KeyCode.D) ? transform.right : Vector3.zero;

        // Translate the camera.
        transform.position += movement.normalized * movementSpeed * Time.deltaTime;
    }
}
