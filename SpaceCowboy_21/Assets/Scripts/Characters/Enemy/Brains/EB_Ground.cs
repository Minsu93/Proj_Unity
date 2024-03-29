using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class EB_Ground : EnemyBrain
{
    ///지상 유닛. 원래 사용하던 유닛은 여기에 들어간다. 
    ///

    public override void BrainStateChange()
    {
        if (action.onAir) return;

        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Chase:
                if (!inDetectRange) ChangeState(EnemyState.Sleep, 0f);
                if (isVisible) ChangeState(EnemyState.Attack, 0);
                break;

            case EnemyState.Attack:
                if (!inDetectRange) ChangeState(EnemyState.Sleep, 0f);
                if (!isVisible) ChangeState(EnemyState.Chase, 0f);
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
