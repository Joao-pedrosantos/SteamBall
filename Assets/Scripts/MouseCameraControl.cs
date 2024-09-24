using UnityEngine;

public class MouseCameraControl : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity of the mouse
    public Transform playerBody; // Player's body to rotate

    float xRotation = 0f;

    void Start()
    {
        // Optional: Hide the cursor during gameplay
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculating rotation around the x-axis
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotation to camera and player body
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
