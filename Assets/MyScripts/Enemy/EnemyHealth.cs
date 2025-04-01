using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip deathSound;
    public GameObject healthBarParent;       // Reference to the health bar parent object (with HorizontalLayoutGroup)
    public GameObject fullHeartPrefab;       // Full heart prefab
    public GameObject emptyHeartPrefab;      // Empty heart prefab
    public int maxHealth = 5;                // Maximum health (number of hearts)
    private int currentHealth;               // Current health

    private Animator anim;                   // Reference to the enemy's Animator
    private MeleeEnemy enemyMovement;        // Reference to the enemy's movement script
    private Collider2D enemyCollider;        // Reference to the enemy's collider
    private bool isDead = false;             // Prevent multiple deaths

    private void Start()
    {
        // Set the current health to max health initially
        currentHealth = maxHealth;

        // Get references to required components
        anim = GetComponent<Animator>();
        enemyMovement = GetComponent<MeleeEnemy>(); 
        enemyCollider = GetComponent<Collider2D>(); 

        // Initialize the health bar
        InitializeHealthBar();
    }

    // Initialize the health bar by instantiating hearts
    private void InitializeHealthBar()
    {
        // Clear existing hearts
        foreach (Transform child in healthBarParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Create full hearts based on maxHealth
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(fullHeartPrefab, healthBarParent.transform);
            heart.name = "Heart_" + i;
        }
    }

    // Call this method to deal damage
    public void TakeDamage(int damage)
    {
        if (isDead) return;  // Prevent taking damage after death

        currentHealth -= damage;

        // Play hurt animation
        anim.SetTrigger("hurt");
        SoundManager.instance.PlaySoundQuieter(hurtSounds[Random.Range(0, hurtSounds.Length)], 0.5f);
        // Ensure health doesn't go below 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar
        UpdateHealthBar();

        // If health reaches 0, die
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Called when the enemy's health reaches 0
    private void Die()
    {
        if (isDead) return; // Ensure Die() only runs once

        isDead = true; // Mark as dead immediately

        // Play die animation
        anim.SetTrigger("die");
        SoundManager.instance.PlaySoundQuieter(deathSound, 0.5f);

        // Disable movement
        if (enemyMovement != null)
            enemyMovement.enabled = false;

        // Disable patrol
        EnemyPatrol enemyPatrol = GetComponentInParent<EnemyPatrol>();
        if (enemyPatrol != null)
            enemyPatrol.enabled = false;

        // Stop physics movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Call RemoveEnemy after death animation
        StartCoroutine(RemoveEnemy());
    }



    private IEnumerator RemoveEnemy()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + .75f);

        // Disable animator after animation is done to prevent errors
        anim.enabled = false;

        // Destroy the entire parent object, including patrol points
        Destroy(transform.parent.gameObject);
    }


    // Update the health bar when the enemy takes damage
    private void UpdateHealthBar()
    {
        for (int i = 0; i < healthBarParent.transform.childCount; i++)
        {
            Image heartImage = healthBarParent.transform.GetChild(i).GetComponent<Image>();

            if (i < currentHealth)
            {
                heartImage.sprite = fullHeartPrefab.GetComponent<Image>().sprite; // Full heart
            }
            else
            {
                heartImage.sprite = emptyHeartPrefab.GetComponent<Image>().sprite; // Empty heart
            }
        }
    }
}
