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

    public override void Initialize()
    {
        orbitAction = (EA_Orbit)action;
    }
    public override void DetectSiutation()
    {
        //항시 적용
        TotalRangeCheck();

        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Idle:

                if (inChaseRange)
                {
                    ChangeState(EnemyState.Chase, 0f);
                }

                break;


            case EnemyState.Chase:

                if (inAttackRange) action.AimStart();
                else action.AimStop();

                if (inAttackRange)
                {
                    inAttackOnce = true;
                    ChangeState(EnemyState.Attack, 0);
                }

                if ( inAttackOnce && NeedTurnCheck())
                {
                    orbitAction.ChangeDirection();
                    inAttackOnce = false;
                }


                break;

            case EnemyState.Attack:

                if (!inAttackRange)
                {
                    observedPos = playerTr.position;
                    prePlayerDistance = (observedPos - (Vector2)transform.position).magnitude;
                    ChangeState(EnemyState.Chase, 0);
                }

                break;
        }
    }


    //플레이어까지의 거리를 비교해서 멀어지고 있으면 방향을 바꾸고, 가까워지고 있으면 방향을 유지한다. 
    bool NeedTurnCheck() 
    {
        bool needTurn = false;

        float currPlayerDistance = (observedPos - (Vector2)transform.position).magnitude;
        if (currPlayerDistance > prePlayerDistance)
        {
            needTurn = true;
        }

        return needTurn;
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            //orbitAction.IsInsidePlanet();
            orbitAction.ChangeDirection();
        }
        else if (collision.CompareTag("SpaceBorder"))
        {
            //반대 방향으로 돌기
            orbitAction.ChangeDirection();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.CompareTag("Planet"))
        //{
        //    orbitAction.IsOutsidePlanet();
        //}
    }


    public override void WakeUp()
    {
        timeBetweenChecks = 0.5f;
        //잠시 대기 후
        enemyState = EnemyState.Chase;
    }
}
