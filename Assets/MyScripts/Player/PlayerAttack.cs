using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackWidth = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioClip[] swordSounds;

    private Animator anim;
    private PlayerMovement playerMovement;
    private Collision coll;

    private float comboWindow = 0.5f;
    private float comboTimer;
    private int attackComboIndex = 0;
    private bool canAttack = true;
    private bool isAttacking = false;  // NEW: Track if an attack is in progress

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        coll = GetComponent<Collision>();
    }

    private void Update()
    {
        comboTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire2") && coll.onGround)
        {
            HandleCombo();
        }
    }

    private void HandleCombo()
    {
        if (!canAttack) return;

        isAttacking = true; // Mark that an attack has started

        if (comboTimer <= comboWindow)
        {
            attackComboIndex++;
        }
        else
        {
            attackComboIndex = 1;
        }

        if (attackComboIndex == 1)
        {
            anim.SetTrigger("Attack1");
        }
        else if (attackComboIndex == 2)
        {
            anim.SetTrigger("Attack2");
        }
        else if (attackComboIndex == 3)
        {
            anim.SetTrigger("Attack3");
            canAttack = false;
        }

        playerMovement.StopMovementDuringAttack();
        comboTimer = 0f;
    }

    public void PerformAttack()
    {
        Debug.Log("PerformAttack() called!");

        SoundManager.instance.PlaySoundQuieter(swordSounds[Random.Range(0, swordSounds.Length)], 0.5f);

        float direction = playerMovement.side;
        Vector3 boxCenter = transform.position + new Vector3(direction * attackRange, 0, 0);
        Vector2 boxSize = new Vector2(attackWidth, 1f);

        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0, Vector2.zero, 0, enemyLayer);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);
            hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(1);
        }
        else
        {
            Debug.Log("No hit. Check enemy layer & position.");
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false; // Reset attack state

        if (attackComboIndex == 3) 
        {
            attackComboIndex = 0;
            canAttack = true;
        }

        playerMovement.EnableMovement();
    }

    // NEW: Call this when the player takes damage
    public void OnPlayerHit()
    {
        if (isAttacking)
        {
            Debug.Log("Player hit mid-attack! Resetting attack state.");

            isAttacking = false;
            attackComboIndex = 0;
            canAttack = true;

            playerMovement.EnableMovement();
        }
    }
}
