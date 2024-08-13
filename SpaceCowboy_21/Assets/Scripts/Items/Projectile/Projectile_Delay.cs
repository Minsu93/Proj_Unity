using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Delay : Projectile_Player
{
    [SerializeField] float delay;
    bool launch = false;
    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;

        if (lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();
        Invoke("DelayMovement", delay);
    }

    public override void ResetProjectile()
    {
        base.ResetProjectile();
        launch = false;
    }

    public void DelayMovement()
    {
        if(!launch)
        {
            launch = true;
            projectileMovement.StartMovement(speed);
        }

    }

}
