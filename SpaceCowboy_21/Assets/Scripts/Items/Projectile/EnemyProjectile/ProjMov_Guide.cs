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

    protected override void Update()
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
                //_knockBackTime = 0;
                rb.velocity = Vector2.zero;

                knockBackOn = false;

            }
        }

        //�÷��̾��� ��ġ�� �����´�. 
        Transform playerTr = GameManager.Instance.player;

        //�÷��̾ ������� ��������� �ӵ��� �������� �ϸ� ���?
        float targetDist = (playerTr.position - transform.position).magnitude;
        if(targetDist < 3f)
        {
            //���� ��������
        }
        else
        {
            //���� ��������
        }

        //ȸ������ ���Ѵ�. 
        Vector3 targetVec = (playerTr.position - transform.position).normalized;
        Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
        Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
        Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        //������ ȸ����Ų��. 
        transform.rotation = rot;

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
