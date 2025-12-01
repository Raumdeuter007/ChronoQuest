using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 20;
    public float attackRange = 0.5f;
    public float attackCooldown = 1f;
    public Transform attackPoint;
    public LayerMask playerLayer;

    private float attackTimer = 0f;
    [SerializeField] protected Animator animator;
    protected Collider2D playerHit;

    protected void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
    }

    // Call this from WizardMovement when close to player
    public void TryAttack()
    {
        if (attackTimer > 0f) return; // still in cooldown

        // Detect player in range and store it
        playerHit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (playerHit != null)
        {
            // Trigger attack animation
            if (animator != null)
                animator.SetTrigger("Attack");

            attackTimer = attackCooldown; // reset cooldown
        }
    }

    // This will be called via Animation Event at the hit frame
    public void ApplyAttackHit()
    {
        if (playerHit == null) return;

        // Damage
        var playerHealth = playerHit.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        // Knockback
        var playerMovement = playerHit.GetComponent<KingMovement>();
        var rb = playerHit.GetComponent<Rigidbody2D>();
        if (playerMovement != null && rb != null)
        {
            playerMovement.knockBackCounter = playerMovement.knockBackTotalTime;
            playerMovement.knockFromRight = (playerHit.transform.position.x <= transform.position.x);

            float knockbackX = playerMovement.knockFromRight ? -playerMovement.knockBackForce : playerMovement.knockBackForce;
            float knockbackY = playerMovement.knockBackForce;

            knockbackX *= 1.8f;
            knockbackY *= 1.1f;

            rb.linearVelocity = new Vector2(knockbackX, knockbackY);
        }

        // Clear reference
        playerHit = null;
    }
    public bool CanAttack() { return attackTimer <= 0f; }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
