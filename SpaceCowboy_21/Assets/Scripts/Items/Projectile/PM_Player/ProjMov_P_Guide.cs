using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjMov_P_Guide : ProjectileMovement
{
    public float guideRadius = 5f;
    public float rotationSpeed = 1f;
    int targetLayer;

    bool activate = false;

    protected override void Awake()
    {
        base.Awake();

        targetLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyHitableProj") | 1 << LayerMask.NameToLayer("Trap");
    }

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

    public void Update()
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

        Transform t = chaseNearEnemy();

        if (t)
        {
            //회전값을 구한다. 
            Vector3 targetVec = (t.position - transform.position).normalized;
            Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
            Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

            //방향을 회전시킨다. 
            //transform.rotation = rot;
            rb.SetRotation(rot);
        }


        //움직인다
        rb.velocity = transform.right * speed;

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
