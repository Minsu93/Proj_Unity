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

    public bool moveRight;          //ȸ�� ����
    public bool pauseOnAttackMode;  //���ݽ� ����.
    public float pauseTimer = 0.2f; //�ǰ� �� ���� �ð�
    
    public ParticleSystem boosterParticle;

    [Header("Shuttle")]
    public GameObject enemyPrefab;  //������ ��
    public Transform SpawnPoint;   //���� ��ġ

    float moveSpeedMultiplier = 1f; //�� �߰� �� ������ ������ 
    float moveSpd = 0f;
    float pTime;
    int direction;

    bool moveUpdate = true;
    bool attackMode = false;
    Vector3 center;

    AttachToPlanet attachToPlanet;
    EV_OrbitCannon enemyview_s;
    
    //�ӽ�, ȸ�� �뵵
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
 

        //���� �ð�
        if(pTime > 0)
        {
            pTime -= Time.deltaTime;
            return;
        }

        //����
        if (moveUpdate)
        {
            //�ӵ��� 1���� ���� ��� �ӵ��� 1�� ���δ�
            if (moveSpd < 1)
            {
                moveSpd += Time.deltaTime * 10f;
                if (moveSpd >= 1) moveSpd = 1;
            }
                
        }
        else
        {
            //�ӵ��� 0���� Ŭ ��� �ӵ��� 0���� �����
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

        //enemyview�� �ѱ��� ���� ��
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
        //gunTipRot, gunTipPos ������Ʈ
        Vector2 pos = SpawnPoint.position;
        Quaternion rot = transform.rotation;

        GameObject enemy = PoolManager.instance.GetEnemy(enemyPrefab);
        enemy.transform.position = pos;
        enemy.transform.rotation = rot;
        enemy.GetComponent<EnemyBrain>().ResetEnemyBrain();

        //View���� �ִϸ��̼� ����
        //AttackView();
    }

    public override void HitView()
    {
        base.HitView();

        //�ణ ����? 
        pTime = pauseTimer;
    }
    


    public override void AmbushEndEvent()
    {
        hitCollObject.SetActive(true);
        //�����̱� ���� 
        moveUpdate = true;

        ChangeDirection();
    }
    
    //�༺ �ȿ� ���� ��
    public void IsInsidePlanet()
    {
        //�ϴ� ������ �ߴ��Ѵ�. 
        attackOn = false;
        attackMode = false;
        AimStop();
        StopAllCoroutines();
        brain.ChangeState(EnemyState.Idle, 0);
    }

    //�༺ ������ ������ ��
    public void IsOutsidePlanet()
    {
        brain.ChangeState(EnemyState.Chase, 0);
    }

    public void ChangeDirection()
    {
        //moveRight�� ����.
        //����
        moveRight = !moveRight;

        faceRight = moveRight ? true : false;
        FlipToDirectionView();

        direction = moveRight ? -1 : 1;

        //�ӽ� viewObj ȸ��
        if (ViewObj != null)
        {
            ViewObj.transform.localScale = new Vector3(-direction * 2, 2, 2);
        }
    }

}

public enum OrbitType { Attack, Shuttle}