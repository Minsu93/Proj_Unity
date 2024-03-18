using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ground : EnemyBrain
{
    ///지상 유닛. 원래 사용하던 유닛은 여기에 들어간다. 
    ///

    public override void DetectSiutation()
    {
        if (action.onAir) return;

        //항시 적용
        TotalRangeCheck();

        //상태에 따른 활동 
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

                //공격 상태 체크
                if (ThinkAboutAttack()) ChangeState(EnemyState.Attack, 0);
                //추격 
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
