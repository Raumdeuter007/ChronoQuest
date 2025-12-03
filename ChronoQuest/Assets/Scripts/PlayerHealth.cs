using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    private void Start()
    {
        // Set health to full at start
        currentHealth = maxHealth;
    }

    // Call this to apply damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Make sure health stays in range
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    // Call this to heal
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        Animator animator = GetComponent<Animator>();
        if (!animator.GetBool("Dead"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            animator.SetBool("Dead", true);
            animator.SetTrigger("Death");
            StartCoroutine(Death());
            Destroy(GetComponent<CapsuleCollider2D>());
        }
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Stages.ResetAllProgress();
        GameManager.sceneController.LoadScene("main_menu");
    }
}
