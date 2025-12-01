using UnityEngine;

public class WizardDamage : MonoBehaviour
{
    public int damage;
    public PlayerHealth playerHealth;
    public KingMovement playerMovement;
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerMovement.knockBackCounter = playerMovement.knockBackTotalTime;

            if (collision.transform.position.x <= transform.position.x)
                playerMovement.knockFromRight = true;
            else
                playerMovement.knockFromRight = false;

            // Apply a strong impulse once - adjust these multipliers for feel
            float knockbackX = playerMovement.knockFromRight ? -playerMovement.knockBackForce : playerMovement.knockBackForce;
            float knockbackY = playerMovement.knockBackForce;

            knockbackX *= 1.8f; // Increase for more horizontal distance
            knockbackY *= 1.1f; // Increase for higher arc

            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(knockbackX, knockbackY);

            playerHealth.TakeDamage(damage);
        }
    }



}
