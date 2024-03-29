using SpaceCowboy;
using SpaceEnemy;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;

public class EA_Ground : EnemyAction
{
    //점프 
    public float jumpForce = 10f;
    
    bool startJump; //점프 시작 시 한번만 실행
    bool onChaseMode;  //점프 준비
    bool onJumpMode;

    //Move
    protected int targetIndex;
    protected int nextIndex;
    protected int closestIndex;
    protected int dirIndex;

    float mTimer;
    float jTimer;

    Vector2 closestTargetPoint;
    protected Vector2[] ppoints; //위치, 회전, 크기가 적용된 points리스트, 행성 바뀔 때만 한번씩 가져오면 된다. 

    //공중
    public float maxAirTime = 1.0f;
    float airTime;
    protected float lastJumpTime;

    
    
    //스크립트
    protected EnemyView_Shooter enemyview_s;

    protected override void Awake()
    {
        base.Awake();

        enemyview_s = GetComponentInChildren<EnemyView_Shooter>();  //>> view말고 따로 스크립트 빼야겠다.
    }


    protected override void Update()
    {
        CheckOnAir();

        //공중에 오래 있으면 지면으로 강제 착륙시킨다. 
        if (onAir)
        {
            if (airTime < maxAirTime)
            {
                airTime += Time.deltaTime;
            }
            else MoveUpdateOnAir();
        }

        RotateCharacterToGround();

        if (onAir) return;

        EnemyState currState = brain.enemyState;
        if (currState != preState)
        {
            StopAction(preState);
            DoAction(currState);
            preState = currState;
        }

        if (!activate) return;

        if (attackCool)
        {
            atTimer += Time.deltaTime;
            if (atTimer > attackCoolTime)
            {
                attackCool = false;
                atTimer = 0;
            }
        }

        //공격 쿨타임 동안에는 행동을 멈춘다. 공격중에 이동하고 싶으면 Update를 수정해라.
        if (attackCool) return;

        if (onAttack)
        {
            OnAttackAction();
        }

        if (onChase)
        {
            OnChaseAction();
        }
    }

    protected override void StopAction(EnemyState preState)
    {
        switch (preState)
        {
            case EnemyState.Sleep:
                WakeUpEvent();
                break;
            case EnemyState.Chase:
                onChase = false;
                break;
            case EnemyState.Attack:
                onAttack = false;
                break;
        }
    }

    //매 프레임 업데이트
    protected override void OnChaseAction()
    {
        //0.5초마다 타겟 업데이트
        if (mTimer > 0)
        {
            mTimer -= Time.deltaTime;
        }
        else
        {
            mTimer = 0.5f;
            if (brain.OnOtherPlanetCheck())
            {
                onChaseMode = false;
                PrepareJump();
            }
            else
            {
                onChaseMode = true;
                PrepareChase();
            }
        }

        //매 프레임 실행 
        if (!onChaseMode)
        {
            if (MoveToTarget()) JumpToPlanet();
        }
        else
        {
            MoveToTarget();
        }

    }

    protected override void OnAttackAction()
    {
        attackCool = true;
        StartCoroutine(AttackCoroutine());
    }


    #region OnAir

    void RotateCharacterToGround()
    {
        if (gravity.nearestPlanet == null) return;

        Vector2 upVec = ((Vector2)transform.position - gravity.nearestPoint).normalized;
        RotateToVector(upVec, turnSpeedOnLand);
    }

    void RotateToVector(Vector2 normal, float turnSpeed)
    {
        Vector3 vectorToTarget = normal;
        int turnspeedMultiplier;

        //normal과 transform.upward 사이의 값 차이가 크면 보정을 가해준다. 
        float rotateAngle = Vector2.Angle(transform.up, normal);
        turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);
    }


    //플레이어가 공중에 있을 때, 떠 있는 시간이 길어지면 행성 방향으로 점점 가까워진다. 
    void MoveUpdateOnAir()
    {
        if (gravity.nearestPlanet == null) return;
        
        Vector2 vec = gravity.nearestPointGravityVector;
        float vel = rb.velocity.magnitude;
        rb.velocity = Vector2.Lerp(rb.velocity, vec * vel, 1f * Time.fixedDeltaTime);
    }

    void CheckOnAir()
    {
        if (!onAir) return;

        //점프 시작 딜레이
        if (Time.time - lastJumpTime < 0.1f)
            return;

        //착지 시
        RaycastHit2D footHit = Physics2D.CircleCast(transform.position, enemyHeight, transform.up * -1, 0.1f, LayerMask.GetMask("Planet"));
        if (footHit.collider != null)
        {
            onAir = false;
            rb.velocity = Vector2.zero;

            airTime = 0;

            if (brain.OnOtherPlanetCheck())
            {
                onChaseMode = false;
                PrepareJump();
            }
            else
            {
                onChaseMode = true;
                PrepareChase();
            }

        }

    }

    #endregion

    #region ChaseMethods  
    void PrepareChase()
    {
        //먼저 플레이어가 보이는 Collider2D 상의 포인트들의 값을 알아낸다.
        List<int> visiblePoints = new List<int>();

        //행성 바뀔 때만 한번씩 가져오면 된다. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        int pointCounts = ppoints.Length - 1;
        Vector2 playerPos = brain.playerTr.position;
        float maxF = float.MaxValue;

        for (int i = 0; i < pointCounts; i++)
        {
            //1,2,3 통과. 4의 배수만 진행
            if (i % 4 != 0)
                continue;

            Vector2 pointVector = ppoints[i];
            Vector2 dir = playerPos - pointVector;
            float dist = dir.magnitude;
            dir = dir.normalized;

            //적AI가 서 있는 행성의 Point 중에서 플레이어가 보이는 Point 들만 뽑아낸다. 
            RaycastHit2D hit = Physics2D.Raycast(pointVector, dir, dist, LayerMask.GetMask("Planet"));
            if (hit.collider == null)
            {
                visiblePoints.Add(i);
                if(dist < maxF)
                {
                    maxF = dist;
                    targetIndex = i;
                }
                Debug.DrawRay(pointVector, dir * dist, Color.green, 0.5f);
            }
        }

        //플레이어가 보이는 구역이 없으면.
        if (visiblePoints.Count <= 0)
        {
            return;
        }

        //현재와 다음 인덱스 위치를 구한다. 
        closestIndex = gravity.nearestPlanet.GetClosestIndex(transform.position);

        //방향 index를 구한다.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

        //+방향(positive)이 가까우면 1, -방향(negative)이 가까우면 -1
        dirIndex = positive < negative ? 1 : -1;
        nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;


        //이동 방향을 바라본다.
        faceRight = dirIndex > 0f ? true : false;
        FlipToDirectionView();

        ////이동 애니메이션
        //StartRunView();

    }

    bool MoveToTarget()
    {
        if (targetIndex < 0) return false;

        int pointCounts = ppoints.Length - 1;

        Vector2 movePos = ppoints[nextIndex];
        Vector2 moveDir = (movePos - rb.position).normalized;
        float moveDist = (movePos - rb.position).magnitude;

        // 움직일 거리가 거의 가까워졌으면 타겟을 바꾼다.
        if (moveDist < moveSpeed * Time.fixedDeltaTime)
        {
            closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
            nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
        }
        // 목적지에 가까이 도착하면 종료한다.
        if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < 0.1f)
        {
            //onChase = false;
            return true;
        }

        // 오브젝트를 이동 방향으로 이동
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        return false;
        
    }

    

    #endregion

    #region JumpMethod

    void PrepareJump()
    {
        //행성 바뀔 때만 한번씩 가져오면 된다. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        int pointCounts = gravity.nearestCollider.points.Length - 1;
        Planet playerPlanet = GameManager.Instance.playerNearestPlanet;
        //PlanetBridge bridge = gravity.nearestPlanet.GetjumpPoint(playerPlanet);
        //if (bridge.planet == null) { return; }

        bool findBridge = gravity.nearestPlanet.GetjumpPoint(playerPlanet, out PlanetBridge _bridge);
        if (!findBridge)
        {
            targetIndex = -1;
            return;
        }

        targetIndex = _bridge.bridgeIndex;
        closestTargetPoint = _bridge.targetVector;
        closestIndex = gravity.nearestPlanet.GetClosestIndex(transform.position);

        //방향 index를 구한다.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

        //+방향(positive)이 가까우면 1, -방향(negative)이 가까우면 -1
        dirIndex = positive < negative ? 1 : -1;
        nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;

        //이동 방향을 바라본다.
        faceRight = dirIndex > 0f ? true : false;
        FlipToDirectionView();
    }

    void JumpToPlanet()
    {
        ////애니메이션 정지
        //StartIdleView();

        //gravity.activate = false;
        onAir = true;
        //startJump = true;
        lastJumpTime = Time.time;
        airTime = 0;

        //점프
        Vector2 dir = closestTargetPoint - (Vector2)transform.position;
        rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);
    }


    #endregion

    //#region KnockBackMethod
    //public void EnemyKnockBack(Vector2 hitPoint)
    //{
    //    //움직임, 슈팅 등의 루틴들을 정지한다. 
    //    if (gravity == null)
    //        return;

    //    StopAllCoroutines();
    //    brain.enemyState = EnemyState.Stun;

    //    //맞은 부분이 캐릭터 앞쪽인지 뒷쪽인지 구한다
    //    Vector2 hitVec = hitPoint - (Vector2)transform.position;

    //    int dirInt = Vector2.SignedAngle(transform.up, hitVec) < 0 ? -1 : 1;

    //    int pointCounts = gravity.nearestEdgeCollider.pointCount - 1;

    //    //캐릭터의 위치를 구한다. 
    //    float closestDistance = Mathf.Infinity;
    //    int closestIndex = -1;

    //    for (int i = 0; i < pointCounts; i++)
    //    {
    //        Vector2 worldEdgePoint = GetPointPos(i);
    //        float distance = Vector2.Distance(transform.position, worldEdgePoint);

    //        if (distance < closestDistance)
    //        {
    //            closestDistance = distance;
    //            closestIndex = i;
    //        }
    //    }

    //    //맞은 부분의 반대방향으로 캐릭터를 몇 초간 이동시긴다

    //    StartCoroutine(MoveRoutine(closestIndex, dirInt, pointCounts, knockBackTime, knockBackSpeed));

    //}
    //#endregion


    protected virtual IEnumerator AttackCoroutine()
    {
        //캐릭터 위치로 회전.
        faceRight = Vector2.SignedAngle(transform.up, brain.playerDirection) < 0 ? true : false;
        FlipToDirectionView();

        //조준 시작
        AimOnView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));
        
        var guntip = enemyview_s.GetGunTipPos();
        ShootAction(guntip.Item1, guntip.Item2, 0);

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        //조준 완료
        AimOffView();
    }



}
