using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_Roller : EA_Ground
{
    [Header("Roller")]
    public float chargeAttackDuration = 3.0f;
    public float chargeAttackSpeed = 10f;

    //protected override IEnumerator AttackCoroutine()
    //{
    //    //캐릭터 위치로 회전.
    //    faceRight = Vector2.SignedAngle(transform.up, brain.playerDirection) < 0 ? true : false;
    //    FlipToDirectionView();

    //    yield return StartCoroutine(DelayRoutine(preAttackDelay));

    //    //돌진 공격
    //    StartCoroutine(ChargeAttack(chargeAttackDuration));
    //    yield return StartCoroutine(DelayRoutine(afterAttackDelay));
    //}

    //돌진 공격, 일정 시간동안 앞으로 돌진한다. 
    //IEnumerator ChargeAttack(float duration)
    //{
    //    int pointCounts = ppoints.Length - 1;
    //    ppoints = gravity.nearestPlanet.GetPoints(enemyHeight);

    //    closestIndex = gravity.nearestPlanet.GetClosestIndex(transform.position);
    //    nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;

    //    dirIndex = faceRight ? 1 : -1;

    //    float time = duration;

    //    while(time > 0)
    //    {
    //        time -= Time.deltaTime;

    //        Vector2 movePos = ppoints[nextIndex];
    //        Vector2 moveDir = (movePos - rb.position).normalized;
    //        float moveDist = (movePos - rb.position).magnitude;


    //        // 움직일 거리가 가까우면 타겟을 바꾼다.
    //        if (moveDist < chargeAttackSpeed * Time.fixedDeltaTime)
    //        {
    //            closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
    //            nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
    //        }

    //        // 오브젝트를 이동 방향으로 이동
    //        rb.MovePosition(rb.position + moveDir * chargeAttackSpeed * Time.fixedDeltaTime);

    //        yield return null;
    //    }


        
    //}
}
