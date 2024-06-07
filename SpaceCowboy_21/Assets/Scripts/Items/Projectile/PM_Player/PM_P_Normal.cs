using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_P_Normal : ProjectileMovement
{
    [SerializeField] float guideRadius = 5f;
    [SerializeField] float rotationSpeed = 1f;
    float rspd;
    
    int targetLayer;
    bool activate = false;
    Transform targetTr;

    protected override void Awake()
    {
        base.Awake();
        targetLayer = 1 << LayerMask.NameToLayer("Enemy");
    }

    //일반 총알의 경우
    public override void StartMovement(float speed)
    {
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    //유도 총알의 경우
    public void StartMovement(float speed, int guideAmount)
    {

        if (guideAmount > 0)
        {
            //유도 ON
            this.speed = speed;
            activate = true;
            targetTr = null;
            rspd = rotationSpeed * guideAmount;

            //움직인다
            rb.velocity = transform.right * speed;
        }

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

        if(targetTr == null)
        {
            targetTr = chaseNearEnemy();
        }
        //Transform t = chaseNearEnemy();

        if (targetTr)
        {
            //회전값을 구한다. 
            Vector3 targetVec = (targetTr.position - transform.position).normalized;
            Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
            Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rspd);

            //방향을 회전시킨다. 
            rb.SetRotation(rot);

            rb.velocity = transform.right * speed;
        }

        ////회전한다
        //if (rotateBySpeed)
        //{
        //    RotateBySpeed();
        //}

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
