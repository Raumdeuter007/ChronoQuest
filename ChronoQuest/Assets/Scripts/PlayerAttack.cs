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

    private void Awake()
    {
        controls = new PlayerMovement();
        controls.Gameplay.Attack.performed += ctx => Attack();
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
    private void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack1");
    }

    // ------------------------------------------------
    // Called by animation event in the attack animation
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
