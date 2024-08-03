using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Reflect : Projectile_Player
{
    protected int reflectCount;    //�ݻ� Ƚ��

    protected override void HitEvent(ITarget target, IHitable hitable)
    {
        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        //�ݻ�����
        if (reflectCount > 0)
        {
            reflectCount--;

            ReflectProjectile(target.GetCollider());
            return;
        }

        AfterHitEvent();


    }

    protected override void NonHitEvent(ITarget target)
    {

        ShowHitEffect(nonHitEffect);

        //�ݻ�����
        if (target != null && reflectCount > 0)
        {
            reflectCount--;

            ReflectProjectile(target.GetCollider());
            return;
        }

        AfterHitEvent();

    }

    void ReflectProjectile(Collider2D other)
    {
        float reflectionSpeed = rb.velocity.magnitude;
        Vector2 normal = other.ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, normal.normalized);

        rb.velocity = reflectDirection * reflectionSpeed;
    }
}
