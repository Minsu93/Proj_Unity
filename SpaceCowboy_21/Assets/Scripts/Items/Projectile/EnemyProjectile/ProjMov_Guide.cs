using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ProjMov_Guide : ProjectileMovement
{
    public float rotationSpeed = 1f;
    bool activate = false;

    public override void StartMovement(float speed)
    {
        this.speed = speed;
        activate = true;
    }

    public override void StopMovement()
    {
        activate = false;
        rb.velocity = Vector2.zero;
    }

    public void FixedUpdate()
    {
        if (!activate)
            return;

        //플레이어의 위치를 가져온다. 
        Transform playerTr = GameManager.Instance.player;

        //플레이어에 어느정도 가까워지면 속도가 느려지게 하면 어떨까?
        float targetDist = (playerTr.position - transform.position).magnitude;
        if(targetDist < 3f)
        {
            //점점 느려진다
        }
        else
        {
            //점점 빨라진다
        }

        //회전값을 구한다. 
        Vector3 targetVec = (playerTr.position - transform.position).normalized;
        Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
        Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
        Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        transform.rotation = rot;

        //방향을 회전시킨다. 
        rb.velocity = transform.right * speed;  //rb.velocity는 로컬 방향값인가봄.Set은 다루기 쉬워도 Get 해서 바꾸는건 쉽지않다. 

        
    }




}
