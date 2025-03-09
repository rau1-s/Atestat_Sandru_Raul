using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerMovement;
    private float comboWindow = 0.5f; // Combo window time
    private float comboTimer;
    private int attackComboIndex = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        comboTimer += Time.deltaTime;

        if (Input.GetButtonDown("Fire2"))
        {
            HandleCombo();
        }
    }

    void HandleCombo()
    {
        if (comboTimer <= comboWindow)
        {
            attackComboIndex++; // Increment the combo index (1 -> 2 -> 3)
        }
        else
        {
            attackComboIndex = 1; // Reset combo if window has passed
        }

        if (attackComboIndex == 1)
        {
            anim.SetTrigger("Attack1");
            playerMovement.StopMovementDuringAttack(); // Disable movement during attack
        }
        else if (attackComboIndex == 2)
        {
            anim.SetTrigger("Attack2");
            playerMovement.StopMovementDuringAttack(); // Disable movement during attack
        }
        else if (attackComboIndex == 3)
        {
            anim.SetTrigger("Attack3");
            playerMovement.StopMovementDuringAttack(); // Disable movement during attack
            attackComboIndex = 0; // Reset combo after third attack
        }

        comboTimer = 0f; // Reset combo timer after each attack
    }

    public void OnAttackEnd()
    {
        playerMovement.EnableMovement(); // Re-enable movement after attack
    }
}
