using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ground_Jumper : EB_Ground
{
    public override void BrainStateChange()
    {
        if (action.onAir) return;

        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            case EnemyState.Chase:
                //���� ���̰�, ���� ���� �Ÿ��� ���� ��, ���� �༺�� ���� �� .
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
