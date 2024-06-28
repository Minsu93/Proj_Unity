using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Air : EnemyAction
{
    /// <summary>
    /// 공중 유닛은 어떠한 행동도 하지 않고, 플레이어가 다가왔을 때 공격한다. 
    /// 넉백 상황만 조금 고려하면 좋을 듯. 
    /// </summary>

    public override void BrainStateChange()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                //플레이어가 죽지 않았다면, 바로 추격한다.
                if (GameManager.Instance.playerIsAlive)
                    enemyState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (brain.inAttackRange && brain.isVisible)
                    enemyState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                if (!brain.inAttackRange || !brain.isVisible)
                    enemyState = EnemyState.Chase;

                break;
        }
    }


    protected override void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                onChase = false;
                onAttack = false;
                StartIdleView();
                break;

            case EnemyState.Chase:
                onChase = true;
                onAttack = false;
                break;

            //추가한 부분 : 공격 시 이동
            case EnemyState.Attack:
                onChase = false;
                onAttack = true;
                break;

        }
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        EnemyChase_Air chase_Air = chase as EnemyChase_Air;
        if(chase_Air != null)
        {
            chase_Air.StopSwim();
        }
        
    }

    protected override IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        Vector2 targetPos = strikePos;
        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHeight, dir.normalized, dir.magnitude, LayerMask.GetMask("Planet"));
        if (hit.collider != null)
        {
            targetPos = hit.point;
        }

        Vector2 strikeStartPos = transform.position;
        float dist = (targetPos - strikeStartPos).magnitude;
        float strikeTime = dist / strikeSpeed;
        float time = 0;

        while (time < strikeTime)
        {
            time += Time.deltaTime;
            transform.position = (Vector2.Lerp(strikeStartPos, targetPos, time / strikeTime));

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        WakeUpEvent();
    }

    public override void WakeUpEvent()
    {
        enemyState = EnemyState.Chase;
        activate = true;
    }
}
