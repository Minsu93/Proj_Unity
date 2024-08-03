using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activate) return;

        if (collision.TryGetComponent<ITarget>(out ITarget target))
        {
            if (collision.TryGetComponent<IEnemyHitable>(out IEnemyHitable eHitable))
            {
                HitEvent(target, eHitable);
            }
            else
            {
                NonHitEvent(target);
            }

            WeaponImpactEvent();
        }
    }


    protected virtual void HitEvent(ITarget target, IEnemyHitable eHitable)
    {

        eHitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        AfterHitEvent();
    }
}
