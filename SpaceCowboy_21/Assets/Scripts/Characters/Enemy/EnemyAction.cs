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
    bool isAimOn = false;   //조준 중인가요
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
    public event System.Action EnemyStartRun;
    public event System.Action EnemyStartIdle;
    public event System.Action EnemyAttackEvent;
    public event System.Action EnemyAimOnEvent;
    public event System.Action EnemyAimOffEvent;
    public event System.Action EnemySeeDirection;
    public event System.Action EnemyHitEvent;
    public event System.Action EnemyDieEvent;
    public event System.Action EnemyResetEvent;
    public event System.Action EnemyStrikeEvent;
    public event System.Action EnemyClearEvent;




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
        WaveManager.instance.StageClear += WaveClearEvent;
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

    /// <summary>
    /// 업데이트하기 전 조건이다. true면 업데이트를 수행하지 않는다. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool BeforeUpdate()
    {
        if (!activate) return true;

        return false;
    }

    
    /// <summary>
    /// 유닛 종류에 따라 다르게 상황을 감지한다. 
    /// </summary>
    public abstract void BrainStateChange();

    //상태가 바뀔 때 한번만 실행 
    protected virtual void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                onChase = false;
                onAttack = false;
                StartIdleView();
                break;

            case EnemyState.Chase:
                StartIdleView();
                onChase = true;
                onAttack = false;
                break;

            case EnemyState.Attack:
                onChase = false;
                onAttack = true;
                break;

        }
    }


    #region Strike Mode
    public void EnemyStartStrike(Planet target)
    {
        ResetAction();

        StrikeAction(target);
        
        StrikeView();
    }

    public void StrikeAction(Planet targetPlanet)
    {
        int strikePointIndex;
        Vector2 StrikePosition;
        strikePointIndex = targetPlanet.GetClosestIndex(transform.position);
        StrikePosition = targetPlanet.worldPoints[strikePointIndex];

        //캐릭터를 회전한다. 
        Vector2 rotateVec =  (Vector2)transform.position - StrikePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateVec.normalized);

        //강습을 시작한다. 
        StartCoroutine(StrikeRoutine(StrikePosition));
    }

    //Planet SelectPlanet(List<Planet> planets, List<int> weights)
    //{
    //    int totalWeight = 0;
    //    foreach (var item in weights)
    //    {
    //        totalWeight += item;
    //    }

    //    int randomWeight = Random.Range(0, totalWeight);
    //    int cumulativeWeight = 0;

    //    for (int i = 0; i < weights.Count; i++)
    //    {
    //        cumulativeWeight += weights[i];
    //        if (randomWeight < cumulativeWeight)
    //        {
    //            return planets[i];
    //        }
    //    }
    //    return planets[0];
    //}

    protected virtual IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        enemyState = EnemyState.Strike;

        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position,enemyHeight, dir.normalized, float.MaxValue, LayerMask.GetMask("Planet"));
        if (hit.collider == null) yield break;

        Vector2 strikeStartPos = transform.position;
        Vector2 normal = (hit.point - strikeStartPos).normalized;
        Vector2 strikeTargetPos = hit.point - (normal * (distanceFromStrikePoint + enemyHeight));
        
        float strikeTime = (strikeStartPos - strikeTargetPos).magnitude / strikeSpeed;
        float time = 0; //강습 시간
        //float distance = hit.distance; //남은 거리
        while (time < strikeTime)
        {
            time += Time.deltaTime;
            //distance = Vector2.Distance(transform.position, strikeTargetPos);
            rb.MovePosition(Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }
        //착지하면 활동 시작. 
        yield return new WaitForSeconds(0.5f);
        WakeUpEvent();
    }
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
        ClearView();

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

    //플레이어가 죽은 경우 다시 잠든다. 
    public void PlayerIsDead()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Idle;
    }

    //셔틀에서 스폰될 때? 
    public void ResetAction()
    {
        onChase = false;
        onAttack = false;
        
        hitColl.enabled = true;

        preState = EnemyState.Strike;

        health.ResetHealth();
        ResetView();
    }

    //Strike 끝나고 깨어나기. 
    public virtual void WakeUpEvent()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Chase;
        activate = true;
        gravity.activate = true;
    }

    protected virtual void OnDieAction()
    {
        activate = false;

        StopAllCoroutines();
        attack.StopAttackAction();
        DieView();

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
            HitView();
            if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

            if (health.IsDead())
            {
                WhenDieEvent();

                GameManager.Instance.playerManager.ChargeFireworkEnergy();
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
        DieView();

        iconUI.SetActive(false);
        hitColl.enabled = false;

        StartCoroutine(DieRoutine(3.0f));
        
        //WaveManager에 전달.
        if(WaveManager.instance != null)
            WaveManager.instance.CountEnemyLeft(this.gameObject);

        if (deadEffect != null) GameManager.Instance.particleManager.GetParticle(deadEffect, transform.position, transform.rotation);
    }

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

    public void AttackView()
    {
        if (EnemyAttackEvent != null)
            EnemyAttackEvent();
    }

    protected void AimOnView()
    {
        if (EnemyAimOnEvent != null)
            EnemyAimOnEvent();
    }
    protected void AimOffView()
    {
        if (EnemyAimOffEvent != null)
            EnemyAimOffEvent();
    }

    public void FlipToDirectionView()
    {
        if (EnemySeeDirection != null)
            EnemySeeDirection();
    }

    public virtual void HitView()
    {
        if (EnemyHitEvent != null)
            EnemyHitEvent();
    }

    public void DieView()
    {
        if (EnemyDieEvent != null)
            EnemyDieEvent();
    }

    public void ClearView()
    {
        if (EnemyClearEvent != null) EnemyClearEvent();
    }
    public void ResetView()
    {
        if (EnemyResetEvent != null) EnemyResetEvent();
    }


    public void AimStart()
    {
        if (!isAimOn)
        {
            isAimOn = true;
            AimOnView();
        }
    }
    public void AimStop()
    {
        if (isAimOn)
        {
            isAimOn = false;
            AimOffView();
        }
    }

    protected void StrikeView()
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


