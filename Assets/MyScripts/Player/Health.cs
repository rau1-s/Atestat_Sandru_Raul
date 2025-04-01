using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    public HealthBar healthBar;  // Reference to the HealthBar script
    public Animator anim;  // Reference to the Animator
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script
    public PlayerAttack attack;
    private bool dead;

    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip deathSound;
    private SpriteRenderer spriteRend;

    void Start()
    {
        currentHealth = 6;
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();  // Ensure PlayerMovement is referenced
        healthBar = FindObjectOfType<HealthBar>();
        healthBar.UpdateHearts();  // Update UI at start
        spriteRend = GetComponent<SpriteRenderer>();
        attack = GetComponent<PlayerAttack>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the HealthBar UI after health change
        healthBar.UpdateHearts();  // Now only update UI from PlayerHealth

        Debug.Log(currentHealth);

        if (currentHealth > 0)
        {
            SoundManager.instance.PlaySoundQuieter(hurtSounds[Random.Range(0, hurtSounds.Length)], 0.5f);
            attack.OnPlayerHit();
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
        }
        else 
        {
            if(!dead)
            {
                anim.SetTrigger("die");
                SoundManager.instance.PlaySoundQuieter(deathSound, 0.5f);
                GetComponent<PlayerMovement>().enabled = false; // Disable movement when dead
                dead = true;
            }
        }
    }

    public void Heal(int amount)
    {
        if(currentHealth == 9)
            currentHealth += 1; // Special case: Add 1 when at max health minus 1
        else
            currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        anim.SetTrigger("heal");
        playerMovement.enabled = false; // Disable movement during healing animation
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;  // Stops all movement
        // Update the HealthBar UI after health change
        healthBar.UpdateHearts();  // Now only update UI from PlayerHealth

        // Call Coroutine to re-enable movement after animation finishes
        StartCoroutine(EnableMovementAfterHealing());
    }

    private IEnumerator EnableMovementAfterHealing()
    {
        // Wait for the healing animation to finish
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim = GetComponentInParent<Animator>();
        anim.SetFloat("HorizontalAxis", 0);
        playerMovement.enabled = true;  // Re-enable movement after animation
    }

    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for(int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));

        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
}
