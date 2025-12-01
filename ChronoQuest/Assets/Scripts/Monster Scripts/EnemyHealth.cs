using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public float deathTime = 1;
    protected Animator animator;
    protected WizardMovement movement;   // Reference to movement script

    protected void Start()
    {
        currentHealth = maxHealth;

        // If animator is on child
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("Dead", false);
        // Get the movement script from this enemy
        movement = GetComponent<WizardMovement>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

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
        movement.isStunned = true;
        movement.stunCounter = 1;
        if (!animator.GetBool("Dead"))
        {
            animator.SetBool("Dead", true);
            animator.SetTrigger("Death");
        }
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}
