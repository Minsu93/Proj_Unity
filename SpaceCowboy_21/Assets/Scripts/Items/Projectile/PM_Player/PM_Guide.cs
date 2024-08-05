using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_Guide : ProjectileMovement
{
    [SerializeField] int guideAmount = 1;
    [SerializeField] float guideRadius = 5f;
    [SerializeField] float rotationSpeed = 1f;
    float rspd;

    int targetLayer ;
    bool activate = false;
    Transform targetTr;

    protected override void Awake()
    {
        base.Awake();
        targetLayer = 1 << LayerMask.NameToLayer("Enemy");
    }

    //���� �Ѿ��� ���
    public override void StartMovement(float speed)
    {
        //���� ON
        this.speed = speed;
        activate = true;
        targetTr = null;
        rspd = rotationSpeed * guideAmount;

        //�����δ�
        rb.velocity = transform.right * speed;

    }


    public override void StopMovement()
    {
        if (activate) activate = false;
        rb.velocity = Vector2.zero;

    }


    public void FixedUpdate()
    {
        if (!activate)
            return;

        if (targetTr == null)
        {
            targetTr = chaseNearEnemy();
        }

        if (targetTr)
        {
            //ȸ������ ���Ѵ�. 
            Vector3 targetVec = (targetTr.position - transform.position).normalized;
            Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
            Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rspd);

            //������ ȸ����Ų��. 
            rb.SetRotation(rot);

            rb.velocity = transform.right * speed;
        }

    }

    Transform chaseNearEnemy()
    {
        Transform targetTr = null;

        RaycastHit2D targetHit = Physics2D.CircleCast(transform.position, guideRadius, Vector2.zero, 0, targetLayer);

        if (targetHit.collider != null)
        {
            targetTr = targetHit.transform;
        }

        return targetTr;
    }
}