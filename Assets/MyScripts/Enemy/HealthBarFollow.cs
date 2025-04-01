using UnityEngine;

public class HealthBarFollow : MonoBehaviour
{
    public Camera mainCamera;  // Reference to the main camera

    private void Update()
    {
        // Ensure the health bar faces the camera by rotating it
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        directionToCamera.y = 0; // Don't rotate the health bar on the y-axis, to prevent flipping
        transform.LookAt(transform.position + directionToCamera);
    }
}
