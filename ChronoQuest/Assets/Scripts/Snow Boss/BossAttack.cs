using UnityEngine;
using System.Collections;

public class BossAttack : EnemyAttack
{
    [Header("Boss Teleport Attack Settings")]
    public int attacksPerSequence = 3;
    public float vanishDuration = 0.5f; // Time between vanish and reappear
    public float attackDelay = 0.3f; // Delay after appearing before attack
    public float timeBetweenTeleports = 0.5f; // Time after attack before next vanish

    private int currentAttackCount = 0;
    private bool isInTeleportSequence = false;
    private BossMovement bossMovement;

    new void Start()
    {
        base.Start();
        bossMovement = GetComponent<BossMovement>();
    }

    public void StartTeleportAttackSequence()
    {
        if (isInTeleportSequence)
            return;

        StartCoroutine(ExecuteTeleportAttackSequence());
    }

    private IEnumerator ExecuteTeleportAttackSequence()
    {
        isInTeleportSequence = true;
        currentAttackCount = 0;

        while (currentAttackCount < attacksPerSequence)
        {
            // 1. Vanish (trigger Start_tel animation)
            animator?.SetTrigger("Start_tel");

            yield return new WaitForSeconds(vanishDuration);

            // 2. Teleport behind player
            bossMovement?.TeleportBehindPlayer();

            // 3. Appear (trigger End_tel animation)
            animator?.SetTrigger("End_tel");

            yield return new WaitForSeconds(attackDelay);

            // 4. Attack
            PerformAttack();
            currentAttackCount++;

            // 5. Wait before next teleport (if not last attack)
            if (currentAttackCount < attacksPerSequence)
            {
                yield return new WaitForSeconds(timeBetweenTeleports);
            }
        }

        // Sequence complete
        isInTeleportSequence = false;

        if (bossMovement != null)
            bossMovement.OnTeleportSequenceEnded();
    }

    private void PerformAttack()
    {
        // Detect player in range
        playerHit = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayer);

        if (playerHit != null && animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public bool IsInTeleportSequence()
    {
        return isInTeleportSequence;
    }
}