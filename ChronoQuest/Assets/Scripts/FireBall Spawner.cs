using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnOffsetX = 3f; // Distance to the right
    [SerializeField] private float spawnOffsetY = 5f; // Distance above
    [SerializeField] private float spawnInterval = 2f; // Time between spawns

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnFireball();
            spawnTimer = 0f;
        }
    }

    public void SpawnFireball()
    {
        if (player == null || fireballPrefab == null)
        {
            Debug.LogWarning("Player or Fireball Prefab not assigned!");
            return;
        }

        // Get player's current position
        Vector2 playerPos = player.position;

        // Calculate spawn position (bottom-right above player)
        Vector2 spawnPos = new Vector2(
            playerPos.x + spawnOffsetX,
            playerPos.y + spawnOffsetY
        );

        // Instantiate the fireball
        GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        // Launch it toward the player's position
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
        {
            fireballScript.Launch(playerPos);
        }
    }
}