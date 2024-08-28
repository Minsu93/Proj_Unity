using System.Collections.Generic;
using UnityEngine;

public class EnemyChase_Ground : EnemyChase
{
    //점프 
    public float jumpForce = 10f;
    bool shouldJump = false;  //점프 준비
    bool shouldChase;

    int targetIndex;
    int closestIndex;
    int dirIndex;

    float mTimer;

    Vector2 closestTargetPoint;
    protected Vector2[] ppoints;
    Planet prePlayerPlanet;
    Planet passedPlanet;//지나온 행성, 내 행성이 바뀌었을 때 수정된다. 

    EA_Ground action_Ground;
    EnemyBrain brain;
    CharacterGravity charGravity;

    //debug

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
            passedPlanet = curPlanet;
            curPlanet = charGravity.nearestPlanet;
            if (curPlanet != null)
            {
                ppoints = curPlanet.GetPoints(action_Ground.enemyHeight);
                TargetUpdate();
            }
        }

        //우주에 떠 있거나, 플레이어가 nearestPlanet이 없을 때는 추적하지 않는다. 
        if (curPlanet == null) return;
        if (WaveManager.instance.playerNearestPlanet == null) return; 

        //0.5초마다 타겟 업데이트
        if (mTimer > 0) mTimer -= Time.deltaTime;
        else
        {
            TargetUpdate();
        }

        //추격이 불가능하면 추격하지 않는다. 
        if (!shouldChase) return;

        //이동
        if (shouldJump)
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
        mTimer = 0.5f;

        if (brain.OnOtherPlanetCheck())
        {
            shouldJump = true;
            shouldChase = PrepareJump();
        }
        else
        {
            shouldJump = false;
            PrepareChase();
            shouldChase = true;
        }
    }


    //같은 행성에 있을 때 추격 준비
    void PrepareChase()
    {
        int pointCounts = ppoints.Length - 1;

        //플레이어 위치와 가장 가까운 index를 구한다 = target
        targetIndex = curPlanet.GetClosestIndex(GameManager.Instance.player.position);

        //현재 가장 가까운 위치를 구한다.
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
        if (action_Ground.onAir) return false;

        int pointCounts = ppoints.Length - 1;
        float moveDist;
        Vector2 moveDir;
        do
        {
            int targetPoint = (closestIndex + dirIndex + pointCounts) % pointCounts;
            Vector2 movePos = ppoints[targetPoint];
            moveDir = (movePos - rb.position).normalized;
            moveDist = (movePos - rb.position).magnitude;

            if (moveDist < moveSpeed * Time.fixedDeltaTime)
            {
                closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
            }
        }
        while (moveDist < moveSpeed * Time.fixedDeltaTime);

        // 목적지에 가까이 도착하면 종료한다.
        if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < 0.1f)
        {
            return true;
        }

        // 오브젝트를 이동 방향으로 이동
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        return false;
    }


    bool PrepareJump()
    {
        int pointCounts = ppoints.Length - 1;
        Planet playerPlanet = WaveManager.instance.playerNearestPlanet;

        //점프할 행성을 구한다 
        Planet targetPlanet = ChoosePlanet(playerPlanet, curPlanet);
        if (targetPlanet == null) return false;

        //점프 포인트를 구한다. 
        if (!curPlanet.GetjumpPoint(targetPlanet, out PlanetBridge _bridge)) return false;
  
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

        return true;
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


    Planet ChoosePlanet(Planet playerPlanet, Planet curPlanet)
    {
        List<Planet> playerLinkedPlanets = new List<Planet>();
        List<Planet> curLinkedPlanets = new List<Planet>();
        List<Planet> commonPlanets = new List<Planet> ();
        Planet targetPlanet = null;

        //플레이어의 행성이 바뀌었을 때 
        if (prePlayerPlanet != playerPlanet)
        {
            passedPlanet = null;
            prePlayerPlanet = playerPlanet;
        }

        foreach (PlanetBridge bridge in playerPlanet.linkedPlanetList)
        {
            playerLinkedPlanets.Add(bridge.planet);
        }
        foreach (PlanetBridge bridge in curPlanet.linkedPlanetList)
        {
            curLinkedPlanets.Add(bridge.planet);
        }

        //두개의 List에 겹치는 것만 골라낸다. 
        foreach (Planet planet in playerLinkedPlanets)
        {
            if (curLinkedPlanets.Contains(planet))
            {
                commonPlanets.Add(planet);
            }
        }

        //플레이어 행성을 추가한다. 
        //commonPlanets.Add(playerPlanet);

        //지나온 행성을 제거한다.
        if (passedPlanet!= null && commonPlanets.Contains(passedPlanet))
        {
            commonPlanets.Remove(passedPlanet);
        }

        //리스트에서 랜덤하게 선택해서 진행한다. 

        if (commonPlanets.Count > 0)
        {
            switch(Random.Range(0, 2))
            {
                //플레이어 행성으로 직행
                case 0:
                    targetPlanet = playerPlanet;
                    break;
                //예측 행성으로 이동
                default:
                    targetPlanet = commonPlanets[Random.Range(0, commonPlanets.Count - 1)];
                    break;
            }
        }
        else
        {
            targetPlanet = playerPlanet;
        }
        
        return targetPlanet;
        
    }

}


