using NUnit.Framework.Internal;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float lifetime = 5f;

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionDuration = 0.5f; // How long the explosion animation plays
    [SerializeField] private Vector3 explosionScale = new Vector3(5f, 5f, 1f);

    private bool isLaunched = false;
    private Animator animator;
    private bool hasExploded = false;


    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();

        // Rotate the fireball to point downward when it spawns
        transform.rotation = Quaternion.Euler(0, 0, -90);

        // Destroy after lifetime to prevent memory leaks
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (isLaunched && !hasExploded)
        {
            // Move DOWN by reducing Y position
            transform.position += new Vector3(0, -fallSpeed * Time.deltaTime, 0);
        }
    }

    public void Launch(Vector2 playerPosition)
    {
        isLaunched = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return; // Prevent multiple explosions

        if (other.CompareTag("Player"))
        {
            // Get the PlayerHealth component and deal damage
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Fireball hit player and dealt " + damageAmount + " damage!");
            }

            // Play explosion animation
            PlayExplosion();
        }
        else if (other.CompareTag("Ground"))
        {
            // Play explosion when hitting ground
            PlayExplosion();
        }
    }

    private void PlayExplosion()
    {
        hasExploded = true;

        // Stop the fireball from moving
        isLaunched = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
        transform.localScale = explosionScale;
        // Trigger the explosion animation
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }

        // Disable the collider so it doesn't trigger again
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Destroy the fireball after the explosion animation finishes
        Destroy(gameObject, explosionDuration);
    }
}
