using SpaceCowboy;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using static UnityEditor.Progress;
using System;


public abstract class EnemyAction : MonoBehaviour, IHitable , ITarget
{
    /// <summary>
    /// 적에 따라 다른 행동을 한다. EnemyAction은 Ground, Orbit, Ship 모두에 따라 다른 행동을 하도록 만들어져 있다. 이들의 공통점은 Brain 의 EnemyState를 매턴 받아와, State가 바뀔 때 그에 맞는 행동을 하도록 바뀌는 것이다. 
    /// Brain의 State가 느리게 변하기 때문에, Action에서는 행동의 변화가 천천히 일어날 것으로 기대된다. 
    /// 각 유닛마다 다른 Action을 하겠지만, 큰 틀에서는 비슷하기 때문에 동일한 Action을 상속받아 사용하더라도 문제가 없도록 만들려고 한다. 
    /// </summary>
    public bool activate;
    public EnemyType enemyType = EnemyType.Ground;     //적의 타입.WaveManager에서 받아와 유닛 별 다른 스폰방식을 적용한다. 

    public EnemyState enemyState = EnemyState.Strike;


    [Header("Move Property")]
    public float enemyHeight = 0.51f;
    public float turnSpeedOnLand = 100f;

    [Header("BodyAttack")]
    public float bodyDamage = 3.0f;

    [Header("Strike Property")]
    public float strikeSpeed = 10f;     //강습 속도
    public float distanceFromStrikePoint = 0f;      //정지 거리 

    [Header("Clear Property")]
    [SerializeField] float clearMoveDistance = 3.0f;
    [SerializeField] float clearMoveTime = 3.0f;
    [SerializeField] AnimationCurve clearCurve;

    //이펙트
    [Header("VFX")]
    [SerializeField] protected ParticleSystem hitEffect;    //맞았을 때 효과
    [SerializeField] ParticleSystem deadEffect;   //죽었을 때 효과

    //로컬 변수
    protected bool onAttack;  //공격중일 때 
    protected bool onChase = false;   //chase 중인가요
    protected bool startChase;        //chase 시작 시 한번만 실행.

    protected EnemyState preState = EnemyState.Strike;
    public bool faceRight { get; set; }  //캐릭터가 오른쪽을 보고 있습니까? 
    public bool onAir { get; set; } //공중에 있는가


    //스크립트
    protected EnemyBrain brain;
    protected EnemyAttack attack;
    protected EnemyChase chase;
    protected CharacterGravity gravity;
    protected Rigidbody2D rb;
    protected Collider2D hitColl;
    public GameObject iconUI;
    protected Planet prePlanet;
    protected Health health;




    //이벤트
    public event System.Action EnemyStartIdle;
    public event System.Action EnemyStartRun;
    public event System.Action EnemyStrikeEvent;
    public event System.Action EnemyAttackEvent;
    public event System.Action EnemyHitEvent;
    public event System.Action EnemyDieEvent;
    public event System.Action EnemyAimOnEvent;
    public event System.Action EnemyAimOffEvent;
    public event System.Action EnemyResetEvent;
    public event System.Action EnemyClearEvent;
    public event System.Action EnemySeeDirection;





    protected virtual void Awake()
    {
        gravity = GetComponent<CharacterGravity>();
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();
        chase = GetComponent<EnemyChase>();
        hitColl = GetComponent<Collider2D>();
        health = GetComponent<Health>();

    }

    private void Start()
    {
        WaveManager.instance.MonsterDisappearEvent += WaveClearEvent;
        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //플레이어가 죽은 경우 실행하는 이벤트 
    }

    protected virtual void OnEnable()
    {
        iconUI.SetActive(true);
    }
    private void OnDisable()
    {
        iconUI.SetActive(false);
    }


    protected virtual void Update()
    {
        //Enemy가 죽거나 Strike가 끝나기 전에는 Update하지 않는다. 
        if (BeforeUpdate()) return;

        //브레인에서 플레이어 관련 변수 업데이트
        brain.TotalCheck();

        //업데이트에 따라 enemyState 변경. 
        BrainStateChange();

        //enemyState에 따른 Action 실행 
        if (enemyState != preState)
        {
            DoAction(enemyState);
            preState = enemyState;
        }

        //pause 상태.
        if (enemyState == EnemyState.Wait) return;

        if (onAttack)
        {
            if(attack != null)
                attack.OnAttackAction();
        }

        if (onChase)
        {
            if(chase != null) 
                chase.OnChaseAction();
        }
    }

    // 업데이트하기 전 조건이다. true면 업데이트를 수행하지 않는다. 
    protected virtual bool BeforeUpdate()
    {
        if (!activate) return true;

        return false;
    }
        
    // 유닛 종류에 따라 다르게 상황을 감지한다. 
    public abstract void BrainStateChange();

    //상태가 바뀔 때 한번만 실행 
    protected virtual void DoAction(EnemyState state)
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

            case EnemyState.Attack:
                StartIdleView();
                onChase = false;
                onAttack = true;
                break;

        }
    }


    #region Strike Mode
    //지상 및 궤도 타입의 Strike 방식
    public void EnemyStartStrike(Vector2 strikePos)
    {
        ResetAction();

        enemyState = EnemyState.Strike;
        StartStrikeView();

        //캐릭터를 회전한다. 
        Vector2 rotateVec = (Vector2)transform.position - strikePos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateVec.normalized);

        //강습을 시작한다. 
        StartCoroutine(StrikeRoutine(strikePos));
    }

    protected abstract IEnumerator StrikeRoutine(Vector2 strikePos);

    #endregion

    #region Wave Clear
    //스폰된 모든 몬스터 사라지는 이벤트
    protected virtual void WaveClearEvent()
    {
        if (!activate) return;
        activate = false;

        StopAllCoroutines();
        attack.StopAttackAction();
        iconUI.SetActive(false);
        hitColl.enabled = false;

        //이동완료 시 투명화.
        StartClearView();

        StartCoroutine(ClearRoutine());
        StartCoroutine(DieRoutine(3.0f));
    }

    IEnumerator ClearRoutine()
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = transform.position + (transform.up * clearMoveDistance);
        float time = 0;
        while(time < 1)
        {
            time += Time.deltaTime / clearMoveTime;
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, clearCurve.Evaluate(time)));
            yield return null;
        }
    }
    #endregion

    #region Basic Actions
    //공격 이후 적유닛 행동
    public virtual void AfterAttack()
    {
        EnemyPause(attack.attackCoolTime);
    }

    //플레이어가 죽은 경우 다시 잠든다. 
    public void PlayerIsDead()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Idle;
    }

    //초기화
    public void ResetAction()
    {
        onChase = false;
        onAttack = false;
        
        hitColl.enabled = true;

        preState = EnemyState.Strike;

        health.ResetHealth();
        StartResetView();
    }

    //Strike 끝나고 깨어나기. 
    public virtual void WakeUpEvent()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Chase;
        activate = true;
        gravity.activate = true;
    }

    //유닛 정지
    public void EnemyPause(float second)
    {
        StartCoroutine(PauseRoutine(second));
    }
    IEnumerator PauseRoutine(float sec)
    {
        //지난 행동상태
        EnemyState preState = enemyState;
        enemyState = EnemyState.Wait;

        yield return new WaitForSeconds(sec);
        enemyState = preState;
    }

    protected virtual void OnDieAction()
    {
        activate = false;

        StopAllCoroutines();
        attack.StopAttackAction();
        StartDieView();

        iconUI.SetActive(false);
        hitColl.enabled = false;

        StartCoroutine(DieRoutine(3.0f));
    }

    protected IEnumerator DieRoutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        gravity.activate = false;
        gameObject.SetActive(false);
    }

    #endregion

    #region Collide with Player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (bodyDamage == 0) return;
            if (other.TryGetComponent<PlayerBehavior>(out PlayerBehavior pb))
            {
                pb.DamageEvent(bodyDamage, transform.position);
            }
        }
    }

    #endregion

    #region Hit and Damage
    public virtual void DamageEvent(float damage, Vector2 hitVec)
    {
        if (enemyState == EnemyState.Die) return;

        if (health.AnyDamage(damage))
        {
            //맞는 효과 
            StartHitView();
            if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

            if (health.IsDead())
            {
                WhenDieEvent();

                //GameManager.Instance.playerManager.ChargeFireworkEnergy();
            }
        }
    }

    public virtual void WhenDieEvent()
    {
        enemyState = EnemyState.Die;

        activate = false;
        onChase = false;
        onAttack = false;

        StopAllCoroutines();
        attack.StopAttackAction();
        StartDieView();

        iconUI.SetActive(false);
        hitColl.enabled = false;

        StartCoroutine(DieRoutine(3.0f));

        //WaveManager에 전달.
        if (WaveManager.instance != null)
            WaveManager.instance.CountEnemyLeft(this.gameObject);

        if (deadEffect != null) GameManager.Instance.particleManager.GetParticle(deadEffect, transform.position, transform.rotation);
    }

    void IHitable.KnockBackEvent(Vector2 hitPos, float forceAmount)
    {
        EnemyKnockBack(hitPos, forceAmount);
    }

    //넉백 루틴은 각자 다름. 플레이어와 같은 Ground인 경우, Orbit인 경우.
    public abstract void EnemyKnockBack(Vector2 hitPos, float forceAmount);

    #endregion

    #region Animation View Events
    protected void StartRunView()
    {
        if (EnemyStartRun != null)
            EnemyStartRun();
    }
    protected void StartIdleView()
    {

        if (EnemyStartIdle != null)
            EnemyStartIdle();
    }

    public void StartAttackView()
    {
        if (EnemyAttackEvent != null)
            EnemyAttackEvent();
    }

    public void FlipToDirectionView()
    {
        if (EnemySeeDirection != null)
            EnemySeeDirection();
    }

    public virtual void StartHitView()
    {
        if (EnemyHitEvent != null)
            EnemyHitEvent();
    }

    public void StartDieView()
    {
        if (EnemyDieEvent != null)
            EnemyDieEvent();
    }

    public void StartClearView()
    {
        if (EnemyClearEvent != null) EnemyClearEvent();
    }
    public void StartResetView()
    {
        if (EnemyResetEvent != null) EnemyResetEvent();
    }


    public void StartAimStart()
    {
        if (EnemyAimOnEvent != null)
            EnemyAimOnEvent();
    }
    public void StartAimStop()
    {
        if (EnemyAimOffEvent != null)
            EnemyAimOffEvent();
    }

    protected void StartStrikeView()
    {
        if (EnemyStrikeEvent != null)
            EnemyStrikeEvent();
    }

    #endregion


    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(transform.position, preState.ToString());
    }

    public Collider2D GetCollider()
    {
        return hitColl;
    }


}




[System.Serializable]
public struct ProjectileStruct
{
    public GameObject projectile;
    public float damage;
    public float speed;
    public float spreadAngle;
    public float lifeTime;

    public ProjectileStruct(GameObject projectile, float damage, float speed, float spreadAngle, float lifeTime)
    {
        this.projectile = projectile;
        this.damage = damage;
        this.speed = speed;
        this.spreadAngle = spreadAngle;
        this.lifeTime = lifeTime;
    }
}

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Die,
    Wait,
    Strike
}

public enum EnemyType
{
    Ground, 
    Orbit,
    Air
}

