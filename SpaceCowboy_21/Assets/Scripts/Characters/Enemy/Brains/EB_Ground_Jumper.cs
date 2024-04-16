using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ground_Jumper : EB_Ground
{
    public override void BrainStateChange()
    {
        if (action.onAir) return;

        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Chase:
                //적이 보이고, 적이 공격 거리에 왔을 때, 같은 행성에 있을 때 .
                //if (!inDetectRange) ChangeState(EnemyState.Sleep, 0f);

                if (inAttackRange && !inOtherPlanet) ChangeState(EnemyState.Attack, 0);
                
                break;

            case EnemyState.Attack:

                //if (!inDetectRange) ChangeState(EnemyState.Sleep, 0f);
                if (!inAttackRange || inOtherPlanet) ChangeState(EnemyState.Chase, 0f);
                break;

        }

    }
}
