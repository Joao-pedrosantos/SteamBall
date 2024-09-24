using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Reference to the player GameObject.
    public GameObject player;

    // The distance between the camera and the player.
    private Vector3 offset;

    // Sensitivity of the camera rotation.
    public float rotationSpeed = 5.0f;

    // Boolean to track if the menu is open
    private bool isMenuOpen = false;

    // Reference to the menu (replace this with your actual menu object or method)
    public GameObject menuScreen;

    // Start is called before the first frame update.
    void Start()
    {
        // Calculate the initial offset between the camera's position and the player's position.
        offset = transform.position - player.transform.position;

        // Ensure the menu is hidden at the start
        if (menuScreen != null)
        {
            menuScreen.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the 'P' key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMenu();
        }
    }

    // LateUpdate is called once per frame after all Update functions have been completed.
    void LateUpdate()
    {
        // Only move the camera if the menu is not open
        if (!isMenuOpen)
        {
            // Get the mouse input for rotating the camera.
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed;

            // Rotate the offset around the player based on mouse input.
            Quaternion camTurnAngle = Quaternion.AngleAxis(horizontal, Vector3.up);

            // Apply the rotation to the offset.
            offset = camTurnAngle * offset;

            // Optionally, you can clamp the vertical rotation to prevent flipping the camera upside down.
            offset = Quaternion.AngleAxis(vertical, transform.right) * offset;

            // Maintain the same offset between the camera and player throughout the game.
            transform.position = player.transform.position + offset;

            // Make the camera look at the player.
            transform.LookAt(player.transform.position);
        }
    }

    // Method to toggle the menu screen
    void ToggleMenu()
    {
        if (menuScreen != null)
        {
            isMenuOpen = !isMenuOpen; // Toggle the menu state
            menuScreen.SetActive(isMenuOpen); // Show or hide the menu
        }
    }
}
