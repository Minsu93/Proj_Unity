using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Turret : DroneItem_Projectile
{
    float checkTimer;
    Vector2 moveTargetPos;


    public override void UseDrone(Vector2 mousePos, Quaternion quat)
    {
        Debug.Log("Start Use Drone");

        //위치까지 이동 & 작동 
        stopFollow = true;
        moveTargetPos = mousePos;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
     
        if (!activate) return;
                
        if (stopFollow && !useDrone)
        {
            //거리 계산
            float dist = Vector2.Distance(transform.position, moveTargetPos);
            if (dist < 0.1f)
            {
                //스킬 진짜 시작
                physicsColl.enabled = false;
                ActivateFunction();
            }
            else
            {
                //스킬 위치까지 이동
                RigidBodyMoveToPosition(moveTargetPos, out _);
            }
        }
    }

    //위치에 도착해서 실행하는 행동
    void ActivateFunction()
    {
        useDrone = true;
    }

    protected override void Update()
    {
        if (!activate) return;
        
        base.Update();

        if (useDrone)
        {
            ShootMethod();
        } 
    }


    void ShootMethod()
    {
        //시간 체크
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            //적 체크
            checkTimer = 0;
            targetTr = CheckEnemyIsNear();
        }

        
        //발사 
        if (targetTr != null)
        {
            if (Time.time - lastShootTime > attackProperty.shootCoolTime)
            {
                //쏜 시간 체크
                lastShootTime = Time.time;

                //사격
                StartCoroutine(burstShootRoutine(targetTr));
            }
        }
    }


}
