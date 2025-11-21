using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    public int attackDamage = 25;
    public float attackRange = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private PlayerMovement controls;

    // Combo system
    private int currentAttack = 0;        // 0 = no attack, 1 = attack1, etc.
    private bool isAttacking = false;     // true while any attack is playing
    private bool canCombo = false;        // true when combo window is open
    private bool inputBuffered = false;   // true if player presses attack in combo window

    private void Awake()
    {
        controls = new PlayerMovement();
        controls.Gameplay.Attack.performed += ctx => OnAttackInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    // ------------------------------------------------
    // Called when the player presses attack
    // ------------------------------------------------
    private void OnAttackInput()
    {
        if (!isAttacking)
        {
            // Start first attack
            currentAttack = 1;
            isAttacking = true;
            animator.SetTrigger("Attack1");
        }
        else
        {
            // Always buffer input if an attack is playing
            inputBuffered = true;

            // If combo window is already open, trigger next attack immediately
            if (canCombo)
            {
                TriggerNextAttack();
                inputBuffered = false;
                canCombo = false;
            }
        }
    }


    // ------------------------------------------------
    // Called by animation event: opens the combo window
    // ------------------------------------------------
    public void OpenComboWindow()
    {
        canCombo = true;

        // If player already pressed attack, immediately trigger next attack
        if (inputBuffered)
        {
            TriggerNextAttack();
            inputBuffered = false;
            canCombo = false;
        }
    }

    // ------------------------------------------------
    // Called by animation event: closes the combo window
    // ------------------------------------------------
    public void CloseComboWindow()
    {
        canCombo = false;
        inputBuffered = false;
    }

    // ------------------------------------------------
    // Called by animation event at the end of an attack
    // ------------------------------------------------
    public void EndAttack()
    {
        isAttacking = false;
        currentAttack = 0;
        canCombo = false;
        inputBuffered = false;
    }

    // ------------------------------------------------
    // Handles chaining to next attack
    // ------------------------------------------------
    private void TriggerNextAttack()
    {
        if (currentAttack == 1)
        {
            currentAttack = 2;
            animator.SetTrigger("Attack2");
        }
        else if (currentAttack == 2)
        {
            currentAttack = 3;
            animator.SetTrigger("Attack3");
        }
    }

    // ------------------------------------------------
    // Damage logic remains unchanged
    // ------------------------------------------------
    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            var health = enemy.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
