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
    private int comboStep = 0;

    public void FixedUpdate()
    {
        AnimatorStateInfo tag = animator.GetCurrentAnimatorStateInfo(0);
        if (!tag.IsTag("Attack") && !tag.IsTag("Transition"))
        {
            comboStep = 0;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                comboStep++;
                if (comboStep > 3) comboStep = 1;
                animator.SetTrigger("Attack");
            }
        }
    }

    // private void StartAttack(int step)
    // {
    //     comboStep = step;
    //     comboTimer = 0;
    //     attackQueued = false;

    //     animator.SetInteger("AttackIndex", comboStep);

    //     Debug.Log($"➡ Starting Attack {comboStep}");
    // }

    // Called from Animation Event: opens combo window for next attack (only for step 1 and 2)
    // public void OpenComboWindow()
    // {
    //     Debug.Log($"➡ OpenComboWindow called. comboStep={comboStep}, attackQueued={attackQueued}");

    //     if (attackQueued && comboStep < 3)
    //     {
    //         PlayNextComboStep();
    //     }
    // }


    // Called from Animation Event: closes combo window after attack
    // public void CloseComboWindow()
    // {
    //     attackQueued = false;
    //     Debug.Log($"⬛ CloseComboWindow called. comboStep={comboStep}");
    // }

    // private void PlayNextComboStep()
    // {
    //     if (comboStep < 3 && attackQueued)
    //     {
    //         comboStep++;
    //         comboTimer = 0;
    //         attackQueued = false;

    //         animator.SetInteger("AttackIndex", comboStep);

    //         Debug.Log($"➡ Playing next attack: {comboStep}");
    //     }
    // }

    // public void EndCombo()
    // {
    //     Debug.Log("🔵 Combo ended. Resetting combo.");
    //     comboStep = 0;
    //     comboTimer = 0;
    //     attackQueued = false;
    //     animator.SetInteger("AttackIndex", 0);
    // }

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
