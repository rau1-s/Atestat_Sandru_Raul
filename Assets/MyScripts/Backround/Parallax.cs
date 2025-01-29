using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public Transform followTarget;
    public float parallaxFactor = 0.5f;
    private Vector3 previousCamPos;

    void Start()
    {
        if (followTarget == null)
        {
            followTarget = Camera.main.transform;
        }
        previousCamPos = followTarget.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = new Vector3(followTarget.position.x - previousCamPos.x, 0, 0);        
        transform.position += deltaMovement * parallaxFactor;
        previousCamPos = followTarget.position;
    }
}