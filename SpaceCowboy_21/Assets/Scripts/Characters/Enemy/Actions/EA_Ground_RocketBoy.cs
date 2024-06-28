using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_RocketBoy : EA_Ground
{
    public override void BrainStateChange()
    {
        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            case EnemyState.Idle:
                //�÷��̾ ���� �ʾҴٸ�, �ٷ� �߰��Ѵ�.
                if (GameManager.Instance.playerIsAlive)
                    enemyState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (brain.inAttackRange)
                    enemyState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                if (!brain.inAttackRange)
                    enemyState = EnemyState.Chase;

                break;
        }
    }
}
