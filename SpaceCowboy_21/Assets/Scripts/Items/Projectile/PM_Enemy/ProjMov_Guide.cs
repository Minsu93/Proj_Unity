using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ProjMov_Guide : ProjectileMovement
{
    public float rotationSpeed = 1f;

    public float startTimer = 0.5f; //�����ϰ� ���ʵ��� ���� ����� ����.
    public float rotationTime = 0f; //��� ���� ���� ���Ŀ� ��������� �������
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
        //�ӵ� ����
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

        //�����ϰ� �� ���Ŀ� ��������� ������. 
        if(stimer > 0)
        {
            stimer -= Time.deltaTime;
            if(stimer <= 0)
            {
                chaseActivate = true;
            }
        }

        //��� ���� ���� �Ŀ� ������ �����Ѵ�. ���� rotationTimer��ġ�� 0�̶�� ��� �Ѵ´�. 
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


        //�÷��̾ ������� ��������� ��������� ���߸�  ���? ���� rotationTimer��ġ�� 0�̶�� ��� �Ѵ´�. 
        if (distanceLimit > 0 && chaseActivate)
        {
            float targetDist = (playerTr.position - transform.position).magnitude;
            if (targetDist < distanceLimit)
            {
                //������ �����Ѵ�
                chaseActivate = false;
            }
        }

        if(chaseActivate)
        {
            //ȸ������ ���Ѵ�. 
            Vector3 targetVec = (playerTr.position - transform.position).normalized;
            Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
            Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

            //������ ȸ����Ų��. 
            transform.rotation = rot;
            //rb.SetRotation(rot);
        }
        
        //�����δ�
        rb.velocity = transform.right * speed; 

        
    }

    public override void KnockBackEvent(Vector2 objVel)
    {
        knockBackOn = true;
        _knockBackTime = knockBackTime;



        //�Ѿ� �ӵ��� ���͸� ����
        Vector2 negativeVel = objVel.normalized;
        negativeVel *= knockBackForce;

        rb.velocity = negativeVel;
        //rb.AddForce(negativeVel, ForceMode2D.Impulse);

        //StartMovement(speed);
    }




}
