using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_Airminer : EnemyBrain
{
    public override void DetectSiutation()
    {
        //�׽� ����
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

        //�������� ����
        if (health.AnyDamage(dmg))
        {
            //�´� ȿ�� 
            //action.HitView();

            if (health.IsDead())
            {
                //StopAllCoroutines();
                //���� ��� 
                enemyState = EnemyState.Die;
                gameObject.SetActive(false);
            }
        }
    }
}
