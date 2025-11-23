using UnityEngine;

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
        Destroy(gameObject);
        // Disable movement, play animation, reload scene, etc.
        // GetComponent<KingMovement>().enabled = false;
    }
}
