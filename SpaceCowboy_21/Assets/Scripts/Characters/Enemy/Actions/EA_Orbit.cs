using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class EA_Orbit : EnemyAction
{

    [Header("Orbit")]
    public bool moveRight;          //회전 방향
    public bool pauseOnAttackMode;  //공격시 정지.
    public float pauseTimer = 0.1f; //피격 시 정지 시간

    [Header("Orbit Attack")]
    public int burstNumber = 3;
    public float burstDelay = 0.5f;
    
    //변수
    float moveSpeedMultiplier = 1f; //적 발견 시 빠르게 움직임 
    float moveSpd = 0f;
    float pTime;
    protected int direction;
    bool moveUpdate = true;
    Vector3 center;
    
    //파티클
    public ParticleSystem boosterParticle;

    //스크립트
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

    protected void Start()
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


    protected override void OnChaseAction()
    {
        return;
    }

    protected override void OnAttackAction()
    {
        attackCool = true;
        StartCoroutine(AttackCoroutine());
    }

    protected virtual IEnumerator AttackCoroutine()
    {
        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        int burst = burstNumber;
        //if (enemyview_s != null)
        //{
        //    var guntip = enemyview_s.GetGunTipPos();
        //    ShootAction(guntip.Item1, guntip.Item2, 0);
        //}

        while (burst > 0)
        {
            burst--;

            Vector2 pos = transform.position;
            Vector2 vec = Quaternion.Euler(0, 0, 90) * brain.playerDirection;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, vec);

            ShootAction(pos, rot, 0);

            yield return new WaitForSeconds(burstDelay);
        }
    }




    public override void HitView()
    {
        //애니메이션 이벤트
        base.HitView();

        //약간 경직? 
        pTime = pauseTimer;
    }
    


    public override void WakeUpEvent()
    {
        base.WakeUpEvent();

        //움직이기 시작 
        moveUpdate = true;
        if (boosterParticle != null) boosterParticle.Play();
        ChangeDirectionToRight(moveRight);
    }

    protected override void OnDieAction()
    {
        base.OnDieAction();

        moveUpdate = false;
        if (boosterParticle != null) boosterParticle.Stop();

    }

    public virtual void ChangeDirectionToRight(bool right)
    {

        faceRight = right ? true : false;
        FlipToDirectionView();

        direction = right ? -1 : 1;


    }

}
