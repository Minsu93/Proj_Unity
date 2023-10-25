using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLassoChecker : MonoBehaviour
{
    public GravityLasso lasso { get; set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (lasso == null)
            return;

        if (collision.CompareTag("Planet"))
        {
            lasso.TriggerByBig(null);
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (enemy.isGrabable)
                {
                    lasso.TriggerByMedium(collision);
                }
                else
                {
                    lasso.TriggerByBig(collision);

                }
            }
        }
        else if (collision.CompareTag("EnemyProjectile"))
        {
            if(collision.TryGetComponent<Projectile>(out Projectile proj))
            {
                if (proj.hitByProjectileOn)
                {
                    lasso.TriggerByMedium(collision);
                }
            }
            
        }

    }
}
