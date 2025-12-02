using UnityEngine;
using System.Collections;
using System;

public class SnowBossMovement : WizardMovement
{
    [Header("Boss Behavior Settings")]
    public float idleTimeBeforeTeleport = 5f; // Time in idle before starting teleport
    public float walkDistance = 3f; // Distance to start walking toward player
    public float teleportDistanceBehind = 2f;

    [Header("Boundary Settings")]
    public Transform tilemapBoundsMin;
    public Transform tilemapBoundsMax;

    private enum BossState { Idle, Walking, TeleportAttack, Stunned }
    private BossState currentState = BossState.Idle;

    private Vector3 spawnPosition;
    private float idleTimer = 0f;
    private SnowBossHealth bossHealth;
    private SnowBossAttack bossAttack;
    private bool wasAttackedDuringIdle = false;

    new void Start()
    {
        // Don't call base.Start() to avoid patrol initialization
        anim = GetComponentInChildren<Animator>();
        bossHealth = GetComponent<SnowBossHealth>();
        bossAttack = GetComponent<SnowBossAttack>();
        attackScript = bossAttack;

        spawnPosition = transform.position;
    }

    void Update()
    {
        // Handle base stun from EnemyHealth
        if (isStunned)
        {
            stunCounter -= Time.deltaTime;
            if (stunCounter <= 0)
                isStunned = false;
            return;
        }

        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Walking:
                HandleWalkingState();
                break;
        }
    }

    private void HandleIdleState()
    {
        SetMoving(false);

        // Check distance to player
        float distanceToPlayer = Math.Abs(transform.position.x - playerTransform.position.x);

        if (distanceToPlayer > walkDistance)
        {
            // Switch to walking
            currentState = BossState.Walking;
            idleTimer = 0f;
            return;
        }

        // Increment idle timer
        idleTimer += Time.deltaTime;

        // Check if should start teleport sequence
        if (idleTimer >= idleTimeBeforeTeleport || wasAttackedDuringIdle)
        {
            StartTeleportSequence();
        }
    }

    private void HandleWalkingState()
    {
        // Face player
        if (playerTransform.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        // Walk toward player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * moveSpeed * Time.deltaTime;
        SetMoving(true);

        // Check if close enough to switch to idle
        float distanceToPlayer = Math.Abs(transform.position.x - playerTransform.position.x);

        if (distanceToPlayer <= walkDistance)
        {
            currentState = BossState.Idle;
            idleTimer = 0f;
        }
    }

    private void StartTeleportSequence()
    {
        currentState = BossState.TeleportAttack;
        idleTimer = 0f;
        wasAttackedDuringIdle = false;

        if (bossHealth != null)
            bossHealth.SetImmune(true);

        if (bossAttack != null)
            bossAttack.StartTeleportAttackSequence();
    }

    // Called by BossAttack during teleport sequence
    public void TeleportBehindPlayer()
    {
        if (playerTransform == null)
            return;

        Vector3 targetPosition = CalculateTeleportPosition();
        transform.position = targetPosition;

        // Face player
        if (playerTransform.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private Vector3 CalculateTeleportPosition()
    {
        // Determine behind direction based on player facing
        float direction = (playerTransform.localScale.x > 0) ? -1f : 1f;
        Vector3 behindPosition = playerTransform.position + new Vector3(direction * teleportDistanceBehind, 0.7f, 0);

        // Check if within bounds, otherwise teleport in front
        if (IsWithinBounds(behindPosition))
        {
            return behindPosition;
        }
        else
        {
            direction *= -1;
            Vector3 frontPosition = playerTransform.position + new Vector3(direction * teleportDistanceBehind, 0.7f, 0);
            return ClampToBounds(frontPosition);
        }
    }

    private bool IsWithinBounds(Vector3 position)
    {
        if (tilemapBoundsMin == null || tilemapBoundsMax == null)
            return true;

        return position.x >= tilemapBoundsMin.position.x &&
               position.x <= tilemapBoundsMax.position.x;
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        if (tilemapBoundsMin == null || tilemapBoundsMax == null)
            return position;

        float clampedX = Mathf.Clamp(position.x, tilemapBoundsMin.position.x, tilemapBoundsMax.position.x);
        return new Vector3(clampedX, position.y, position.z);
    }

    // Called by BossAttack when teleport sequence ends
    public void OnTeleportSequenceEnded()
    {
        StartCoroutine(ReturnToSpawnAndStun());
    }

    private IEnumerator ReturnToSpawnAndStun()
    {
        // Return to spawn position
        transform.position = spawnPosition;

        // Enter stunned state
        currentState = BossState.Stunned;

        bossHealth?.SetImmune(false); // Become vulnerable

        yield return null;
    }

    // Called by BossHealth when stunned
    public void OnStunned()
    {
        currentState = BossState.Stunned;
    }

    // Called by BossHealth when stun ends
    public void OnStunEnded()
    {
        currentState = BossState.Idle;
        idleTimer = 0f;
        wasAttackedDuringIdle = false;
    }

    // Called externally when boss is attacked during idle
    public void OnAttackedDuringIdle()
    {
        if (currentState == BossState.Idle)
        {
            wasAttackedDuringIdle = true;
        }
    }

    private void SetMoving(bool moving)
    {
        anim?.SetBool("isMoving", moving);
    }
}