using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private Animator animator;
    public int hitTakenCount = 0;
    public HealHUD healHUD;
    public float deathTime = 1;
    protected WizardMovement movement;   // Reference to movement script

    protected void Start()
    {
        currentHealth = maxHealth;

        // If animator is on child
        animator = GetComponentInChildren<Animator>();
        //animator.SetBool("Dead", false);
        // Get the movement script from this enemy
        movement = GetComponent<WizardMovement>();
    }

    public virtual void TakeDamage(int damage)
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
        movement.isStunned = true;
        movement.stunCounter = 1;
        //if (!animator.GetBool("Dead"))
        //{
        //    animator.SetBool("Dead", true);
        //    animator.SetTrigger("Death");
        //}
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}
