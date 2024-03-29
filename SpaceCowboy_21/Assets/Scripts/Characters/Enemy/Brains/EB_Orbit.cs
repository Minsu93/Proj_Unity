using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Orbit : EnemyBrain
{
    /// 궤도 유닛. 궤도를 동일한 속도로 돌며, 플레이어 방향으로 투사체를 쏘거나 추격 공중 유닛을 소환한다. 

    bool inAttackOnce;   //캐릭터 회전용.
    float prePlayerDistance;
    Vector2 observedPos;

    EA_Orbit orbitAction;

    protected override void Awake()
    {
        base.Awake();
        orbitAction = (EA_Orbit)action;
    }

    public override void BrainStateChange()
    {
        //상태에 따른 활동 
        switch (enemyState)
        {
            //case EnemyState.Sleep:
            //    if (inDetectRange)
            //    {
            //        WakeUp();
            //    }
            //    break;

            case EnemyState.Chase:
                if (inAttackRange && isVisible)
                {
                    ChangeState(EnemyState.Attack, 0);
                }
                
                if (!inAttackRange && !isVisible)
                {
                    //플레이어 방향을 감지하고 회전하기
                    if(playerIsRight)
                    {
                        orbitAction.ChangeDirectionToRight(true);
                    }
                    else
                    {
                        orbitAction.ChangeDirectionToRight(false);
                    }
                }

                break;

            case EnemyState.Attack:

                if (!inAttackRange || !isVisible) 
                {
                    ChangeState(EnemyState.Chase, 0);
                }

                break;
        }
    }

    protected override void AfterHitEvent()
    {
        return;
    }

    protected override void WhenDieEvent()
    {
        return;
    }
}
