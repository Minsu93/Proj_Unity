using SpaceCowboy;
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

    //����
    public float maxAirTime = 1.0f;
    public float airTime;
    public float lastJumpTime; 

    protected override void Update()
    {
        //���� üũ �߰�
        CheckOnAir();

        EnemyState currState = brain.enemyState;

        if (currState != preState)
        {
            DoAction(currState);
            preState = currState;
        }

        //���� ȸ�� �߰�
        if(currState != EnemyState.Strike) RotateCharacterToGround();

        if (!activate) return;

        //���� üũ �߰�
        if (onAir) return;

        if (onAttack)
        {
            if (attack != null)
                attack.OnAttackAction();
        }

        if (onChase)
        {
            if (chase != null)
                chase.OnChaseAction();
        }
    }



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
        }

        //���߿� ���� ������ �������� ���� ������Ų��. 

        if (airTime < maxAirTime)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            if (gravity.nearestPlanet == null) return;

            Vector2 vec = (gravity.nearestPoint - (Vector2)transform.position).normalized;
            float vel = rb.velocity.magnitude;
            rb.velocity = Vector2.Lerp(rb.velocity, vec * vel, 1f * Time.fixedDeltaTime);
        }

    }


    //#region ChaseMethods  
    //void PrepareChase()
    //{
    //    //���� �÷��̾ ���̴� Collider2D ���� ����Ʈ���� ���� �˾Ƴ���.
    //    List<int> visiblePoints = new List<int>();

    //    //�༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 
    //    ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

    //    int pointCounts = ppoints.Length - 1;
    //    Vector2 playerPos = brain.playerTr.position;
    //    float maxF = float.MaxValue;

    //    for (int i = 0; i < pointCounts; i++)
    //    {
    //        //1,2,3 ���. 4�� ����� ����
    //        if (i % 4 != 0)
    //            continue;

    //        Vector2 pointVector = ppoints[i];
    //        Vector2 dir = playerPos - pointVector;
    //        float dist = dir.magnitude;
    //        dir = dir.normalized;

    //        //��AI�� �� �ִ� �༺�� Point �߿��� �÷��̾ ���̴� Point �鸸 �̾Ƴ���. 
    //        RaycastHit2D hit = Physics2D.Raycast(pointVector, dir, dist, LayerMask.GetMask("Planet"));
    //        if (hit.collider == null)
    //        {
    //            visiblePoints.Add(i);
    //            if(dist < maxF)
    //            {
    //                maxF = dist;
    //                targetIndex = i;
    //            }
    //            Debug.DrawRay(pointVector, dir * dist, Color.green, 0.5f);
    //        }
    //    }

    //    //�÷��̾ ���̴� ������ ������.
    //    if (visiblePoints.Count <= 0)
    //    {
    //        return;
    //    }

    //    //����� ���� �ε��� ��ġ�� ���Ѵ�. 
    //    closestIndex = gravity.nearestPlanet.GetClosestIndex(transform.position);

    //    //���� index�� ���Ѵ�.
    //    int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
    //    int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

    //    //+����(positive)�� ������ 1, -����(negative)�� ������ -1
    //    dirIndex = positive < negative ? 1 : -1;
    //    nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;


    //    //�̵� ������ �ٶ󺻴�.
    //    faceRight = dirIndex > 0f ? true : false;
    //    FlipToDirectionView();

    //    ////�̵� �ִϸ��̼�
    //    //StartRunView();

    //}

    //bool MoveToTarget()
    //{
    //    if (targetIndex < 0) return false;

    //    int pointCounts = ppoints.Length - 1;

    //    Vector2 movePos = ppoints[nextIndex];
    //    Vector2 moveDir = (movePos - rb.position).normalized;
    //    float moveDist = (movePos - rb.position).magnitude;

    //    // ������ �Ÿ��� ���� ����������� Ÿ���� �ٲ۴�.
    //    if (moveDist < moveSpeed * Time.fixedDeltaTime)
    //    {
    //        closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
    //        nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
    //    }
    //    // �������� ������ �����ϸ� �����Ѵ�.
    //    if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < 0.1f)
    //    {
    //        //onChase = false;
    //        return true;
    //    }

    //    // ������Ʈ�� �̵� �������� �̵�
    //    rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    //    return false;
        
    //}

    

    //#endregion

    //#region JumpMethod

    //void PrepareJump()
    //{
    //    //�༺ �ٲ� ���� �ѹ��� �������� �ȴ�. 
    //    ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

    //    int pointCounts = gravity.nearestCollider.points.Length - 1;
    //    Planet playerPlanet = GameManager.Instance.playerNearestPlanet;
    //    //PlanetBridge bridge = gravity.nearestPlanet.GetjumpPoint(playerPlanet);
    //    //if (bridge.planet == null) { return; }

    //    bool findBridge = gravity.nearestPlanet.GetjumpPoint(playerPlanet, out PlanetBridge _bridge);
    //    if (!findBridge)
    //    {
    //        targetIndex = -1;
    //        return;
    //    }

    //    targetIndex = _bridge.bridgeIndex;
    //    closestTargetPoint = _bridge.targetVector;
    //    closestIndex = gravity.nearestPlanet.GetClosestIndex(transform.position);

    //    //���� index�� ���Ѵ�.
    //    int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
    //    int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

    //    //+����(positive)�� ������ 1, -����(negative)�� ������ -1
    //    dirIndex = positive < negative ? 1 : -1;
    //    nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;

    //    //�̵� ������ �ٶ󺻴�.
    //    faceRight = dirIndex > 0f ? true : false;
    //    FlipToDirectionView();
    //}

    //void JumpToPlanet()
    //{
    //    ////�ִϸ��̼� ����
    //    //StartIdleView();

    //    //gravity.activate = false;
    //    onAir = true;
    //    //startJump = true;
    //    lastJumpTime = Time.time;
    //    airTime = 0;

    //    //����
    //    Vector2 dir = closestTargetPoint - (Vector2)transform.position;
    //    rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);
    //}


    //#endregion

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


    



}
