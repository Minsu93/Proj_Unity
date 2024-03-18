using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ProjMov_Guide : ProjectileMovement
{
    public float rotationSpeed = 1f;

    public float startTimer = 0.5f; //시작하고 몇초동안 유도 기능이 없다.
    public float rotationTime = 0f; //쏘고 나서 몇초 이후에 유도기능이 사라진다
    public float distanceLimit = 0f;
    float stimer;
    float timer;
    bool chaseActivate = false;
    bool activate = false;
    Transform playerTr;

    public override void StartMovement(float speed)
    {
        this.speed = speed;
        activate = true;

        if(startTimer > 0)
        {
            stimer = startTimer;

        }else
        {
            chaseActivate = true;

        }

        timer = rotationTime;
        playerTr = GameManager.Instance.player;
        //chaseActivate = false;
        //속도 지정
        //rb.velocity = transform.right * speed;
    }

    public override void StopMovement()
    {
        activate = false;
        rb.velocity = Vector2.zero;
    }

    public override void Update()
    {
        if (!activate)
            return; 

        if (rotateBySpeed)
        {
            RotateBySpeed();
        }
    }

    public void FixedUpdate()
    {
        if (!activate)
            return;

        if (_knockBackTime > 0 && knockBackOn)
        {
            _knockBackTime -= Time.deltaTime;
            if (_knockBackTime <= 0)
            {
                rb.velocity = Vector2.zero;

                knockBackOn = false;
            }
        }

        //시작하고 몇 초후에 유도기능이 켜진다. 
        if(stimer > 0)
        {
            stimer -= Time.deltaTime;
            if(stimer <= 0)
            {
                chaseActivate = true;
            }
        }

        //쏘고 나서 몇초 후에 유도를 정지한다. 만약 rotationTimer수치가 0이라면 계속 쫓는다. 
        if (rotationTime > 0 && chaseActivate)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                chaseActivate = false;                
            }
        }


        //플레이어에 어느정도 가까워지면 유도기능을 멈추면  어떨까? 만약 rotationTimer수치가 0이라면 계속 쫓는다. 
        if (distanceLimit > 0 && chaseActivate)
        {
            float targetDist = (playerTr.position - transform.position).magnitude;
            if (targetDist < distanceLimit)
            {
                //유도를 정지한다
                chaseActivate = false;
            }
        }

        if(chaseActivate)
        {
            //회전값을 구한다. 
            Vector3 targetVec = (playerTr.position - transform.position).normalized;
            Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
            Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

            //방향을 회전시킨다. 
            transform.rotation = rot;
            //rb.SetRotation(rot);
        }
        
        //움직인다
        rb.velocity = transform.right * speed; 

        
    }

    public override void KnockBackEvent(Vector2 objVel)
    {
        knockBackOn = true;
        _knockBackTime = knockBackTime;



        //총알 속도의 벡터를 구함
        Vector2 negativeVel = objVel.normalized;
        negativeVel *= knockBackForce;

        rb.velocity = negativeVel;
        //rb.AddForce(negativeVel, ForceMode2D.Impulse);

        //StartMovement(speed);
    }




}
