using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChase_Air : EnemyChase
{
    /// ���� ������ �÷��̾� �������� õõ�� �̵��Ѵ�. 
    /// ����ó�� �̵� > ���� > �̵� > ���� ��
    /// �����̱� ���� �÷��̾� ��ġ�� �����ϰ� Path�� �׸���. 
    /// Path�� ������ �������� �ʴ´�. 
    /// path�� waypoint�� �ʹ� ������ ���� ��������Ʈ�� �Ѿ��. 
    /// ������ ������ path�� �����Ѵ�.
    /// 
    /// 
    public AnimationCurve moveCurve;
    public float moveDistPerRow = 2f;
    public float moveDuration = 1f;
    public float delayDuration = 0.5f;

    public float nextWayPointDistance = 0.5f;
    public float randomTargetPointDistance = 5f;
    int currentWaypoint = 0;
    bool isSwim = false;

    Seeker seeker;
    Path path;
    Coroutine moveRoutine;

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<Seeker>();
    }

    public override void OnChaseAction()
    {
        if(!isSwim)
        {
            //1. path����� �����Ѵ�
            isSwim = true;

            //2. path����� ������ OnPathComplete �ݹ��ؼ� �̵��� �����Ѵ�. �÷��̾� �ֺ� ���� ��ġ.
            Vector2 endPoint = GameManager.Instance.player.position;
            Vector2 ranPoint = UnityEngine.Random.insideUnitCircle * randomTargetPointDistance;
            endPoint += ranPoint;

            seeker.StartPath(rb.position, endPoint, OnPathComplete);
        }
    }


    //����� ������ path�� ������ ������ path�� �����ϰ� ���� waypoint�� 0���� �ʱ�ȭ�Ѵ�. 
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
        }

        if (path != null)
        {
            StartCoroutine(AirSwimRoutine());
        }

    }

    IEnumerator AirSwimRoutine()
    {
        Vector2 startPoint = transform.position;
        Vector2 targetPoint;
        float distance;
        currentWaypoint = 0;

        rb.velocity = Vector2.zero;

        //��ǥ���� ����
        do
        {
            if (currentWaypoint >= path.vectorPath.Count)
            {
                yield break;
            }
            targetPoint = path.vectorPath[currentWaypoint]; 
            distance = Vector2.Distance(rb.position, targetPoint);
            currentWaypoint++;
        } 
        while (distance < nextWayPointDistance);

        //�̵�
        Vector2 dir = (targetPoint - rb.position).normalized;
        Vector2 MovePoint = startPoint + (dir * moveDistPerRow);
        float time = 0;
        while(time < moveDuration)
        {
            time += Time.deltaTime ;
            rb.MovePosition(Vector2.Lerp(startPoint, MovePoint, moveCurve.Evaluate(time / moveDuration)));
            
            yield return null;
        }

        //���
        yield return new WaitForSeconds(delayDuration);

        moveRoutine = null;
        isSwim = false;
    }


    public void StopSwim()
    {
        if (moveRoutine != null) 
        { 
            StopCoroutine(moveRoutine); 
        }
        isSwim = false;
        path = null;
    }


}
