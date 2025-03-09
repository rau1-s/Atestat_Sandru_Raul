using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBar : MonoBehaviour
{
    public int maxHealth = 10;  // 5 hearts, each representing 2 HP
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    public GameObject heartPrefab;  // Prefab for heart UI elements
    public PlayerHealth playerHealth; // Reference to the PlayerHealth script

    private List<Image> hearts = new List<Image>();

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        CreateHearts();
        UpdateHearts();
    }


    void CreateHearts()
    {
        for (int i = 0; i < 5; i++) // Always create 5 hearts
        {
            GameObject heartObj = Instantiate(heartPrefab, transform);
            Image heartImage = heartObj.GetComponent<Image>();
            hearts.Add(heartImage);
        }
    }

    public void UpdateHearts()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            
            if (playerHealth == null)
            {
                Debug.LogError("UpdateHearts() called, but playerHealth is STILL NULL! Make sure PlayerHealth exists in the scene.");
                return;
            }
            Debug.Log("HealthBar: Successfully assigned PlayerHealth.");
        }

        int currentHealth = playerHealth.currentHealth;
        Debug.Log("Updating Hearts: Current Health = " + currentHealth);

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartValue = (i + 1) * 2;

            if (currentHealth >= heartValue)
                hearts[i].sprite = fullHeart;
            else if (currentHealth == heartValue - 1)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
