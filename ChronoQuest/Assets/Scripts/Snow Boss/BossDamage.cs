using UnityEngine;

public class BossDamage : WizardDamage
{
    private BossHealth bossHealth;
    private BossMovement bossMovement;

    void Start()
    {
        bossHealth = GetComponent<BossHealth>();
        bossMovement = GetComponent<BossMovement>();
    }

    private new void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Here");
        // Check immunity
        if (bossHealth != null && bossHealth.IsImmune())
        {
            Debug.Log("No No Damage");
            // If attacked during idle, trigger teleport
            bossMovement?.OnAttackedDuringIdle();

            return;
        }

        // Normal damage behavior when vulnerable
        base.OnCollisionEnter2D(collision);
    }
}