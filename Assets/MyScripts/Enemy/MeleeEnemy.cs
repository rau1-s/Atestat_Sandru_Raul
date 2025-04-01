using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private int damage;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionDelay;  // Time to wait after detecting player
    [SerializeField] private AudioClip swordSound;
    private float cooldownTimer = Mathf.Infinity;
    private float detectionTimer;
    private bool playerDetected;
    private Animator anim;
    private PlayerHealth playerHealth;
    private EnemyPatrol enemyPatrol;

    private void Start()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        detectionTimer += Time.deltaTime;

        // Only when player is seen
        if (PlayerInSight())
        {
            if (!playerDetected)
            {
                // Start detection delay and stop enemy movement
                detectionTimer = 0;
                playerDetected = true;
                enemyPatrol.enabled = false; // Stop patrolling
            }

            // Wait for detection delay
            if (detectionTimer >= detectionDelay && cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;

                // Only attack if the player is alive
                if (playerHealth != null && playerHealth.currentHealth > 0)
                {
                    SoundManager.instance.PlaySoundQuieter(swordSound, 0.5f);
                    anim.SetTrigger("MeleeAttack");
                }
            }
        }
        else
        {
            if (playerDetected)
            {
                playerDetected = false;
            }

            if (enemyPatrol != null)
                enemyPatrol.enabled = true; // Resume patrolling when player leaves
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<PlayerHealth>();
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if (PlayerInSight() && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
