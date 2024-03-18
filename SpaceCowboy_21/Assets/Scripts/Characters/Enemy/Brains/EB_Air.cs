using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Air : EnemyBrain
{

    public override void DetectSiutation()
    {
        //항시 적용
        TotalRangeCheck();

        //상태에 따른 활동 
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
        //잠시 대기 후
        ChangeState(EnemyState.Chase, 0f);
    }

    public override void DamageEvent(float dmg)
    {
        if (enemyState == EnemyState.Die)
            return;

        //데미지를 적용
        if (health.AnyDamage(dmg))
        {
            //맞는 효과 
            action.HitView();

            if (health.IsDead())
            {
                StopAllCoroutines();
                //죽은 경우 
                ChangeState(EnemyState.Die, 0f);
            }
        }
    }
}
