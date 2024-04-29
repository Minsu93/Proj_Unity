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
    //점프 
    public float jumpForce = 10f;

    //공중
    public float maxAirTime = 1.0f;
    public float airTime;
    public float lastJumpTime; 

    protected override void Update()
    {
        //공중 체크 추가
        CheckOnAir();

        EnemyState currState = brain.enemyState;

        if (currState != preState)
        {
            DoAction(currState);
            preState = currState;
        }

        //공중 회전 추가
        if(currState != EnemyState.Strike) RotateCharacterToGround();

        if (!activate) return;

        //공중 체크 추가
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

        //normal과 transform.upward 사이의 값 차이가 크면 보정을 가해준다. 
        float rotateAngle = Vector2.Angle(transform.up, normal);
        turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);
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
        }

        //공중에 오래 있으면 지면으로 강제 착륙시킨다. 

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


    



}
