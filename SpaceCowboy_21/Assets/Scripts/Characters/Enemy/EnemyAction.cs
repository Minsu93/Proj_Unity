using SpaceCowboy;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;


public abstract class EnemyAction : MonoBehaviour, IHitable , ITarget, IKickable
{
    /// <summary>
    /// ���� ���� �ٸ� �ൿ�� �Ѵ�. EnemyAction�� Ground, Orbit, Ship ��ο� ���� �ٸ� �ൿ�� �ϵ��� ������� �ִ�. �̵��� �������� Brain �� EnemyState�� ���� �޾ƿ�, State�� �ٲ� �� �׿� �´� �ൿ�� �ϵ��� �ٲ�� ���̴�. 
    /// Brain�� State�� ������ ���ϱ� ������, Action������ �ൿ�� ��ȭ�� õõ�� �Ͼ ������ ���ȴ�. 
    /// �� ���ָ��� �ٸ� Action�� �ϰ�����, ū Ʋ������ ����ϱ� ������ ������ Action�� ��ӹ޾� ����ϴ��� ������ ������ ������� �Ѵ�. 
    /// </summary>
    public bool activate;
    public EnemyType enemyType = EnemyType.Ground;     //���� Ÿ��.WaveManager���� �޾ƿ� ���� �� �ٸ� ��������� �����Ѵ�. 

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

    [Header("Kicked Property")]
    [SerializeField] protected bool knockbackable = false;    //������ �¾��� �� �˹� �Ǵ��� ������
    [SerializeField] protected float knockbackAmount = 5.0f;

    //����Ʈ
    [Header("VFX")]
    [SerializeField] protected ParticleSystem hitEffect;    //�¾��� �� ȿ��
    [SerializeField] ParticleSystem deadEffect;   //�׾��� �� ȿ��

    //���� ����
    protected bool onAttack;  //�������� �� 
    protected bool onChase = false;   //chase ���ΰ���
    protected bool onWait = false;    // wait ���ΰ���?

    protected bool startChase;        //chase ���� �� �ѹ��� ����.
    protected float groggyChance = 1f;    //�׷α� ���°� �� Ȯ�� * 100f
    protected bool groggyOn = false;
    protected float groggyTime = 5f;

    protected EnemyState preState = EnemyState.Strike;
    public bool faceRight { get; set; }  //ĳ���Ͱ� �������� ���� �ֽ��ϱ�? 
    public bool onAir { get; set; } //���߿� �ִ°�


    //��ũ��Ʈ
    protected EnemyBrain brain;
    protected EnemyAttack attack;
    protected EnemyChase chase;
    protected CharacterGravity gravity;
    protected Rigidbody2D rb;
    protected Collider2D enemyColl;
    public GameObject iconUI;
    protected Planet prePlanet;
    protected Health health;
    protected DropItem dropItem;

    [SerializeField] protected Collider2D projHitColl;


    //�̺�Ʈ
    public event System.Action<EnemyState> EnemyChangeStateEvent;
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
        enemyColl = GetComponent<Collider2D>();
        health = GetComponent<Health>();
        dropItem = GetComponent<DropItem>();

    }

    private void Start()
    {
        WaveManager.instance.MonsterDisappearEvent += WaveClearEvent;
        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //�÷��̾ ���� ��� �����ϴ� �̺�Ʈ 
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
        DoAction();

        //pause ����.
        if (enemyState == EnemyState.Groggy) return;
        //if (enemyState == EnemyState.Wait) return;
        if (onWait) return;

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

    // ������Ʈ�ϱ� �� �����̴�. true�� ������Ʈ�� �������� �ʴ´�. 
    protected virtual bool BeforeUpdate()
    {
        if (!activate) return true;
        return false;
    }
        
    // ���� ������ ���� �ٸ��� ��Ȳ�� �����Ѵ�. 
    public abstract void BrainStateChange();

    // EnemyState�� ���� �ٸ� �ൿ ���� 
    protected void DoAction()
    {
        if (enemyState != preState)
        {
            ActionByState(enemyState);
            if (EnemyChangeStateEvent != null) EnemyChangeStateEvent(enemyState);
            preState = enemyState;
            //�׾����� ��ü ��Ȱ��ȭ.
            if(enemyState == EnemyState.Die)
            {
                activate = false;
            }
        }
    }

    //���°� �ٲ� �� �ѹ��� ���� 
    protected virtual void ActionByState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                onChase = false;
                onAttack = false;
                break;

            case EnemyState.Chase:
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
    //���� �� �˵� Ÿ���� Strike ���
    public void EnemyStartStrike(Vector2 strikePos)
    {
        ResetAction();

        enemyState = EnemyState.Strike;
        StartStrikeView();

        //ĳ���͸� ȸ���Ѵ�. 
        Vector2 rotateVec = (Vector2)transform.position - strikePos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateVec.normalized);

        //������ �����Ѵ�. 
        StartCoroutine(StrikeRoutine(strikePos));
    }

    protected abstract IEnumerator StrikeRoutine(Vector2 strikePos);

    //Strike ������ �����. 
    public virtual void AfterStrikeEvent()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Chase;
        activate = true;
        gravity.activate = true;
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
        enemyColl.enabled = false;

        //�̵��Ϸ� �� ����ȭ.
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
    //���� ���� ������ �ൿ


    //�÷��̾ ���� ��� �ٽ� ����. 
    public void PlayerIsDead()
    {
        //if (enemyState == EnemyState.Die) return;
        enemyState = EnemyState.Idle;
    }

    //�ʱ�ȭ
    public void ResetAction()
    {
        onChase = false;
        onAttack = false;
        onWait = false;

        enemyColl.enabled = true;
        EnemyIgnoreProjectile(false);

        preState = EnemyState.Strike;

        health.ResetHealth();
        StartResetView();
    }

    public virtual void AfterAttack()
    {
        EnemyPause(attack.attackCoolTime);
    }

    //���� ����
    public void EnemyPause(float second)
    {
        StartCoroutine(PauseRoutine(second));
    }
    IEnumerator PauseRoutine(float sec)
    {
        //���� �ൿ����
        onWait = true;

        yield return new WaitForSeconds(sec);
        onWait = false;
    }
    public void EnemyForcePause()
    {
        onWait = true;
    }
    public void EnemyForceWake()
    {
        onWait = false;
    }


    //protected virtual void OnDieAction()
    //{
    //    activate = false;

    //    StopAllCoroutines();
    //    attack.StopAttackAction();
    //    StartDieView();

    //    iconUI.SetActive(false);
    //    hitColl.enabled = false;

    //    StartCoroutine(DieRoutine(3.0f));
    //}


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
        if( enemyState == EnemyState.Groggy) return;

        if (health.AnyDamage(damage))
        {
           
            if (health.IsDead())
            {
                ////�׷α� ����
                //if( UnityEngine.Random.Range(0, 1f) < groggyChance)
                //{
                //    StopAllCoroutines();
                //    StartCoroutine(GroggyEvent());
                //}
                //else
                //    WhenDieEvent();
                WhenDieEvent();
            } 
            else
            {
                //�´� ȿ�� 
                StartHitView();
                if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);
            }
        }
    }

    protected IEnumerator GroggyEvent()
    {
        Debug.Log("Groggy");

        groggyOn = true;
        enemyState = EnemyState.Groggy;
        
        onChase = false;
        onAttack = false;
        attack.StopAttackAction();
        StartAimStop();
        EnemyIgnoreProjectile(true);

        float time = 0;

        while(groggyOn && time < groggyTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        groggyOn = false;
        WhenDieEvent();
    }

    public virtual void WhenDieEvent()
    {
        enemyState = EnemyState.Die;

        //activate = false;
        onChase = false;
        onAttack = false;

        StopAllCoroutines();
        attack.StopAttackAction();
        if (EnemyDieEvent != null) EnemyDieEvent();

        enemyColl.enabled = false;
        EnemyIgnoreProjectile(true);

        StartCoroutine(DieRoutine(3.0f));

        //WaveManager�� ����.
        if (WaveManager.instance != null)
            WaveManager.instance.CountEnemyLeft(this.gameObject);

        if (deadEffect != null) GameManager.Instance.particleManager.GetParticle(deadEffect, transform.position, transform.rotation);
    }

    protected IEnumerator DieRoutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        gravity.activate = false;
        gameObject.SetActive(false);
    }

    public void Kicked(Vector2 hitPos)
    {
        if (knockbackable)
        {
            EnemyKnockBack(hitPos, knockbackAmount);
        }

        //if(groggyOn)
        //{
        //    groggyOn = false;
        //    StartHitView();
        //    dropItem.GenerateItem();
        //}
        
    }
    //�˹� ��ƾ�� ���� �ٸ�. �÷��̾�� ���� Ground�� ���, Orbit�� ���.
    public abstract void EnemyKnockBack(Vector2 hitPos, float forceAmount);

    public void EnemyIgnoreProjectile(bool ignore)
    {
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("PlayerProj"), ignore);
        projHitColl.enabled = !ignore;
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
        return enemyColl;
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
    Strike,
    Groggy
}

public enum EnemyType
{
    Ground, 
    Orbit,
    Air
}

