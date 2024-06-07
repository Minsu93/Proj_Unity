using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class EA_Orbit : EnemyAction
{
    /// <summary>
    /// 스트라이크 모드일때 행성을 그냥 지정해버린다. 
    /// 장애물이 있으면 방향을 바꾼다. 
    /// </summary>
    /// 

    [Header("Orbit")]
    public float pauseTimer = 0.1f; //피격 시 정지 시간

    //변수
    float pTime;

    //파티클
    public ParticleSystem boosterParticle;

    EnemyChase_Orbit chase_Orbit;



    protected override void Awake()
    {
        base.Awake();
        chase_Orbit = GetComponent<EnemyChase_Orbit>();
    }

    protected override bool BeforeUpdate()
    {
        if (!activate) return true;

        //스턴 시간 추가
        if (pTime > 0)
        {
            pTime -= Time.deltaTime;
            return true;
        }

        return false;
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
                StartRunView();
                onChase = true;
                onAttack = false;
                break;

                //추가한 부분 : 공격 시 이동
            case EnemyState.Attack:
                onChase = true;
                onAttack = true;
                break;

        }
    }

    
    public override void WakeUpEvent()
    {
        enemyState = EnemyState.Chase;
        activate = true;

        //중력 활성화 제거
        //gravity.activate = true;

        //방향 지정 추가
        ChangeDirection(brain.playerIsRight);

        //파티클 조절 추가
        if (boosterParticle != null) boosterParticle.Play();

    }


    protected override void OnDieAction()
    {
        base.OnDieAction();

        //죽으면 궤도 물체 지상으로 떨어짐.
        gravity.activate = true;

        //파티클 추가
        if (boosterParticle != null) boosterParticle.Stop();
    }


    void ChangeDirection()
    {
        faceRight = !faceRight;
        FlipToDirectionView();
        chase_Orbit.ChangeDirection(faceRight);
    }

    void ChangeDirection(bool isRight)
    {
        faceRight = isRight;
        FlipToDirectionView();
        chase_Orbit.ChangeDirection(faceRight);
    }

    //base에 피격시 정지하는 pTime 만 추가함.
    public override void DamageEvent(float damage, Vector2 hitVec)
    {
        if (enemyState == EnemyState.Die) return;

        if (health.AnyDamage(damage))
        {
            StartHitView();

            //피격시 정지 추가
            pTime = pauseTimer;

            if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

            if (health.IsDead())
            {
                WhenDieEvent();

                GameManager.Instance.playerManager.ChargeFireworkEnergy();
            }
        }
    }

    //행성 도착 시 행성 center를 가져오는 로직 추가함. 
    protected override IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHeight, dir.normalized, float.MaxValue, LayerMask.GetMask("Planet"));
        if (hit.collider == null) yield break;

        //추가한 부분
        Planet planet = hit.collider.GetComponent<Planet>();

        Vector2 strikeStartPos = transform.position;
        Vector2 strikeTargetPos = hit.point;
        float strikeTime = hit.distance / strikeSpeed;
        float time = 0; 
        float distance = hit.distance; 
        while (distance > distanceFromStrikePoint + enemyHeight)
        {
            time += Time.deltaTime;
            distance = Vector2.Distance(transform.position, strikeTargetPos);
            transform.position = (Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        //추가한 부분
        chase_Orbit.SetCenterPoint(planet);

        WakeUpEvent();
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        EnemyPause(1f);

        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;

        rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);
    }


    //행성과 부딪히면 방향 전환. 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Planet"))
        {
            ChangeDirection();
        }
    }
}
