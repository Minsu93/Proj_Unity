using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_Airminer : EnemyBrain
{
    public override void BrainStateChange()
    {
        switch (enemyState)
        {
            case EnemyState.Sleep:

                if (inAttackRange)
                {
                    ChangeState(EnemyState.Attack, 0);
                }

                break;


            case EnemyState.Attack:

                if (!inAttackRange)
                {
                    ChangeState(EnemyState.Sleep, 0);
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
