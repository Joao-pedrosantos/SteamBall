using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Rigidbody of the player.
    private Rigidbody rb;
    private int count;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;

    // Speed at which the player moves.
    public float speed = 5f;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI timerText; // New TextMeshPro for displaying the timer
    public TextMeshProUGUI jumpCountText; // UI element for displaying jump counts

    // Jump force to apply when jumping.
    public float jumpForce = 5f;

    // Check if the player is on the ground.
    private bool isGrounded = true;

    // Count the number of jumps made (to limit to double jumps).
    private int jumpCount = 0;

    // Maximum number of jumps the player can make.
    public int maxJumpCount = 2;

    // Track the last safe position.
    private Vector3 lastSafePosition;

    // Reference to the main camera.
    public Transform cameraTransform;

    // Countdown timer related variables
    private float remainingTime = 90f; // Set 3 minutes (180 seconds) countdown
    private bool timerRunning = true;

    // Audio sources for various sounds
    public AudioSource jumpSound;      // Sound for jumping
    public AudioSource jumpPadSound;   // Sound for landing on a jump pad
    public AudioSource teleportSound;  // Sound for teleporting back after falling
    public AudioSource pickupSound;    // Sound for picking up an item

    // Start is called before the first frame update.
    void Start()
    {
        count = 0;

        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();

        // Initialize last safe position to the starting position of the player.
        lastSafePosition = transform.position;

        SetCountText();
        UpdateTimerText(); // Initialize the timer text
        UpdateJumpCountText(); // Initialize the jump count text
    }

    void UpdateJumpCountText()
    {
        jumpCountText.text = "Jumps Left: " + (maxJumpCount - jumpCount);
    }

    void SetCountText()
    {
        countText.text = "Keys: " + count.ToString();
    }

    void ShowMessage(string message)
    {
        timerText.text = message;  // Display message using the timerText element for now
        StartCoroutine(HideMessageAfterTime(3));  // Hide message after 3 seconds
    }

    IEnumerator HideMessageAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        timerText.text = "";  // Clear the message after time has passed
    }

    // Handle pickup logic and timer increment
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            count += 1;
            other.gameObject.SetActive(false);
            SetCountText();

            // Play pickup sound if assigned
        if (pickupSound != null && !pickupSound.enabled)
        {
            pickupSound.enabled = true; // Ensure the AudioSource is enabled
            pickupSound.Play();
        }


            // Add 5 seconds to the timer
            remainingTime += 5f;
            remainingTime = Mathf.Min(remainingTime, 300f);  // Cap at 5 minutes
            UpdateTimerText();

            // Unlock extra jump after 5 pickups
            if (count % 5 == 0)
            {
                maxJumpCount++;
                ShowMessage("Extra jump unlocked!");
            }
        }
    }

    // Handle player movement
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Handle player jumping
    void OnJump(InputValue jumpValue)
    {
        if ((isGrounded || jumpCount < maxJumpCount) && jumpValue.isPressed)
        {
            // Apply jump force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Increment the jump count
            jumpCount++;

            // Update the jump count UI
            UpdateJumpCountText();

            // Player is no longer grounded after jumping
            isGrounded = false;

            // Play the jump sound if the AudioSource is assigned
            if (jumpSound != null)
            {
                jumpSound.Play();
            }
        }
    }

    // FixedUpdate handles the player's movement physics
    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        // Adjust movement based on the camera's orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * movementY + right * movementX).normalized;

        rb.AddForce(moveDirection * speed);
    }

    // Handle collisions with the ground or jump pads
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("JumpPad"))
        {
            isGrounded = true;
            jumpCount = 0;  // Reset jump count upon landing
            UpdateJumpCountText();

            // Update last safe position
            lastSafePosition = transform.position;

            // If the collision is with a JumpPad, apply the launch force and play sound
            if (collision.gameObject.CompareTag("JumpPad"))
            {
                float launchForce = 20f;  // Adjust force as necessary
                rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);

                // Play the JumpPad sound if the AudioSource is assigned
                if (jumpPadSound != null)
                {
                    jumpPadSound.Play();
                }
            }
        }

        if (collision.gameObject.CompareTag("Barrel"))
    {
        remainingTime -= 20f; // Subtract 20 seconds
        remainingTime = Mathf.Max(remainingTime, 0f); // Ensure time doesn't go negative
        UpdateTimerText();

        // Optionally destroy the barrel on impact
        Destroy(collision.gameObject);
    }

        if (collision.gameObject.CompareTag("Victory"))
        {
            SceneManager.LoadSceneAsync(4);
        }
    }

    // Handle player falling off the platform
    void Update()
    {
        if (transform.position.y < -50)
        {
            // Teleport the player back to the last safe position.
            transform.position = lastSafePosition;
            rb.velocity = Vector3.zero;

            // Subtract 10 seconds for falling
            remainingTime -= 10f;
            remainingTime = Mathf.Max(remainingTime, 0f);
            UpdateTimerText();

            // Play the teleport sound if the AudioSource is assigned
            if (teleportSound != null)
            {
                teleportSound.Play();
            }
        }

        // Handle countdown timer logic
        if (timerRunning)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                remainingTime = 0;
                timerRunning = false;
                UpdateTimerText();
                TimerEnded();
            }
        }
    }

    // Update the timer text display
    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = "Time: " + minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    // Function called when the timer ends
// Assuming remainingTime is your timer variable in PlayerController
void TimerEnded()
{
    PlayerPrefs.SetFloat("LastedTime", remainingTime);
    Debug.Log("Time's up!");
    SceneManager.LoadSceneAsync("Endgame"); // Make sure this matches your endgame scene name
}

}
