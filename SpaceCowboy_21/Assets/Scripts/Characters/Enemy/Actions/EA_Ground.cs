using SpaceCowboy;
using System.Collections;
using UnityEngine;

public class EA_Ground : EnemyAction
{
    //점프 
    public float jumpForce = 10f;

    //공중
    public float maxAirTime = 1.0f;
    public float airTime {get; set;}
    public float lastJumpTime { get; set;} 

    //override 되는 부분

    protected override bool BeforeUpdate()
    {
        if (!activate) return true;

        //공중 체크 
        CheckOnAir();
        //공중 회전
        RotateCharacterToGround();
        if (onAir) return true;

        return false;
    }

    public override void BrainStateChange()
    {
        //상태에 따른 활동 
        switch (enemyState)
        {
            case EnemyState.Idle:
                //플레이어가 죽지 않았다면, 바로 추격한다.
                if (GameManager.Instance.playerIsAlive)
                    enemyState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (brain.inAttackRange && brain.isVisible)
                    enemyState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                if (!brain.inAttackRange || !brain.isVisible)
                    enemyState = EnemyState.Chase;

                break;
        }
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        EnemyPause(1f);

        if (onAir)
        {
            Vector2 dir = (Vector2)transform.position - hitPos;
            dir = dir.normalized;

            rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);
        }
        else
        {
            //hit포인트가 좌/우 어디에 있는지 검사 후 반대 방향으로 튕겨나간다. 
            int hitIndex = Vector2.SignedAngle(transform.up, hitPos - (Vector2)transform.position) > 0 ? 1 : -1;

            Vector2 dir = (transform.right * hitIndex + transform.up) * 0.71f;

            rb.velocity = Vector2.zero;
            rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            onAir = true;
            airTime = 0;
            lastJumpTime = Time.time;
        }

    }

    protected override IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHeight, dir.normalized, float.MaxValue, LayerMask.GetMask("Planet"));
        if (hit.collider == null) yield break;

        Vector2 strikeStartPos = transform.position;
        Vector2 normal = (strikeStartPos - hit.point).normalized;
        Vector2 strikeTargetPos = hit.point + (normal * (distanceFromStrikePoint + enemyHeight));

        float strikeTime = (strikeStartPos - strikeTargetPos).magnitude / strikeSpeed;
        float time = 0; //강습 시간
        while (time < strikeTime)
        {
            time += Time.deltaTime;
            rb.MovePosition(Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }
        //착지하면 활동 시작. 
        yield return new WaitForSeconds(0.5f);
        AfterStrikeEvent();
    }

    //추가된 부분

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





    //#region  
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
