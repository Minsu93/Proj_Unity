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
    Transform playerTr;

    //시작 시 추적 시작.
    public override void StartMovement(float speed)
    {
        this.speed = speed;
        rb.velocity = Vector2.zero;
        playerTr = GameManager.Instance.player;
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
        if(playerTr == null) return;

        //회전값을 구한다. 
        Vector3 targetVec = (playerTr.position - transform.position).normalized;
        Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
        Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
        Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        //방향을 회전시킨다. 
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);

        //움직인다
        rb.MovePosition(rb.position + ((Vector2)transform.right * speed * Time.deltaTime)); 
        
    }

}
