using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;

public class EnemyChase_Ground : EnemyChase
{
    //���� 
    public float jumpForce = 10f;
    bool shouldJump = false;  //���� �غ�

    int targetIndex;
    int closestIndex;
    int dirIndex;

    float mTimer;

    Vector2 closestTargetPoint;
    protected Vector2[] ppoints;
    Planet prePlayerPlanet;
    Planet passedPlanet;//������ �༺, �� �༺�� �ٲ���� �� �����ȴ�. 

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
        //�༺�� �ٲ�� ppoint ������Ʈ
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
        //���ֿ� �� �ְų�, �÷��̾ nearestPlanet�� ���� ���� �������� �ʴ´�. 
        if (curPlanet == null) return;
        if (WaveManager.instance.playerNearestPlanet == null) return; 

        //0.5�ʸ��� Ÿ�� ������Ʈ
        if (mTimer > 0) mTimer -= Time.deltaTime;
        else
        {
            mTimer = 0.5f;
            TargetUpdate();
        }

        //�̵�
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
        if (brain.OnOtherPlanetCheck())
        {
            shouldJump = true;
            PrepareJump();
        }
        else
        {
            shouldJump = false;
            PrepareChase();
        }
    }


    //���� �༺�� ���� �� �߰� �غ�
    void PrepareChase()
    {
        ////�÷��̾ ���̴� ���� ����� point�� �̵��Ѵ�. 
        int pointCounts = ppoints.Length - 1;

        //�÷��̾� ��ġ�� ���Ѵ�
        targetIndex = curPlanet.GetClosestIndex(GameManager.Instance.player.position);

        //����� ���� �ε��� ��ġ�� ���Ѵ�. 
        closestIndex = curPlanet.GetClosestIndex(transform.position);

        //���� index�� ���Ѵ�.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;
        dirIndex = positive < negative ? 1 : -1;            //+����(positive)�� ������ 1, -����(negative)�� ������ -1

        //�̵� ������ �ٶ󺻴�.
        action_Ground.faceRight = dirIndex > 0f ? true : false;
        action_Ground.FlipToDirectionView();

    }


    //targetIndex�� ����� ������? T/F
    bool MoveToTarget()
    {
        if (curPlanet != charGravity.nearestPlanet) 
            return false;

        int pointCounts = ppoints.Length - 1;

        Vector2 movePos = ppoints[(closestIndex + dirIndex + pointCounts) % pointCounts];
        Vector2 moveDir = (movePos - rb.position).normalized;
        float moveDist = (movePos - rb.position).magnitude;

        // ������ �Ÿ��� ���� ����������� Ÿ���� �ٲ۴�.
        if (moveDist < moveSpeed * Time.fixedDeltaTime)
        {
            closestIndex = (closestIndex + dirIndex + pointCounts) % pointCounts;
        }

        // �������� ������ �����ϸ� �����Ѵ�.
        if (Vector2.Distance(ppoints[targetIndex], (Vector2)transform.position) < 0.1f)
        {
            return true;
        }

        // ������Ʈ�� �̵� �������� �̵�
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        return false;
    }


    void PrepareJump()
    {
        int pointCounts = ppoints.Length - 1;
        Planet playerPlanet = WaveManager.instance.playerNearestPlanet;

        //������ �༺�� ���Ѵ� 
        Planet targetPlanet = ChoosePlanet(playerPlanet, curPlanet);
        if (targetPlanet == null) return;

        //���� ����Ʈ�� ���Ѵ�. 
        if (!curPlanet.GetjumpPoint(targetPlanet, out PlanetBridge _bridge)) return;
  
        targetIndex = _bridge.bridgeIndex;
        closestTargetPoint = _bridge.targetVector;
        closestIndex = curPlanet.GetClosestIndex(transform.position);

        //���� index�� ���Ѵ�.
        int positive = (targetIndex - closestIndex + pointCounts) % pointCounts;
        int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;
        dirIndex = positive < negative ? 1 : -1;

        //�̵� ������ �ٶ󺻴�.
        action_Ground.faceRight = dirIndex > 0f ? true : false;
        action_Ground.FlipToDirectionView();
    }


    void JumpToPlanet()
    {
        action_Ground.onAir = true;
        action_Ground.lastJumpTime = Time.time;
        action_Ground.airTime = 0;

        //����
        Vector2 dir = closestTargetPoint - (Vector2)transform.position;
        rb.AddForce(dir.normalized * jumpForce, ForceMode2D.Impulse);
    }


    Planet ChoosePlanet(Planet playerPlanet, Planet curPlanet)
    {
        List<Planet> playerLinkedPlanets = new List<Planet>();
        List<Planet> curLinkedPlanets = new List<Planet>();
        List<Planet> commonPlanets = new List<Planet> ();
        Planet targetPlanet = null;

        //�÷��̾��� �༺�� �ٲ���� �� 
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

        //�ΰ��� List�� ��ġ�� �͸� ��󳽴�. 
        foreach (Planet planet in playerLinkedPlanets)
        {
            if (curLinkedPlanets.Contains(planet))
            {
                commonPlanets.Add(planet);
            }
        }

        //�÷��̾� �༺�� �߰��Ѵ�. 
        //commonPlanets.Add(playerPlanet);

        //������ �༺�� �����Ѵ�.
        if (passedPlanet!= null && commonPlanets.Contains(passedPlanet))
        {
            commonPlanets.Remove(passedPlanet);
        }

        //����Ʈ���� �����ϰ� �����ؼ� �����Ѵ�. 

        if (commonPlanets.Count > 0)
        {
            switch(Random.Range(0, 2))
            {
                //�÷��̾� �༺���� ����
                case 0:
                    targetPlanet = playerPlanet;
                    break;
                //���� �༺���� �̵�
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


