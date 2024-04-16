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


    
    protected override void Update()
    {
        EnemyState currState = brain.enemyState;

        if (currState != preState)
        {
            DoAction(currState);
            preState = currState;
        }

        if (!activate) return;

        //스턴 시간 추가
        if (pTime > 0)
        {
            pTime -= Time.deltaTime;
            return;
        }

        if (onAttack)
        {
            if (attack != null)
                attack.OnAttackAction();
        }

        if (onChase)
        {
            if (chase != null) chase.OnChaseAction();

        }
    }

    protected override void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Strike:
                onChase = false;
                onAttack = false;
                StrikeAction();
                StrikeView();
                break;

            case EnemyState.Sleep:
                onChase = false;
                onAttack = false;
                OnSleepEvent();
                AddOnPlanetEnemyList();
                StartIdleView();
                break;

            case EnemyState.Chase:
                StartIdleView();
                onChase = true;
                onAttack = false;
                break;

                //공격 시 이동
            case EnemyState.Attack:
                onChase = true;
                onAttack = true;
                break;

            case EnemyState.Die:
                onChase = false;
                onAttack = false;
                OnDieAction();
                break;
        }
    }


    public override void WakeUpEvent()
    {
        activate = true;
        hitColl.enabled = true;

        //방향 지정 추가
        ChangeDirection(brain.playerIsRight);

        //파티클 추가
        if (boosterParticle != null) boosterParticle.Play();

    }


    protected override void OnDieAction()
    {
        base.OnDieAction();
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

    public override void HitView()
    {
        base.HitView();

        //경직 
        pTime = pauseTimer;
    }

    protected override IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHeight, dir.normalized, float.MaxValue, LayerMask.GetMask("Planet"));
        if (hit.collider == null) yield break;

        //Orbit 추가
        chase_Orbit.SetCenterPoint(hit.transform.GetComponent<Planet>());

        Vector2 strikeStartPos = transform.position;
        Vector2 strikeTargetPos = hit.point;
        float strikeTime = hit.distance / strikeSpeed;
        float time = 0; //강습 시간
        float distance = hit.distance; //남은 거리
        while (distance > distanceFromStrikePoint + enemyHeight)
        {
            time += Time.deltaTime;
            distance = Vector2.Distance(transform.position, strikeTargetPos);
            transform.position = (Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        brain.WakeUp();
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Planet"))
        {
            ChangeDirection();
        }
    }
}
