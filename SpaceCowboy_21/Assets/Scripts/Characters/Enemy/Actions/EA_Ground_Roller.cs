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
    //    //ĳ���� ��ġ�� ȸ��.
    //    faceRight = Vector2.SignedAngle(transform.up, brain.playerDirection) < 0 ? true : false;
    //    FlipToDirectionView();

    //    yield return StartCoroutine(DelayRoutine(preAttackDelay));

    //    //���� ����
    //    StartCoroutine(ChargeAttack(chargeAttackDuration));
    //    yield return StartCoroutine(DelayRoutine(afterAttackDelay));
    //}

    //���� ����, ���� �ð����� ������ �����Ѵ�. 
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


    //        // ������ �Ÿ��� ������ Ÿ���� �ٲ۴�.
    //        if (moveDist < chargeAttackSpeed * Time.fixedDeltaTime)
    //        {
    //            closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
    //            nextIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
    //        }

    //        // ������Ʈ�� �̵� �������� �̵�
    //        rb.MovePosition(rb.position + moveDir * chargeAttackSpeed * Time.fixedDeltaTime);

    //        yield return null;
    //    }


        
    //}
}
