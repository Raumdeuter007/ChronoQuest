using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private Animator animator;
    private WizardMovement movement;   // Reference to movement script
    public int hitTakenCount = 0;
    public HealHUD healHUD;       
    void Start()
    {
        currentHealth = maxHealth;

        // If animator is on child
        animator = GetComponentInChildren<Animator>();

        // Get the movement script from this enemy
        movement = GetComponent<WizardMovement>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hitTakenCount++;

        if (hitTakenCount == 2)
        {
            healHUD.Heal();
            hitTakenCount = 0;
        }

        // Stop movement while hit
        if (movement != null)
        {
            movement.isStunned = true;
            movement.stunCounter = movement.stunTime;
        }

        // Play hit animation
        if (animator != null)
            animator.SetTrigger("Hit");

        // Check for death
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
