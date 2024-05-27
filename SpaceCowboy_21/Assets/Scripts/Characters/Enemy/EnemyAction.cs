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
    /// ���� ���� �ٸ� �ൿ�� �Ѵ�. EnemyAction�� Ground, Orbit, Ship ��ο� ���� �ٸ� �ൿ�� �ϵ��� ������� �ִ�. �̵��� �������� Brain �� EnemyState�� ���� �޾ƿ�, State�� �ٲ� �� �׿� �´� �ൿ�� �ϵ��� �ٲ�� ���̴�. 
    /// Brain�� State�� ������ ���ϱ� ������, Action������ �ൿ�� ��ȭ�� õõ�� �Ͼ ������ ���ȴ�. 
    /// �� ���ָ��� �ٸ� Action�� �ϰ�����, ū Ʋ������ ����ϱ� ������ ������ Action�� ��ӹ޾� ����ϴ��� ������ ������ ������� �Ѵ�. 
    /// </summary>
    public bool activate;
    public EnemyState enemyState = EnemyState.Strike;


    [Header("Move Property")]
    public float enemyHeight = 0.51f;
    public float turnSpeedOnLand = 100f;

    [Header("BodyAttack")]
    public float bodyDamage = 3.0f;

    [Header("Strike Property")]
    public float strikeSpeed = 10f;     //���� �ӵ�
    public float distanceFromStrikePoint = 0f;      //���� �Ÿ� 

    [Header("Clear Property")]
    [SerializeField] float clearMoveDistance = 3.0f;
    [SerializeField] float clearMoveTime = 3.0f;
    [SerializeField] AnimationCurve clearCurve;

    //����Ʈ
    [Header("VFX")]
    [SerializeField] protected ParticleSystem hitEffect;    //�¾��� �� ȿ��
    [SerializeField] ParticleSystem deadEffect;   //�׾��� �� ȿ��

    //���� ����
    bool isAimOn = false;   //���� ���ΰ���
    protected bool onAttack;  //�������� �� 
    protected bool onChase = false;   //chase ���ΰ���
    protected bool startChase;        //chase ���� �� �ѹ��� ����.

    protected EnemyState preState = EnemyState.Strike;
    public bool faceRight { get; set; }  //ĳ���Ͱ� �������� ���� �ֽ��ϱ�? 
    public bool onAir { get; set; } //���߿� �ִ°�


    //��ũ��Ʈ
    protected EnemyBrain brain;
    protected EnemyAttack attack;
    protected EnemyChase chase;
    protected CharacterGravity gravity;
    protected Rigidbody2D rb;
    protected Collider2D hitColl;
    public GameObject iconUI;
    protected Planet prePlanet;
    protected Health health;




    //�̺�Ʈ
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
        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //�÷��̾ ���� ��� �����ϴ� �̺�Ʈ 
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
        //Enemy�� �װų� Strike�� ������ ������ Update���� �ʴ´�. 
        if (BeforeUpdate()) return;

        //�극�ο��� �÷��̾� ���� ���� ������Ʈ
        brain.TotalCheck();

        //������Ʈ�� ���� enemyState ����. 
        BrainStateChange();

        //enemyState�� ���� Action ���� 
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
    /// ������Ʈ�ϱ� �� �����̴�. true�� ������Ʈ�� �������� �ʴ´�. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool BeforeUpdate()
    {
        if (!activate) return true;

        return false;
    }

    
    /// <summary>
    /// ���� ������ ���� �ٸ��� ��Ȳ�� �����Ѵ�. 
    /// </summary>
    public abstract void BrainStateChange();

    //���°� �ٲ� �� �ѹ��� ���� 
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

        //ĳ���͸� ȸ���Ѵ�. 
        Vector2 rotateVec =  (Vector2)transform.position - StrikePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateVec.normalized);

        //������ �����Ѵ�. 
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
        float time = 0; //���� �ð�
        //float distance = hit.distance; //���� �Ÿ�
        while (time < strikeTime)
        {
            time += Time.deltaTime;
            //distance = Vector2.Distance(transform.position, strikeTargetPos);
            rb.MovePosition(Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }
        //�����ϸ� Ȱ�� ����. 
        yield return new WaitForSeconds(0.5f);
        WakeUpEvent();
    }
    #endregion

    #region Wave Clear
    //������ ��� ���� ������� �̺�Ʈ
    protected virtual void WaveClearEvent()
    {
        if (!activate) return;
        activate = false;

        StopAllCoroutines();
        attack.StopAttackAction();
        iconUI.SetActive(false);
        hitColl.enabled = false;

        //�̵��Ϸ� �� ����ȭ.
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

    //�÷��̾ ���� ��� �ٽ� ����. 
    public void PlayerIsDead()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Idle;
    }

    //��Ʋ���� ������ ��? 
    public void ResetAction()
    {
        onChase = false;
        onAttack = false;
        
        hitColl.enabled = true;

        preState = EnemyState.Strike;

        health.ResetHealth();
        ResetView();
    }

    //Strike ������ �����. 
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
            //�´� ȿ�� 
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
        
        //WaveManager�� ����.
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


