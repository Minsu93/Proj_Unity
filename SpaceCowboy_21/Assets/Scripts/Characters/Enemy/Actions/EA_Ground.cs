using SpaceCowboy;
using System.Collections;
using UnityEngine;

public class EA_Ground : EnemyAction
{
    //���� 
    public float jumpForce = 10f;

    //����
    public float maxAirTime = 1.0f;
    public float airTime {get; set;}
    public float lastJumpTime { get; set;} 

    //override �Ǵ� �κ�

    protected override bool BeforeUpdate()
    {
        if (!activate) return true;

        //���� üũ 
        CheckOnAir();
        //���� ȸ��
        RotateCharacterToGround();
        if (onAir) return true;

        return false;
    }

    public override void BrainStateChange()
    {
        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            case EnemyState.Idle:
                //�÷��̾ ���� �ʾҴٸ�, �ٷ� �߰��Ѵ�.
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
            //hit����Ʈ�� ��/�� ��� �ִ��� �˻� �� �ݴ� �������� ƨ�ܳ�����. 
            int hitIndex = Vector2.SignedAngle(transform.up, hitPos - (Vector2)transform.position) > 0 ? 1 : -1;

            Vector2 dir = (transform.right * hitIndex + transform.up) * 0.71f;

            rb.velocity = Vector2.zero;
            rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
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
        float time = 0; //���� �ð�
        while (time < strikeTime)
        {
            time += Time.deltaTime;
            rb.MovePosition(Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }
        //�����ϸ� Ȱ�� ����. 
        yield return new WaitForSeconds(0.5f);
        AfterStrikeEvent();
    }

    //�߰��� �κ�

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





    //#region  
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
