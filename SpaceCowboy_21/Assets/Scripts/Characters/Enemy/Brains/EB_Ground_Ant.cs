using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ground_Ant : EnemyBrain
{
    public override void BrainStateChange()
    {
        if (action.onAir) return;

        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Sleep:

                if (inDetectRange)
                {
                    ChangeState(EnemyState.Chase, 0f);
                }

                break;

            case EnemyState.Chase:
            case EnemyState.Wait:

                if (inDetectRange) 
                {
                    //T
                    if (OnOtherPlanetCheck()) ChangeState(EnemyState.Chase, 0);
                    else ChangeState(EnemyState.Wait, 0);
                }

                break;

        }

    }

}
