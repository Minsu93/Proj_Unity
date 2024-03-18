using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ground : EnemyBrain
{
    ///���� ����. ���� ����ϴ� ������ ���⿡ ����. 
    ///

    public override void DetectSiutation()
    {
        if (action.onAir) return;

        //�׽� ����
        TotalRangeCheck();

        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            case EnemyState.Idle:

                if (inChaseRange)
                {
                    ChangeState(EnemyState.Chase,0f);
                }

                break;

            case EnemyState.Chase:
            case EnemyState.ToJump:
            case EnemyState.Wait:

                //���� ���� üũ
                if (ThinkAboutAttack()) ChangeState(EnemyState.Attack, 0);
                //�߰� 
                else
                {
                    if (inChaseRange)
                    {
                        //T
                        if (OnSamePlanetCheck()) ChangeState(EnemyState.Chase, 0);
                        else
                        {
                            if (FindAttackablePoint()) ChangeState(EnemyState.Chase, 0);

                            else ChangeState(EnemyState.ToJump, 0);
                        }
                    }
                    else ChangeState(EnemyState.Wait, 0);
                }

                break;

        }

    }

    public bool ThinkAboutAttack()
    {
        bool canAttack = false;

        if (inAttackRange && playerIsVisible && !action.attackCool) canAttack = true;

        return canAttack;
    }

}
