using UnityEngine;
using System.Collections;
using System;
public class SnowBossHealth : EnemyHealth
{
    [Header("Boss Stun Settings")]
    public int hitsToStun = 3;
    public float stunDuration = 3f;

    private int currentHitCount = 0;
    private bool isImmune = true; // Start immune
    private bool isStunned = false;
    private SnowBossMovement bossMovement;

    new void Start()
    {
        base.Start();
        bossMovement = GetComponent<SnowBossMovement>();
    }

    public override void TakeDamage(int damage)
    {
        Debug.Log("here");
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

    protected override IEnumerator Death()
    {
        yield return new WaitForSeconds(deathTime);
        GameObject foundObject = GameObject.Find("Finish Level");
        LevelFinish level = foundObject?.GetComponent<LevelFinish>();
        level?.Finish();
    }
}