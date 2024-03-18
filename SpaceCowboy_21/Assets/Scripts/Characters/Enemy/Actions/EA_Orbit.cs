using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class EA_Orbit : EnemyAction
{

    [Header("Orbit")]
    public OrbitType orbitType = OrbitType.Attack;

    public bool moveRight;          //회전 방향
    public bool pauseOnAttackMode;  //공격시 정지.
    public float pauseTimer = 0.2f; //피격 시 정지 시간
    
    public ParticleSystem boosterParticle;

    [Header("Shuttle")]
    public GameObject enemyPrefab;  //스폰할 적
    public Transform SpawnPoint;   //스폰 위치

    float moveSpeedMultiplier = 1f; //적 발견 시 빠르게 움직임 
    float moveSpd = 0f;
    float pTime;
    int direction;

    bool moveUpdate = true;
    bool attackMode = false;
    Vector3 center;

    AttachToPlanet attachToPlanet;
    EV_OrbitCannon enemyview_s;
    
    //임시, 회전 용도
    public GameObject ViewObj;

    protected override void Awake()
    {
        base.Awake();

        enemyview_s = GetComponentInChildren<EV_OrbitCannon>();

        attachToPlanet = GetComponent<AttachToPlanet>();

    }
    private void Start()
    {
        center = attachToPlanet.coll.transform.position;

    }

    private void FixedUpdate()
    {
 

        //스턴 시간
        if(pTime > 0)
        {
            pTime -= Time.deltaTime;
            return;
        }

        //공전
        if (moveUpdate)
        {
            //속도가 1보다 낮은 경우 속도를 1로 높인다
            if (moveSpd < 1)
            {
                moveSpd += Time.deltaTime * 10f;
                if (moveSpd >= 1) moveSpd = 1;
            }
                
        }
        else
        {
            //속도가 0보다 클 경우 속도를 0으로 낮춘다
            if (moveSpd > 0)
            {
                moveSpd -= Time.deltaTime * 10f;
                if(moveSpd <= 0) { moveSpd = 0; }
            }
        }
        if(moveSpd > 0)
            transform.RotateAround(center, Vector3.forward, direction * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
    }

    public override void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Ambush:
                AmbushStartEvent();
                moveUpdate = false;
                break;

            case EnemyState.Idle:
                StartIdleView();
                ChangeDirection();
                if(boosterParticle != null) boosterParticle.Play();

                break;

            case EnemyState.Chase:
                if (!attackMode)
                {
                    attackMode = true;
                    AttackModeEvent();
                    moveSpeedMultiplier = 2.0f;
                }
                if (pauseOnAttackMode) 
                {
                    moveUpdate = true;
                } 
                StopAllCoroutines();

                break;

            case EnemyState.Attack:
                if (pauseOnAttackMode) moveUpdate = false;

                StopAllCoroutines();
                StartCoroutine(AttackRepeater());
                break;


            case EnemyState.Die:
                moveUpdate = false;

                StopAllCoroutines();
                DieView();
                if (boosterParticle != null) boosterParticle.Stop();

                hitCollObject.SetActive(false);
                gameObject.SetActive(false);
                moveSpeedMultiplier = 1f;
                break;
        }
    }

    IEnumerator AttackRepeater()
    {
        while (true)
        {
            yield return StartCoroutine(AttackCoroutine());
            yield return new WaitForSeconds(attackCoolTime);
        }
    }

    protected IEnumerator AttackCoroutine()
    {
        attackOn = true;

        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        //enemyview가 총구가 있을 때
        if(orbitType == OrbitType.Attack)
        {
            var guntip = enemyview_s.GetGunTipPos();

            yield return StartCoroutine(ShootRoutine(guntip.Item1, guntip.Item2, 0, AttackDelay));
        }
        else if(orbitType == OrbitType.Shuttle)
        {
            SummonAction();
        }
        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        attackOn = false;
    }

    void SummonAction()
    {
        //gunTipRot, gunTipPos 업데이트
        Vector2 pos = SpawnPoint.position;
        Quaternion rot = transform.rotation;

        GameObject enemy = PoolManager.instance.GetEnemy(enemyPrefab);
        enemy.transform.position = pos;
        enemy.transform.rotation = rot;
        enemy.GetComponent<EnemyBrain>().ResetEnemyBrain();

        //View에서 애니메이션 실행
        //AttackView();
    }

    public override void HitView()
    {
        base.HitView();

        //약간 경직? 
        pTime = pauseTimer;
    }
    


    public override void AmbushEndEvent()
    {
        hitCollObject.SetActive(true);
        //움직이기 시작 
        moveUpdate = true;

        ChangeDirection();
    }
    
    //행성 안에 있을 때
    public void IsInsidePlanet()
    {
        //하던 공격을 중단한다. 
        attackOn = false;
        attackMode = false;
        AimStop();
        StopAllCoroutines();
        brain.ChangeState(EnemyState.Idle, 0);
    }

    //행성 밖으로 나왔을 때
    public void IsOutsidePlanet()
    {
        brain.ChangeState(EnemyState.Chase, 0);
    }

    public void ChangeDirection()
    {
        //moveRight만 조절.
        //반전
        moveRight = !moveRight;

        faceRight = moveRight ? true : false;
        FlipToDirectionView();

        direction = moveRight ? -1 : 1;

        //임시 viewObj 회전
        if (ViewObj != null)
        {
            ViewObj.transform.localScale = new Vector3(-direction * 2, 2, 2);
        }
    }

}

public enum OrbitType { Attack, Shuttle}