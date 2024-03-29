using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Air : EnemyBrain
{

    public override void BrainStateChange()
    {
        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Sleep:

                if (inDetectRange) ChangeState(EnemyState.Chase, 0f);
                break;

            case EnemyState.Chase:

                if ((inAttackRange && isVisible && !action.attackCool)) ChangeState(EnemyState.Attack, 0f);
                break;
        }
    }

    //public bool ThinkAboutAttack()
    //{
    //    bool canAttack = false;

    //    if (inAttackRange && isVisible && !action.attackCool) canAttack = true;

    //    return canAttack;
    //}



    //public override void DamageEvent(float dmg)
    //{
    //    if (enemyState == EnemyState.Die)
    //        return;

    //    //데미지를 적용
    //    if (health.AnyDamage(dmg))
    //    {
    //        //맞는 효과 
    //        action.HitView();

    //        if (health.IsDead())
    //        {
    //            StopAllCoroutines();
    //            //죽은 경우 
    //            ChangeState(EnemyState.Die, 0f);
    //        }
    //    }
    //}

    protected override void AfterHitEvent() 
    {
        return;
    }

    protected override void WhenDieEvent()
    {
        return;
    }
}
