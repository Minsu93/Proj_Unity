using System.Collections;
using System.Threading;
using UnityEngine;

public class EA_Orbit : EnemyAction
{
    /// <summary>
    /// 스트라이크 모드일때 행성을 그냥 지정해버린다. 
    /// 장애물이 있으면 방향을 바꾼다. 
    /// </summary>
    /// 

    [Header("Orbit")]
    [SerializeField] float pauseTimer = 0.1f; //피격 시 정지 시간

    [Header("KnockBack_Orbit")]
    [SerializeField] AnimationCurve knockBackCurve;
    [SerializeField] float knockbackTime = 1f;   //날아가는 시간 
    [SerializeField] float stopDuration = 1f;

    //변수
    float pTime;

    //파티클
    [SerializeField] ParticleSystem boosterParticle;
    EnemyChase_Orbit chase_Orbit;


    //override 한 부분

    protected override void Awake()
    {
        base.Awake();
        chase_Orbit = chase as EnemyChase_Orbit;
    }

   
    protected override void Update()
    {
        //Enemy가 죽거나 Strike가 끝나기 전에는 Update하지 않는다. 
        if (BeforeUpdate()) return;

        //브레인에서 플레이어 관련 변수 업데이트
        brain.TotalCheck();

        //업데이트에 따라 enemyState 변경. 
        BrainStateChange();

        //enemyState에 따른 Action 실행 
        DoAction();

        ///수정된 부분
        if (enemyState == EnemyState.Wait) return;

        if (onAttack)
        {
            if (attack != null)
                attack.OnAttackAction();
        }

        if (gravity.nearestPlanet == null) return;

        if (onChase)
        {
            if (chase_Orbit != null)
            {
                chase_Orbit.OnChaseAction();
            }
        }
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

    protected override void ActionByState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                StartIdleView();
                onChase = false;
                onAttack = false;
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

    
    //base에 피격시 정지하는 pTime 만 추가함.
    public override void DamageEvent(float damage, Vector2 hitVec)
    {
        if (enemyState == EnemyState.Die) return;


        if (health.AnyDamage(damage))
        {

            //피격시 정지 추가
            pTime = pauseTimer;


            if (health.IsDead())
            {
                WhenDieEvent();
            }
            else
            {
                StartHitView();
                if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

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

        AfterStrikeEvent();
    }

    public override void AfterStrikeEvent()
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

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        EnemyPause(knockbackTime + 0.3f);

        chase_Orbit.ResetCenterPoint();

        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);

        StartCoroutine(StopRoutine(knockbackTime + 0.3f));
    }

    IEnumerator StopRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Planet nearestPlanet = gravity.nearestPlanet;
        while(nearestPlanet == null)
        {
            yield return null;
        }
        

        float timer = 0;
        Vector2 vel = rb.velocity;

        //속도를 늦춰서 정지합니다. 시간은 1초.
        float secToStop = 1.0f;
        while(timer < secToStop)
        {
            timer += Time.deltaTime;
            rb.velocity = Vector2.Lerp(vel, Vector2.zero, knockBackCurve.Evaluate(timer));
            yield return null;
        }

        if(nearestPlanet != null)
        {
            chase_Orbit.SetCenterPoint(nearestPlanet);
        }
    }

    public override void WhenDieEvent()
    {
        base.WhenDieEvent();

        //죽으면 궤도 물체 지상으로 떨어짐.
        //gravity.activate = true;

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


    //행성과 부딪히면 방향 전환. 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Planet"))
        {
            ChangeDirection();
        }
    }
}
