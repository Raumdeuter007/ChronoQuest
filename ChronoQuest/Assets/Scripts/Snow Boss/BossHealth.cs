using UnityEngine;
using System.Collections;

public class BossHealth : EnemyHealth
{
    [Header("Boss Stun Settings")]
    public int hitsToStun = 3;
    public float stunDuration = 3f;

    private int currentHitCount = 0;
    private bool isImmune = true; // Start immune
    private bool isStunned = false;
    private BossMovement bossMovement;

    new void Start()
    {
        base.Start();
        bossMovement = GetComponent<BossMovement>();
    }

    public new void TakeDamage(int damage)
    {
        // Ignore damage if immune or stunned
        if (isImmune || isStunned)
            return;

        currentHitCount++;
        base.TakeDamage(damage);

        if (currentHitCount >= hitsToStun)
        {
            EnterStunState();
        }
    }

    private void EnterStunState()
    {
        isStunned = true;
        isImmune = true;
        currentHitCount = 0;

        if (bossMovement != null)
            bossMovement.OnStunned();

        StartCoroutine(StunTimer());
    }

    private IEnumerator StunTimer()
    {
        yield return new WaitForSeconds(stunDuration);
        ExitStunState();
    }

    public void ExitStunState()
    {
        isStunned = false;
        isImmune = true; // Become immune again for next idle phase

        if (bossMovement != null)
            bossMovement.OnStunEnded();
    }

    public void SetImmune(bool immune)
    {
        isImmune = immune;
    }

    public bool IsImmune()
    {
        return isImmune;
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}