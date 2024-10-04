using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_Turret : DroneItem_Projectile
{
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




}
