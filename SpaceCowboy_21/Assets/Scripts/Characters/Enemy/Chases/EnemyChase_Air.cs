using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyChase_Air : EnemyChase
{
    /// 공중 유닛은 플레이어 방향으로 천천히 이동한다. 
    /// 문어처럼 이동 > 정지 > 이동 > 정지 식
    /// 움직이기 전에 플레이어 위치를 감지하고 Path를 그린다. 
    /// Path가 없으면 움직이지 않는다. 
    /// path의 waypoint가 너무 가까우면 다음 웨이포인트로 넘어간다. 
    /// 움직일 때마다 path를 갱신한다.
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
            //1. path계산을 시작한다
            isSwim = true;

            //2. path계산이 끝나면 OnPathComplete 콜백해서 이동을 시작한다. 플레이어 주변 랜덤 위치.
            Vector2 endPoint = GameManager.Instance.player.position;
            Vector2 ranPoint = UnityEngine.Random.insideUnitCircle * randomTargetPointDistance;
            endPoint += ranPoint;

            seeker.StartPath(rb.position, endPoint, OnPathComplete);
        }
    }


    //계산이 끝나고 path에 에러가 없으면 path를 적용하고 현재 waypoint를 0으로 초기화한다. 
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

        //목표지점 설정
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

        //이동
        Vector2 dir = (targetPoint - rb.position).normalized;
        Vector2 MovePoint = startPoint + (dir * moveDistPerRow);
        float time = 0;
        while(time < moveDuration)
        {
            time += Time.deltaTime ;
            rb.MovePosition(Vector2.Lerp(startPoint, MovePoint, moveCurve.Evaluate(time / moveDuration)));
            
            yield return null;
        }

        //대기
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
