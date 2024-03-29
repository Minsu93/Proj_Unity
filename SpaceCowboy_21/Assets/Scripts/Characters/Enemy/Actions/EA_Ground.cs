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
    //���� 
    public float jumpForce = 10f;
    
    bool startJump; //���� ���� �� �ѹ��� ����
    bool onChaseMode;  //���� �غ�
    bool onJumpMode;

    //Move
    protected int targetIndex;
    protected int nextIndex;
    protected int closestIndex;
    protected int dirIndex;

    float mTimer;
    float jTimer;

    Vector2 closestTargetPoint;
    protected Vector2[] ppoints; //��ġ, ȸ��, ũ�Ⱑ ����� points����Ʈ, �༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 

    //����
    public float maxAirTime = 1.0f;
    float airTime;
    protected float lastJumpTime;

    
    
    //��ũ��Ʈ
    protected EnemyView_Shooter enemyview_s;

    protected override void Awake()
    {
        base.Awake();

        enemyview_s = GetComponentInChildren<EnemyView_Shooter>();  //>> view���� ���� ��ũ��Ʈ ���߰ڴ�.
    }


    protected override void Update()
    {
        CheckOnAir();

        //���߿� ���� ������ �������� ���� ������Ų��. 
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

        //���� ��Ÿ�� ���ȿ��� �ൿ�� �����. �����߿� �̵��ϰ� ������ Update�� �����ض�.
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

    //�� ������ ������Ʈ
    protected override void OnChaseAction()
    {
        //0.5�ʸ��� Ÿ�� ������Ʈ
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

        //�� ������ ���� 
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

        //normal�� transform.upward ������ �� ���̰� ũ�� ������ �����ش�. 
        float rotateAngle = Vector2.Angle(transform.up, normal);
        turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);
    }


    //�÷��̾ ���߿� ���� ��, �� �ִ� �ð��� ������� �༺ �������� ���� ���������. 
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

        //���� ���� ������
        if (Time.time - lastJumpTime < 0.1f)
            return;

        //���� ��
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
        //���� �÷��̾ ���̴� Collider2D ���� ����Ʈ���� ���� �˾Ƴ���.
        List<int> visiblePoints = new List<int>();

        //�༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 
        ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

        int pointCounts = ppoints.Length - 1;
        Vector2 playerPos = brain.playerTr.position;
        float maxF = float.MaxValue;

        for (int i = 0; i < pointCounts; i++)
        {
            //1,2,3 ���. 4�� ����� ����
            if (i % 4 != 0)
                continue;

            Vector2 pointVector = ppoints[i];
            Vector2 dir = playerPos - pointVector;
            float dist = dir.magnitude;
            dir = dir.normalized;

            //��AI�� �� �ִ� �༺�� Point �߿��� �÷��̾ ���̴� Point �鸸 �̾Ƴ���. 
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

        //�÷��̾ ���̴� ������ ������.
        if (visiblePoints.Count <= 0)
        {
            return;
        }

        //����� ���� �ε��� ��ġ�� ���Ѵ�. 
        closestIndex = gravity.nearestPlanet.GetClosestIndex(transform.position);

        //���� index�� ���Ѵ�.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

        //+����(positive)�� ������ 1, -����(negative)�� ������ -1
        dirIndex = positive < negative ? 1 : -1;
        nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;


        //�̵� ������ �ٶ󺻴�.
        faceRight = dirIndex > 0f ? true : false;
        FlipToDirectionView();

        ////�̵� �ִϸ��̼�
        //StartRunView();

    }

    bool MoveToTarget()
    {
        if (targetIndex < 0) return false;

        int pointCounts = ppoints.Length - 1;

        Vector2 movePos = ppoints[nextIndex];
        Vector2 moveDir = (movePos - rb.position).normalized;
        float moveDist = (movePos - rb.position).magnitude;

        // ������ �Ÿ��� ���� ����������� Ÿ���� �ٲ۴�.
        if (moveDist < moveSpeed * Time.fixedDeltaTime)
        {
            closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
            nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
        }
        // �������� ������ �����ϸ� �����Ѵ�.
        if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < 0.1f)
        {
            //onChase = false;
            return true;
        }

        // ������Ʈ�� �̵� �������� �̵�
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        return false;
        
    }

    

    #endregion

    #region JumpMethod

    void PrepareJump()
    {
        //�༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 
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

        //���� index�� ���Ѵ�.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

        //+����(positive)�� ������ 1, -����(negative)�� ������ -1
        dirIndex = positive < negative ? 1 : -1;
        nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;

        //�̵� ������ �ٶ󺻴�.
        faceRight = dirIndex > 0f ? true : false;
        FlipToDirectionView();
    }

    void JumpToPlanet()
    {
        ////�ִϸ��̼� ����
        //StartIdleView();

        //gravity.activate = false;
        onAir = true;
        //startJump = true;
        lastJumpTime = Time.time;
        airTime = 0;

        //����
        Vector2 dir = closestTargetPoint - (Vector2)transform.position;
        rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);
    }


    #endregion

    //#region KnockBackMethod
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
    //#endregion


    protected virtual IEnumerator AttackCoroutine()
    {
        //ĳ���� ��ġ�� ȸ��.
        faceRight = Vector2.SignedAngle(transform.up, brain.playerDirection) < 0 ? true : false;
        FlipToDirectionView();

        //���� ����
        AimOnView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));
        
        var guntip = enemyview_s.GetGunTipPos();
        ShootAction(guntip.Item1, guntip.Item2, 0);

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        //���� �Ϸ�
        AimOffView();
    }



}
