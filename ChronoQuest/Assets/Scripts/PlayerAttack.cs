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

    public void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        if (attackPoint == null)
        {
            Debug.LogError("❌ attackPoint is NOT assigned!");
            return;
        }

        Debug.Log("DealDamage was called correctly");
        foreach (var hit in hits)
        {
            var health = hit.GetComponentInParent<EnemyHealth>();
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
