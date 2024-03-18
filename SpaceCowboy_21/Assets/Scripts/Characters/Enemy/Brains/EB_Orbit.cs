using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Orbit : EnemyBrain
{
    /// �˵� ����. �˵��� ������ �ӵ��� ����, �÷��̾� �������� ����ü�� ��ų� �߰� ���� ������ ��ȯ�Ѵ�. 

    bool inAttackOnce;   //ĳ���� ȸ����.
    float prePlayerDistance;
    Vector2 observedPos;

    EA_Orbit orbitAction;

    public override void Initialize()
    {
        orbitAction = (EA_Orbit)action;
    }
    public override void DetectSiutation()
    {
        //�׽� ����
        TotalRangeCheck();

        //���¿� ���� Ȱ�� 
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


    //�÷��̾������ �Ÿ��� ���ؼ� �־����� ������ ������ �ٲٰ�, ��������� ������ ������ �����Ѵ�. 
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
            //�ݴ� �������� ����
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
        //��� ��� ��
        enemyState = EnemyState.Chase;
    }
}
