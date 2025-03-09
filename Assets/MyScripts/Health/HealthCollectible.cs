using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    private int HealthValue = 2;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<PlayerHealth>().Heal(HealthValue);
            gameObject.SetActive(false);
        }
    }
}
