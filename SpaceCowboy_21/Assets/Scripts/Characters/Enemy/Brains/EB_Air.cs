using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Air : EnemyBrain
{

    public override void DetectSiutation()
    {
        //�׽� ����
        TotalRangeCheck();

        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            case EnemyState.Idle:

                if (inChaseRange) ChangeState(EnemyState.Chase, 0f);
                break;

            case EnemyState.Chase:

                if (ThinkAboutAttack()) ChangeState(EnemyState.Attack, 0f);
                break;
        }
    }

    public bool ThinkAboutAttack()
    {
        bool canAttack = false;

        if (inAttackRange && playerIsVisible && !action.attackCool) canAttack = true;

        return canAttack;
    }


    public override void WakeUp()
    {
        //timeBetweenChecks = 0.5f;
        //��� ��� ��
        ChangeState(EnemyState.Chase, 0f);
    }

    public override void DamageEvent(float dmg)
    {
        if (enemyState == EnemyState.Die)
            return;

        //�������� ����
        if (health.AnyDamage(dmg))
        {
            //�´� ȿ�� 
            action.HitView();

            if (health.IsDead())
            {
                StopAllCoroutines();
                //���� ��� 
                ChangeState(EnemyState.Die, 0f);
            }
        }
    }
}
