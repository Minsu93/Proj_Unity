using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class EB_Ground : EnemyBrain
{
    ///���� ����. ���� ����ϴ� ������ ���⿡ ����. 
    ///

    public override void BrainStateChange()
    {
        if (action.onAir) return;

        //���¿� ���� Ȱ�� 
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
