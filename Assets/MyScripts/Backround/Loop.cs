using UnityEngine;

public class ParallaxRepeater : MonoBehaviour
{
    private float width;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        if (transform.position.x < cam.position.x - width * 1.5f)
        {
            transform.position += new Vector3(width * 3, 0, 0);
        }
        else if (transform.position.x > cam.position.x + width * 1.5f)
        {
            transform.position -= new Vector3(width * 3, 0, 0);
        }

        /**
        💡 Why width * 1.5f?
        This ensures that when the leftmost background moves out of view, it gets placed after the rightmost one, maintaining the loop.
        **/
    }
}
