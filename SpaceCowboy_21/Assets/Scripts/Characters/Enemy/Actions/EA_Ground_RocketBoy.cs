using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_RocketBoy : EA_Ground
{
    public override void BrainStateChange()
    {
        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Idle:
                //플레이어가 죽지 않았다면, 바로 추격한다.
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
