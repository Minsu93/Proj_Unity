using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ground_OrbitShooter : EB_Ground
{
    public override void BrainStateChange()
    {
        if (action.onAir) return;

        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            case EnemyState.Chase:

                if (inAttackRange) ChangeState(EnemyState.Attack, 0);
                break;

            case EnemyState.Attack:
                if (!inAttackRange) ChangeState(EnemyState.Chase, 0f);
                break;

        }

    }


}
