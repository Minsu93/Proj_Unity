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
    //���߿� �� �ִ� �ð� 
    float airTime;
    float maxAirTime = 1.0f;
    bool onJump;
    float lastJumpTime;
    Vector2[] ppoints; //��ġ, ȸ��, ũ�Ⱑ ����� points����Ʈ, �༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 

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

        //���߿� ���� ������ �������� ���� ������Ų��. 
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
                //Wait���·� �Ѿ ���� �ϴ� ��� �ൿ�� ���߰� ������ �ִ´�. 
                StopAllCoroutines();
                break;

            case EnemyState.Die:
                StopAllCoroutines();
                DieView();

                //�Ѿ˿� �´� Enemy Collision ����
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

        //normal�� transform.upward ������ �� ���̰� ũ�� ������ �����ش�. 
        float rotateAngle = Vector2.Angle(transform.up, normal);

        turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);

        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);
    }



    void MoveUpdateOnAir()
    {
        //�÷��̾ ���߿� ���� ��, �� �ִ� �ð��� ������� �༺ �������� ���� ���������. 
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

        //üũ ������ �� �����̸� ����. 
        if (Time.time - lastJumpTime < 0.1f)
            return;

        //ĳ������ �߽ɿ���, �� ������ ���̸� ���� ĳ���Ͱ� ���߿� �� �ִ��� �˻��Ѵ�.
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
        //���� �÷��̾ ���̴� Collider2D ���� ����Ʈ���� ���� �˾Ƴ���.
        List<int> visiblePoints = new List<int>();

        //�༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        int pointCounts = ppoints.Length - 1;
        Vector2 playerPos = brain.playerTr.position;

        for (int i = 0; i < pointCounts; i++)
        {
            //1,2,3 ���. 4�� ����� ����
            if (i % 4 != 0)
                continue;

            Vector2 pointVector = ppoints[i];
            Vector2 dir = playerPos - pointVector;
            float dist = dir.magnitude;
            dir = dir.normalized;

            //�����Ÿ� ������ ���鸸 üũ�Ѵ�
            if (dist > brain.attackRange)
                continue;

            //��AI�� �� �ִ� �༺�� Point �߿��� �÷��̾ ���̴� Point �鸸 �̾Ƴ���. 
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

        //�÷��̾ ���̴� ������ ������.
        if (visiblePoints.Count <= 0)
        {
            return;
        }


        //��AI�� ���� ��ġ�� ���Ѵ�. 
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


        // VisiblePoints �߿��� �� AI���� ����� ����Ʈ���� ���Ѵ�. 
        List<int> closePointsInVisiblePoints = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            int min = int.MaxValue; // ��������� �ּڰ��� ������ ������ �ʱ�ȭ�մϴ�.
            int minIndex = -1; // �ּڰ��� �ε����� ������ ������ �ʱ�ȭ�մϴ�.

            // ����Ʈ�� ��ȸ�ϸ鼭 �ּڰ��� ã���ϴ�.
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

            // �ּڰ��� ã�����Ƿ� ��� ����Ʈ�� �߰��ϰ� ���� ����Ʈ���� �����մϴ�.
            if (minIndex != -1)
            {
                closePointsInVisiblePoints.Add(visiblePoints[minIndex]);
                visiblePoints.RemoveAt(minIndex);
            }
        }


        //���� ���� ����Ʈ�� ���Ѵ�
        int randomInt = UnityEngine.Random.Range(0, closePointsInVisiblePoints.Count);
        int targetIndex = closePointsInVisiblePoints[randomInt];


        //�̵� �ִϸ��̼�
        StartRunView();

        //�̵� �����.
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

        //���� index�� ���Ѵ�.
        int dirIndex;
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

        //+����(positive)�� ������ 1, -����(negative)�� ������ -1
        dirIndex = positive < negative ? 1 : -1;

        //������ �ٶ󺸴� ������ dirIndex�� ���� �޶�����. 
        faceRight = dirIndex > 0f ? true : false;
        FlipToDirectionView();


        //Ÿ���� �ִ� MoveRoutine
        int currIndex = closestIndex;
        int nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
        float currTime = Time.time;
        float randomF = UnityEngine.Random.Range(0.0f, 0.5f);



        while (Time.time - currTime < 10)
        {
            //Vector2 targetPointPos = ppoints[targetIndex];
            //Vector2 pastPointPos = ppoints[currIndex];

            ////������ ����� �븻�� ���Ѵ�.
            //Vector2 direction = targetPointPos - pastPointPos;
            //Vector2 normal = Vector2.Perpendicular(direction).normalized * dirIndex;

            //���� ������ ��Ҵ� Ÿ�� + �븻�������� Ű��ŭ ������ �ִ� ���
            Vector2 movePos = ppoints[nextIndex];

            Vector2 moveDir = (movePos - rb.position).normalized;
            float moveDist = (movePos - rb.position).magnitude;

            Debug.DrawRay(rb.position, movePos - rb.position, Color.cyan, 0.5f);

            // ������Ʈ�� �̵� �������� �̵�
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

            //targetIndex�� ������� ������ ���� �����Ѵ�. 
            if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < randomF )
            {
                //�ִϸ��̼� ����
                StartIdleView();

                yield break;
            }

            // ������ �Ÿ��� ���� ����������� Ÿ���� �ٲ۴�.
            if (moveDist < moveSpeed * Time.fixedDeltaTime)
            {
                currIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
            }

            yield return null;
        }


        //�ִϸ��̼� ����
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
        //�༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        
        int pointCounts = gravity.nearestCollider.points.Length - 1;
        Planet playerPlanet = brain.playerTr.GetComponent<CharacterGravity>().nearestPlanet;


        PlanetBridge bridge = gravity.nearestPlanet.GetjumpPoint(playerPlanet);
        if(bridge.planet == null) { yield break; }

        int jumpPointIndex = bridge.bridgeIndex;
        Vector2 closestPoint = bridge.targetVector;

        //�� ĳ������ ���� ��ġ�� ���Ѵ�. 
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


        //�̵�
        StartRunView();

        yield return StartCoroutine(MoveRoutine(jumpPointIndex, closestIndex));

        //����
        StartCoroutine(JumpToPlanet(closestPoint));

    }

    IEnumerator JumpToPlanet(Vector2 jumpPoint)
    {
        //�ִϸ��̼� ����
        StartIdleView();

        //��� ���
        yield return new WaitForSeconds(0.3f);

        //gravity.activate = false;
        onAir = true;
        onJump = true;
        lastJumpTime = Time.time;

        //����
        Vector2 dir = jumpPoint - (Vector2)transform.position;
        rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);



    }


    #endregion

    #region KnockBackMethod
    //public void EnemyKnockBack(Vector2 hitPoint)
    //{
    //    //������, ���� ���� ��ƾ���� �����Ѵ�. 
    //    if (gravity == null)
    //        return;

    //    StopAllCoroutines();
    //    brain.enemyState = EnemyState.Stun;

    //    //���� �κ��� ĳ���� �������� �������� ���Ѵ�
    //    Vector2 hitVec = hitPoint - (Vector2)transform.position;

    //    int dirInt = Vector2.SignedAngle(transform.up, hitVec) < 0 ? -1 : 1;

    //    int pointCounts = gravity.nearestEdgeCollider.pointCount - 1;

    //    //ĳ������ ��ġ�� ���Ѵ�. 
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

    //    //���� �κ��� �ݴ�������� ĳ���͸� �� �ʰ� �̵��ñ��

    //    StartCoroutine(MoveRoutine(closestIndex, dirInt, pointCounts, knockBackTime, knockBackSpeed));

    //}
    #endregion


    protected virtual IEnumerator AttackCoroutine()
    {
        attackOn = true;
        attackCool = true;

        //���� ����
        AimOnView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        //gunTipRot, gunTipPos ������Ʈ
        var guntip = enemyview_s.GetGunTipPos();

        yield return StartCoroutine(ShootRoutine(guntip.Item1, guntip.Item2, 0 ,AttackDelay));

        //���� �Ϸ�
        AimOffView();

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        brain.ChangeState(EnemyState.Chase, 0f);
        attackOn = false;
    }


}
