using UnityEngine;
using Unity.Cinemachine;

public class CameraFollowWithJumpAndFallOffset : MonoBehaviour
{
    public CinemachineCamera virtualCamera;  // Reference to the Cinemachine Virtual Camera
    public Transform player;  // Reference to the player (the object being followed by the camera)
    public float jumpHeight = 2f;  // The height to offset when jumping
    public float fallOffset = 1.5f;  // The offset to apply when falling
    public float smoothingSpeed = 5f;  // How smooth the camera follows the jump or fall

    private float originalYPos;  // Original camera Y position (ground level)
    private bool isJumping = false;
    private bool isFalling = false;
    private Vector3 targetCameraPosition;  // Target position of the camera

    void Start()
    {
        // Store the original Y position of the camera (ground level)
        originalYPos = virtualCamera.transform.position.y;
        targetCameraPosition = virtualCamera.transform.position;
    }

    void Update()
    {
        // Get the player's vertical velocity to detect jumping and falling
        float playerVelocityY = player.GetComponent<Rigidbody2D>().linearVelocity.y;

        // Check if the player is jumping (positive vertical velocity)
        bool isCurrentlyJumping = playerVelocityY > 0;
        bool isCurrentlyFalling = playerVelocityY < 0;

        // If the player is jumping, apply the jump height offset
        if (isCurrentlyJumping && !isJumping)
        {
            isJumping = true;
            isFalling = false;
            // Set the target camera position to follow the jump height
            targetCameraPosition.y = originalYPos + jumpHeight;
        }
        // If the player is falling, apply the fall offset
        else if (isCurrentlyFalling && !isFalling)
        {
            isFalling = true;
            isJumping = false;
            // Set the target camera position to follow the fall offset
            targetCameraPosition.y = originalYPos - fallOffset;
        }
        // If the player is on the ground (velocity is near zero), reset the camera position
        else if (!isCurrentlyJumping && !isCurrentlyFalling)
        {
            isJumping = false;
            isFalling = false;
            targetCameraPosition.y = originalYPos;
        }

        // Smoothly move the camera vertically to the target position
        virtualCamera.transform.position = Vector3.Lerp(
            virtualCamera.transform.position,
            new Vector3(virtualCamera.transform.position.x, targetCameraPosition.y, virtualCamera.transform.position.z),
            Time.deltaTime * smoothingSpeed
        );
    }
}
