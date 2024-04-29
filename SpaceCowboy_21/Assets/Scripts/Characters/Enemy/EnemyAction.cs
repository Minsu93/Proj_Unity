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
    /// 적에 따라 다른 행동을 한다. EnemyAction은 Ground, Orbit, Ship 모두에 따라 다른 행동을 하도록 만들어져 있다. 이들의 공통점은 Brain 의 EnemyState를 매턴 받아와, State가 바뀔 때 그에 맞는 행동을 하도록 바뀌는 것이다. 
    /// Brain의 State가 느리게 변하기 때문에, Action에서는 행동의 변화가 천천히 일어날 것으로 기대된다. 
    /// 각 유닛마다 다른 Action을 하겠지만, 큰 틀에서는 비슷하기 때문에 동일한 Action을 상속받아 사용하더라도 문제가 없도록 만들려고 한다. 
    /// </summary>
    public bool activate;

    [Header("Move Property")]
    public float enemyHeight = 0.51f;
    public float turnSpeedOnLand = 100f;

    [Header("BodyAttack")]
    public float bodyDamage = 3.0f;

    [Header("Strike Property")]
    public float strikeSpeed = 10f;     //강습 속도
    public float distanceFromStrikePoint = 0f;      //정지 거리 

    //로컬 변수
    bool isAimOn = false;   //조준 중인가요
    protected bool onAttack;  //공격중일 때 
    protected bool onChase = false;   //chase 중인가요
    protected bool startChase;        //chase 시작 시 한번만 실행.

    protected EnemyState preState = EnemyState.Die;
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

    //상태가 바뀔 때 한번만 실행 
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
        //플레이어 행성 or 그 행성과 연결되어있는 행성을 고른다. 
        List<Planet> strikePlanets = new List<Planet>();
        List<int> weights = new List<int>();

        Planet p = GameManager.Instance.playerNearestPlanet;
        if(p == null)
        {
            //플레이어 행성이 없을 때. 
        }
        strikePlanets.Add(p);
        weights.Add(p.linkedPlanetList.Count);  //플레이어 행성 : 주변 행성 비율은 50:50이다.

        foreach (PlanetBridge bridge in p.linkedPlanetList)
        {
            strikePlanets.Add(bridge.planet);
            weights.Add(1);
        }
        Planet randomPlanet = SelectPlanet(strikePlanets, weights);

        int strikePointIndex;
        Vector2 StrikePosition;

        if (randomPlanet == p)   //플레이어의 행성에 떨어질 경우
        {
            //해당 행성 위 가장 가까운 포인트를 구한다. 
            strikePointIndex = randomPlanet.GetClosestIndex(transform.position);
            StrikePosition = randomPlanet.worldPoints[strikePointIndex];
        }
        else //주변 행성에 떨어질 경우
        {
            //플레이어 행성 방향의 plant bridge를 가져온다.
            randomPlanet.GetjumpPoint(p, out PlanetBridge pb);
            strikePointIndex = pb.bridgeIndex;
            StrikePosition = randomPlanet.worldPoints[strikePointIndex];
        }

        //캐릭터를 회전한다. 
        Vector2 rotateVec =  (Vector2)transform.position - StrikePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateVec.normalized);

        //강습을 시작한다. 
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
        float time = 0; //강습 시간
        float distance = hit.distance; //남은 거리
        while (distance > distanceFromStrikePoint + enemyHeight)
        {
            time += Time.deltaTime;
            distance = Vector2.Distance(transform.position, strikeTargetPos);
            rb.MovePosition(Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }
        //착지하면 일반 적으로 돌아간다. resetBrain
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

    //Start에서 Sleep상태일 때. 적은 안 보이거나, 움직이지 않는다. 
    protected void OnSleepEvent()
    {
        activate = false;
        hitColl.enabled = false;
    }

    protected void AddOnPlanetEnemyList()
    {
        //이전 행성이 있다면 리스트에서 제거한다. 
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



