using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_Airminer : EnemyBrain
{
    public override void DetectSiutation()
    {
        //항시 적용
        TotalRangeCheck();

        switch (enemyState)
        {
            case EnemyState.Idle:

                if (inChaseRange)
                {
                    ChangeState(EnemyState.Chase, 0f);
                }

                break;


            case EnemyState.Chase:

                if (inAttackRange)
                {
                    ChangeState(EnemyState.Attack, 0);
                }

                break;

            case EnemyState.Attack:

                if (!inAttackRange)
                {
                    ChangeState(EnemyState.Chase, 0);
                }

                break;
        }
    }

    public override void DamageEvent(float dmg)
    {
        if (enemyState == EnemyState.Die)
            return;

        //데미지를 적용
        if (health.AnyDamage(dmg))
        {
            //맞는 효과 
            //action.HitView();

            if (health.IsDead())
            {
                //StopAllCoroutines();
                //죽은 경우 
                enemyState = EnemyState.Die;
                gameObject.SetActive(false);
            }
        }
    }
}
