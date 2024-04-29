using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;

public class EnemyChase_Ground : EnemyChase
{
    //점프 
    public float jumpForce = 10f;
    bool shouldJump;  //점프 준비

    int targetIndex;
    int closestIndex;
    int dirIndex;

    float mTimer;

    Vector2 closestTargetPoint;
    protected Vector2[] ppoints; 

    EA_Ground action_Ground;
    EnemyBrain brain;
    CharacterGravity charGravity;

    protected override void Awake()
    {
        base.Awake();
        brain = GetComponent<EnemyBrain>();
        charGravity = GetComponent<CharacterGravity>();
        action_Ground = GetComponent<EA_Ground>();
    }

    public override void OnChaseAction()
    {
        //행성이 바뀌면 ppoint 업데이트
        if (curPlanet != charGravity.nearestPlanet)
        {
            curPlanet = charGravity.nearestPlanet;
            if (curPlanet != null)
            {
                ppoints = curPlanet.GetPoints(action_Ground.enemyHeight);
                TargetUpdate();
            }
        }

        if (curPlanet == null) return;

        //0.5초마다 타겟 업데이트
        if (mTimer > 0) mTimer -= Time.deltaTime;
        else
        {
            mTimer = 0.5f;
            TargetUpdate();
        }

        //이동
        if (!shouldJump)
        {
            if (MoveToTarget()) JumpToPlanet();
        }
        else
        {
            MoveToTarget();
        }

    }

    void TargetUpdate()
    {
        if (brain.OnOtherPlanetCheck())
        {
            shouldJump = false;
            PrepareJump();
        }
        else
        {
            shouldJump = true;
            PrepareChase();
        }
    }


    //같은 행성에 있을 때 추격 준비
    void PrepareChase()
    {
        ////플레이어가 보이는 가장 가까운 point로 이동한다. 
        int pointCounts = ppoints.Length - 1;

        //플레이어 위치를 구한다
        targetIndex = curPlanet.GetClosestIndex(brain.playerTr.position);

        //현재와 다음 인덱스 위치를 구한다. 
        closestIndex = curPlanet.GetClosestIndex(transform.position);

        //방향 index를 구한다.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;
        dirIndex = positive < negative ? 1 : -1;            //+방향(positive)이 가까우면 1, -방향(negative)이 가까우면 -1

        //이동 방향을 바라본다.
        action_Ground.faceRight = dirIndex > 0f ? true : false;
        action_Ground.FlipToDirectionView();

    }


    //targetIndex에 가까워 졌나요? T/F
    bool MoveToTarget()
    {
        int pointCounts = ppoints.Length - 1;

        Vector2 movePos = ppoints[(closestIndex + dirIndex + pointCounts) % pointCounts];
        Vector2 moveDir = (movePos - rb.position).normalized;
        float moveDist = (movePos - rb.position).magnitude;

        // 움직일 거리가 거의 가까워졌으면 타겟을 바꾼다.
        if (moveDist < moveSpeed * Time.fixedDeltaTime)
        {
            closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
        }

        // 목적지에 가까이 도착하면 종료한다.
        if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < 0.1f)
        {
            return true;
        }

        // 오브젝트를 이동 방향으로 이동
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        return false;
    }


    void PrepareJump()
    {
        int pointCounts = ppoints.Length - 1;
        Planet playerPlanet = GameManager.Instance.playerNearestPlanet;

        if (!curPlanet.GetjumpPoint(playerPlanet, out PlanetBridge _bridge))
        {
            return;
        }

        targetIndex = _bridge.bridgeIndex;
        closestTargetPoint = _bridge.targetVector;
        closestIndex = curPlanet.GetClosestIndex(transform.position);

        //방향 index를 구한다.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;
        dirIndex = positive < negative ? 1 : -1;

        //이동 방향을 바라본다.
        action_Ground.faceRight = dirIndex > 0f ? true : false;
        action_Ground.FlipToDirectionView();
    }


    void JumpToPlanet()
    {
        action_Ground.onAir = true;
        action_Ground.lastJumpTime = Time.time;
        action_Ground.airTime = 0;

        //점프
        Vector2 dir = closestTargetPoint - (Vector2)transform.position;
        rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);
    }

}
