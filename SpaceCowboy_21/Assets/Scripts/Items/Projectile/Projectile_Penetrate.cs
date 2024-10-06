using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile_Penetrate : Projectile_Player
{
    List<Collider2D> hitList = new List<Collider2D>();//이미 맞은 적 리스트

    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        base.Init(damage, speed, lifeTime, distance);

        ResetHitList();
    }
    protected override void OverlapTarget(Collider2D collision)
    {

        Transform tr = collision.transform;

        if (collision.CompareTag("ProjHitCollider"))
        {
            tr = collision.transform.parent;
        }

        if (tr.TryGetComponent<ITarget>(out ITarget target))
        {
            if (tr.TryGetComponent<IHitable>(out IHitable hitable))
            {
                if (!hitList.Contains(collision))
                {
                    hitList.Add(collision);
                    HitEvent(target, hitable);
                }
            }
            else
            {
               // NonHitEvent(target);
            }

        }


    }

    protected override void HitEvent(ITarget target, IHitable hitable)
    {

        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        //AfterHitEvent();

        WeaponImpactEvent();


    }

    public void ResetHitList()
    {
        hitList.Clear();
    }
}
