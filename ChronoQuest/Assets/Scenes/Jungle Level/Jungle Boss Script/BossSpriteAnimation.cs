using UnityEngine;

public class BossSpriteAnimation: MonoBehaviour
{
    private JungleBossAttack enemyAttack;

    void Awake()
    {
        // Assuming EnemyAttack is on parent
        enemyAttack = GetComponentInParent<JungleBossAttack>();
    }

    // This function can now be called by Animation Event
    public void AttackHit()
    {
        if (enemyAttack != null)
            enemyAttack.ApplyAttackHit();
    }
}
