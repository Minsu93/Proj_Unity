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

        transform.rotation = rot;

        //������ ȸ����Ų��. 
        rb.velocity = transform.right * speed;  //rb.velocity�� ���� ���Ⱚ�ΰ���.Set�� �ٷ�� ������ Get �ؼ� �ٲٴ°� �����ʴ�. 

        
    }




}
