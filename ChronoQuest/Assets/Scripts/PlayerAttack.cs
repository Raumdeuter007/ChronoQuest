using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Damage Settings")]
    public int attackDamage = 25;
    public float attackRange = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Combo Settings")]
    public float comboResetTime = 1.2f; // Time before combo resets
    private float comboTimer = 0f;

    private int comboStep = 0;       // 0 = no attack, 1-3 = attacks
    private bool canCombo = false;   // Combo window is open
    private bool attackQueued = false;

    private PlayerMovement controls;

    private void Awake()
    {
        controls = new PlayerMovement();
        controls.Gameplay.Attack.performed += ctx => OnAttackInput();
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    private void Update()
    {
        // Only allow combo timer reset for Attack1 and Attack2
        if (comboStep > 0 && comboStep < 3)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboResetTime)
            {
                Debug.Log("❗ Combo timer expired. Resetting combo.");
                EndCombo();
            }
        }
    }


    private void OnAttackInput()
    {
        if (comboStep < 3)
        {
            comboStep++;
            animator.SetInteger("AttackIndex", comboStep);
            animator.SetTrigger("Attack");
            comboTimer = 0f;
        }
        else
        {
            // already at max combo
            attackQueued = true;
        }
    }


    private void StartAttack(int step)
    {
        comboStep = step;
        comboTimer = 0f;
        attackQueued = false;

        animator.SetInteger("AttackIndex", comboStep);
        animator.SetTrigger("Attack");

        Debug.Log($"➡ Starting Attack {comboStep}");
    }

    // Called from Animation Event: opens combo window for next attack (only for step 1 and 2)
    public void OpenComboWindow()
    {
        canCombo = true;
        Debug.Log($"➡ OpenComboWindow called. comboStep={comboStep}, attackQueued={attackQueued}");

        if (attackQueued && comboStep < 3)
        {
            PlayNextComboStep();
        }
    }


    // Called from Animation Event: closes combo window after attack
    public void CloseComboWindow()
    {
        canCombo = false;
        attackQueued = false;
        Debug.Log($"⬛ CloseComboWindow called. comboStep={comboStep}");

    }

    private void PlayNextComboStep()
    {
        if (comboStep < 3 && attackQueued)
        {
            comboStep++;
            comboTimer = 0f;
            attackQueued = false;

            animator.SetInteger("AttackIndex", comboStep);
            animator.SetTrigger("Attack");

            Debug.Log($"➡ Playing next attack: {comboStep}");
        }
    }

    public void EndCombo()
    {
        Debug.Log("🔵 Combo ended. Resetting combo.");
        comboStep = 0;
        comboTimer = 0f;
        canCombo = false;
        attackQueued = false;
        animator.SetInteger("AttackIndex", 0);
    }

    public void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (var hit in hits)
        {
            var health = hit.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
