using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Air : EnemyAction
{
    /// <summary>
    /// 공중 유닛은 어떠한 행동도 하지 않고, 플레이어가 다가왔을 때 공격한다. 
    /// 넉백 상황만 조금 고려하면 좋을 듯. 
    /// </summary>
    [Header("KnockBack on Air")] 
    [SerializeField] AnimationCurve knockBackCurve;
    [SerializeField] float knockbackTime = 1f;   //날아가는 시간 
    [SerializeField] float stopDuration = 1f;
    EnemyChase_Air chase_Air;
    protected override void Awake()
    {
        base.Awake();
        chase_Air = chase as EnemyChase_Air;
    }

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


    protected override void ActionByState(EnemyState state)
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
        //strike 중이면 정지한다. 
        StopAllCoroutines();
        //EnemyPause(knockbackTime + 0.3f);
        onWait = true;

        //chase 중지
        if (chase_Air != null)
        {
            chase_Air.StopSwim();
        }
        //attack 중지
        attack.StopAttackAction();

        //넉백
        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);

        StartCoroutine(StopRoutine(knockbackTime + 0.3f));

        //Vector2 dir = (Vector2)transform.position - hitPos;
        //dir = dir.normalized;
        //Vector2 startPos = (Vector2)transform.position;
        //Vector2 targetPos = (Vector2)transform.position + (dir * forceAmount);
        //StartCoroutine(KnockBackRoutine(startPos, targetPos));
    }

    IEnumerator StopRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        float timer = 0;
        Vector2 vel = rb.velocity;

        //속도를 늦춰서 정지합니다.
        while (timer < stopDuration)
        {
            timer += Time.deltaTime;
            rb.velocity = Vector2.Lerp(vel, Vector2.zero, knockBackCurve.Evaluate(timer));
            yield return null;
        }

        //다시 움직임을 활성화 합니다. 
        onWait = false;
    }

    //IEnumerator KnockBackRoutine(Vector2 start, Vector2 end)
    //{
    //    float time = 0;
    //    while (time < 1)
    //    {
    //        time += Time.deltaTime / knockbackTime;
    //        rb.MovePosition(Vector2.Lerp(start, end, knockBackCurve.Evaluate(time)));
    //        yield return null;
    //    }
    //}

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

        AfterStrikeEvent();
    }

    public override void AfterStrikeEvent()
    {
        enemyState = EnemyState.Chase;
        activate = true;
    }
}
