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

        //��ġ���� �̵� & �۵� 
        stopFollow = true;
        moveTargetPos = mousePos;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
     
        if (!activate) return;
                
        if (stopFollow && !useDrone)
        {
            //�Ÿ� ���
            float dist = Vector2.Distance(transform.position, moveTargetPos);
            if (dist < 0.1f)
            {
                //��ų ��¥ ����
                physicsColl.enabled = false;
                ActivateFunction();
            }
            else
            {
                //��ų ��ġ���� �̵�
                RigidBodyMoveToPosition(moveTargetPos, out _);
            }
        }
    }

    //��ġ�� �����ؼ� �����ϴ� �ൿ
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
        //�ð� üũ
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            //�� üũ
            checkTimer = 0;
            targetTr = CheckEnemyIsNear();
        }

        
        //�߻� 
        if (targetTr != null)
        {
            if (Time.time - lastShootTime > attackProperty.shootCoolTime)
            {
                //�� �ð� üũ
                lastShootTime = Time.time;

                //���
                StartCoroutine(burstShootRoutine(targetTr));
            }
        }
    }


}
