using UnityEngine;

public class WizardAnimationEvents : MonoBehaviour
{
    private EnemyAttack enemyAttack;

    void Awake()
    {
        // Assuming EnemyAttack is on parent
        enemyAttack = GetComponentInParent<EnemyAttack>();
    }

    // This function can now be called by Animation Event
    public void AttackHit()
    {
        if (enemyAttack != null)
            enemyAttack.ApplyAttackHit();
    }
}
