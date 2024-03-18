using SpaceCowboy;
using SpaceEnemy;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;

public class EA_Ground : EnemyAction
{
    public float jumpForce = 10f;
    //공중에 떠 있는 시간 
    float airTime;
    float maxAirTime = 1.0f;
    bool onJump;
    float lastJumpTime;
    Vector2[] ppoints; //위치, 회전, 크기가 적용된 points리스트, 행성 바뀔 때만 한번씩 가져오면 된다. 

    Coroutine chaseRoutine;
    EnemyView_Shooter enemyview_s;
    protected override void Awake()
    {
        base.Awake();

        enemyview_s = GetComponentInChildren<EnemyView_Shooter>();
    }

    //protected override void OnEnable()
    //{
    //    base.OnEnable();
    //    gravity.PlanetChangedEvent += GetPlanetPoints;
    //}

    //private void Start()
    //{
    //    gravity.PlanetChangedEvent += GetPlanetPoints;
    //}

    protected override void Update()
    {
        CheckOnAir();

        //공중에 오래 있으면 지면으로 강제 착륙시킨다. 
        if (onAir)
        {
            if (airTime < maxAirTime)
            {
                airTime += Time.deltaTime;

                if (airTime > maxAirTime) MoveUpdateOnAir();
            }
        }

        RotateCharacterToGround();

        if (onAir) return;

        base.Update();
    }

    public override void DoAction (EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Ambush:
                AmbushStartEvent();
                break;

            case EnemyState.Idle:
                StartIdleView();
                break;

            case EnemyState.Chase:
                StopAllCoroutines();
                StartCoroutine(ChaseRepeater());
                break;

            case EnemyState.ToJump:
                StopAllCoroutines();
                StartCoroutine(StartToJump());
                break;

            case EnemyState.Attack:
                StopAllCoroutines();
                StartCoroutine(AttackCoroutine());
                break;

            case EnemyState.Wait:
                //Wait상태로 넘어갈 때는 하던 모든 행동을 멈추고 가만히 있는다. 
                StopAllCoroutines();
                break;

            case EnemyState.Die:
                StopAllCoroutines();
                DieView();

                //총알에 맞는 Enemy Collision 해제
                hitCollObject.SetActive(false);
                //DelayedDying(2f);
                break;
        }
    }

    

    #region OnAir

    void RotateCharacterToGround()
    {
        if (gravity.nearestPlanet == null)
            return;

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



    void MoveUpdateOnAir()
    {
        //플레이어가 공중에 있을 때, 떠 있는 시간이 길어지면 행성 방향으로 점점 가까워진다. 
        if (gravity.nearestPlanet)
        {
            Vector2 vec = gravity.nearestPointGravityVector;
            float vel = rb.velocity.magnitude;


            rb.velocity = Vector2.Lerp(rb.velocity, vec * vel, 1f * Time.fixedDeltaTime);
        }
    }

    void CheckOnAir()
    {
        if (!onAir) return;

        //체크 시작할 때 딜레이를 주자. 
        if (Time.time - lastJumpTime < 0.1f)
            return;

        //캐릭터의 중심에서, 발 밑으로 레이를 쏴서 캐릭터가 공중에 떠 있는지 검사한다.
        RaycastHit2D footHit = Physics2D.CircleCast(transform.position, enemyHeight, transform.up * -1, 0.1f, LayerMask.GetMask("Planet"));
        if (footHit.collider != null)
        {
            onAir = false;
            rb.velocity = Vector2.zero;
            if (onJump)
            {
                //gravity.activate = true;
                onJump = false;
            }
        }

    }

    #endregion

    #region ChaseMethods

    protected IEnumerator ChaseRepeater()
    {
        while (true)
        {
            StartChase();
            yield return new WaitForSeconds(0.5f);
        }
    }


    void StartChase()
    {
        //먼저 플레이어가 보이는 Collider2D 상의 포인트들의 값을 알아낸다.
        List<int> visiblePoints = new List<int>();

        //행성 바뀔 때만 한번씩 가져오면 된다. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        int pointCounts = ppoints.Length - 1;
        Vector2 playerPos = brain.playerTr.position;

        for (int i = 0; i < pointCounts; i++)
        {
            //1,2,3 통과. 4의 배수만 진행
            if (i % 4 != 0)
                continue;

            Vector2 pointVector = ppoints[i];
            Vector2 dir = playerPos - pointVector;
            float dist = dir.magnitude;
            dir = dir.normalized;

            //사정거리 내부의 점들만 체크한다
            if (dist > brain.attackRange)
                continue;

            //적AI가 서 있는 행성의 Point 중에서 플레이어가 보이는 Point 들만 뽑아낸다. 
            RaycastHit2D hit = Physics2D.Raycast(pointVector, dir, dist, LayerMask.GetMask("Planet"));
            if (hit.collider == null)
            {
                visiblePoints.Add(i);
                Debug.DrawRay(pointVector, dir * dist, Color.green, 0.5f);
            }
            else
            {
                Debug.DrawRay(pointVector, dir * dist, Color.red, 0.5f);
            }
        }

        //플레이어가 보이는 구역이 없으면.
        if (visiblePoints.Count <= 0)
        {
            return;
        }


        //적AI의 현재 위치를 구한다. 
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < pointCounts; i++)
        {
            Vector2 v2 = ppoints[i];
            float distance = Vector2.Distance(transform.position, v2);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }


        // VisiblePoints 중에서 적 AI에게 가까운 포인트들을 구한다. 
        List<int> closePointsInVisiblePoints = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            int min = int.MaxValue; // 현재까지의 최솟값을 저장할 변수를 초기화합니다.
            int minIndex = -1; // 최솟값의 인덱스를 저장할 변수를 초기화합니다.

            // 리스트를 순회하면서 최솟값을 찾습니다.
            for (int j = 0; j < visiblePoints.Count; j++)
            {
                int pos = (visiblePoints[j] - closestIndex + pointCounts) % pointCounts;
                int neg = (closestIndex - visiblePoints[j] + pointCounts) % pointCounts;
                int betweenIndex = pos < neg ? pos : neg;


                if (betweenIndex < min)
                {
                    min = betweenIndex;
                    minIndex = j;
                }
            }

            // 최솟값을 찾았으므로 결과 리스트에 추가하고 원래 리스트에서 제거합니다.
            if (minIndex != -1)
            {
                closePointsInVisiblePoints.Add(visiblePoints[minIndex]);
                visiblePoints.RemoveAt(minIndex);
            }
        }


        //이중 랜덤 포인트를 구한다
        int randomInt = UnityEngine.Random.Range(0, closePointsInVisiblePoints.Count);
        int targetIndex = closePointsInVisiblePoints[randomInt];


        //이동 애니메이션
        StartRunView();

        //이동 재시작.
        if(chaseRoutine != null)
        {
            StopCoroutine(chaseRoutine);
            chaseRoutine = null;
        }

        chaseRoutine = StartCoroutine(MoveRoutine(targetIndex, closestIndex));
    }

    IEnumerator MoveRoutine(int targetIndex, int closestIndex)
    {
        int pointCounts = gravity.nearestCollider.points.Length - 1;

        //방향 index를 구한다.
        int dirIndex;
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

        //+방향(positive)이 가까우면 1, -방향(negative)이 가까우면 -1
        dirIndex = positive < negative ? 1 : -1;

        //적들이 바라보는 방향은 dirIndex에 따라 달라진다. 
        faceRight = dirIndex > 0f ? true : false;
        FlipToDirectionView();


        //타겟이 있는 MoveRoutine
        int currIndex = closestIndex;
        int nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
        float currTime = Time.time;
        float randomF = UnityEngine.Random.Range(0.0f, 0.5f);



        while (Time.time - currTime < 10)
        {
            //Vector2 targetPointPos = ppoints[targetIndex];
            //Vector2 pastPointPos = ppoints[currIndex];

            ////움직일 장소의 노말을 구한다.
            //Vector2 direction = targetPointPos - pastPointPos;
            //Vector2 normal = Vector2.Perpendicular(direction).normalized * dirIndex;

            //최종 움직일 장소는 타겟 + 노말방향으로 키만큼 높은데 있는 장소
            Vector2 movePos = ppoints[nextIndex];

            Vector2 moveDir = (movePos - rb.position).normalized;
            float moveDist = (movePos - rb.position).magnitude;

            Debug.DrawRay(rb.position, movePos - rb.position, Color.cyan, 0.5f);

            // 오브젝트를 이동 방향으로 이동
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            //targetIndex에 어느정도 가까이 오면 중지한다. 
            if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < randomF )
            {
                //애니메이션 정지
                StartIdleView();

                yield break;
            }

            // 움직일 거리가 거의 가까워졌으면 타겟을 바꾼다.
            if (moveDist < moveSpeed * Time.fixedDeltaTime)
            {
                currIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
            }

            yield return null;
        }


        //애니메이션 정지
        StartIdleView();
    }

    void GetPlanetPoints()
    {
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);
    }
    //Vector2 GetPointPos(int pointIndex)
    //{
    //    Vector3 localPoint = gravity.nearestCollider.points[pointIndex];
    //    Vector2 pointPos = gravity.nearestCollider.transform.TransformPoint(localPoint);
    //    return pointPos;
    //}


    #endregion

    #region JumpMethod

    protected IEnumerator StartToJump()
    {
        //행성 바뀔 때만 한번씩 가져오면 된다. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        
        int pointCounts = gravity.nearestCollider.points.Length - 1;
        Planet playerPlanet = brain.playerTr.GetComponent<CharacterGravity>().nearestPlanet;


        PlanetBridge bridge = gravity.nearestPlanet.GetjumpPoint(playerPlanet);
        if(bridge.planet == null) { yield break; }

        int jumpPointIndex = bridge.bridgeIndex;
        Vector2 closestPoint = bridge.targetVector;

        //적 캐릭터의 현재 위치를 구한다. 
        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < pointCounts; i++)
        {
            Vector2 worldEdgePoint = ppoints[i];
            float distance = Vector2.Distance(transform.position, worldEdgePoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }


        //이동
        StartRunView();

        yield return StartCoroutine(MoveRoutine(jumpPointIndex, closestIndex));

        //점프
        StartCoroutine(JumpToPlanet(closestPoint));

    }

    IEnumerator JumpToPlanet(Vector2 jumpPoint)
    {
        //애니메이션 정지
        StartIdleView();

        //잠시 대기
        yield return new WaitForSeconds(0.3f);

        //gravity.activate = false;
        onAir = true;
        onJump = true;
        lastJumpTime = Time.time;

        //점프
        Vector2 dir = jumpPoint - (Vector2)transform.position;
        rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);



    }


    #endregion

    #region KnockBackMethod
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
    #endregion


    protected virtual IEnumerator AttackCoroutine()
    {
        attackOn = true;
        attackCool = true;

        //조준 시작
        AimOnView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        //gunTipRot, gunTipPos 업데이트
        var guntip = enemyview_s.GetGunTipPos();

        yield return StartCoroutine(ShootRoutine(guntip.Item1, guntip.Item2, 0 ,AttackDelay));

        //조준 완료
        AimOffView();

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        brain.ChangeState(EnemyState.Chase, 0f);
        attackOn = false;
    }


}
