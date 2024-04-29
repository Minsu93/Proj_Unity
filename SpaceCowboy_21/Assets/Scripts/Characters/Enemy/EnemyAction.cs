using SpaceCowboy;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using static UnityEditor.Progress;


public abstract class EnemyAction : MonoBehaviour
{
    /// <summary>
    /// ���� ���� �ٸ� �ൿ�� �Ѵ�. EnemyAction�� Ground, Orbit, Ship ��ο� ���� �ٸ� �ൿ�� �ϵ��� ������� �ִ�. �̵��� �������� Brain �� EnemyState�� ���� �޾ƿ�, State�� �ٲ� �� �׿� �´� �ൿ�� �ϵ��� �ٲ�� ���̴�. 
    /// Brain�� State�� ������ ���ϱ� ������, Action������ �ൿ�� ��ȭ�� õõ�� �Ͼ ������ ���ȴ�. 
    /// �� ���ָ��� �ٸ� Action�� �ϰ�����, ū Ʋ������ ����ϱ� ������ ������ Action�� ��ӹ޾� ����ϴ��� ������ ������ ������� �Ѵ�. 
    /// </summary>
    public bool activate;

    [Header("Move Property")]
    public float enemyHeight = 0.51f;
    public float turnSpeedOnLand = 100f;

    [Header("BodyAttack")]
    public float bodyDamage = 3.0f;

    [Header("Strike Property")]
    public float strikeSpeed = 10f;     //���� �ӵ�
    public float distanceFromStrikePoint = 0f;      //���� �Ÿ� 

    //���� ����
    bool isAimOn = false;   //���� ���ΰ���
    protected bool onAttack;  //�������� �� 
    protected bool onChase = false;   //chase ���ΰ���
    protected bool startChase;        //chase ���� �� �ѹ��� ����.

    protected EnemyState preState = EnemyState.Die;
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



    protected virtual void Awake()
    {
        gravity = GetComponent<CharacterGravity>();
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<EnemyBrain>();
        attack = GetComponent<EnemyAttack>();
        chase = GetComponent<EnemyChase>();
        hitColl = GetComponent<Collider2D>();
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
        EnemyState currState = brain.enemyState;

        if (currState != preState)
        {
            DoAction(currState);
            preState = currState;
        }

        if (!activate) return;

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

    //���°� �ٲ� �� �ѹ��� ���� 
    protected virtual void DoAction(EnemyState state)
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

            case EnemyState.Attack:
                onChase = false;
                onAttack = true;
                break;

            case EnemyState.Die:
                onChase = false;
                onAttack = false;
                OnDieAction();
                break;
        }
    }


    #region Strike Mode
    public void StrikeAction()
    {
        //�÷��̾� �༺ or �� �༺�� ����Ǿ��ִ� �༺�� ����. 
        List<Planet> strikePlanets = new List<Planet>();
        List<int> weights = new List<int>();

        Planet p = GameManager.Instance.playerNearestPlanet;
        if(p == null)
        {
            //�÷��̾� �༺�� ���� ��. 
        }
        strikePlanets.Add(p);
        weights.Add(p.linkedPlanetList.Count);  //�÷��̾� �༺ : �ֺ� �༺ ������ 50:50�̴�.

        foreach (PlanetBridge bridge in p.linkedPlanetList)
        {
            strikePlanets.Add(bridge.planet);
            weights.Add(1);
        }
        Planet randomPlanet = SelectPlanet(strikePlanets, weights);

        int strikePointIndex;
        Vector2 StrikePosition;

        if (randomPlanet == p)   //�÷��̾��� �༺�� ������ ���
        {
            //�ش� �༺ �� ���� ����� ����Ʈ�� ���Ѵ�. 
            strikePointIndex = randomPlanet.GetClosestIndex(transform.position);
            StrikePosition = randomPlanet.worldPoints[strikePointIndex];
        }
        else //�ֺ� �༺�� ������ ���
        {
            //�÷��̾� �༺ ������ plant bridge�� �����´�.
            randomPlanet.GetjumpPoint(p, out PlanetBridge pb);
            strikePointIndex = pb.bridgeIndex;
            StrikePosition = randomPlanet.worldPoints[strikePointIndex];
        }

        //ĳ���͸� ȸ���Ѵ�. 
        Vector2 rotateVec =  (Vector2)transform.position - StrikePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateVec.normalized);

        //������ �����Ѵ�. 
        StartCoroutine(StrikeRoutine(StrikePosition));

    }

    Planet SelectPlanet(List<Planet> planets, List<int> weights)
    {
        int totalWeight = 0;
        foreach (var item in weights)
        {
            totalWeight += item;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomWeight < cumulativeWeight)
            {
                return planets[i];
            }
        }
        return planets[0];
    }

    protected virtual IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position,enemyHeight, dir.normalized, float.MaxValue, LayerMask.GetMask("Planet"));
        if (hit.collider == null) yield break;

        Vector2 strikeStartPos = transform.position;
        Vector2 strikeTargetPos = hit.point;
        float strikeTime = hit.distance / strikeSpeed;
        float time = 0; //���� �ð�
        float distance = hit.distance; //���� �Ÿ�
        while (distance > distanceFromStrikePoint + enemyHeight)
        {
            time += Time.deltaTime;
            distance = Vector2.Distance(transform.position, strikeTargetPos);
            rb.MovePosition(Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }
        //�����ϸ� �Ϲ� ������ ���ư���. resetBrain
        yield return new WaitForSeconds(0.5f);
        brain.WakeUp();
    }
    #endregion

    #region Basic Actions
   
    protected IEnumerator DelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    public void ResetAction()
    {
        activate = true;

        onAttack = false;
        onAir = true;

        hitColl.enabled = true;

        ResetView();
    }

    //Start���� Sleep������ ��. ���� �� ���̰ų�, �������� �ʴ´�. 
    protected void OnSleepEvent()
    {
        activate = false;
        hitColl.enabled = false;
    }

    protected void AddOnPlanetEnemyList()
    {
        //���� �༺�� �ִٸ� ����Ʈ���� �����Ѵ�. 
        if (prePlanet != null)
        {
            if (prePlanet.enemyList.Contains(brain)) prePlanet.enemyList.Remove(brain);
        }

        Planet p = gravity.nearestPlanet;

        if (p != null)
        {
            p.enemyList.Add(brain);
            prePlanet = p;
        }
    }

    public virtual void WakeUpEvent()
    {
        activate = true;
        hitColl.enabled = true;

        gravity.activate = true;
    }

    protected virtual void OnDieAction()
    {
        StopAllCoroutines();
        attack.StopAttackAction();
        DieView();

        activate = false;
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
        if (bodyDamage == 0) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerBehavior>(out PlayerBehavior pb))
            {
                pb.DamageEvent(bodyDamage, transform.position);
            }
        }
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



