using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLassoChecker : MonoBehaviour
{
    public GravityLasso lasso { get; set; }
    Transform playerTr;
    Vector2 playerPos;
    int targetLayer;
    public bool activate = true;

    private void Awake()
    {
        playerTr = GameManager.Instance.player;
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyHitableProj") | 1 << LayerMask.NameToLayer("Trap");
    }
    private void Update()
    {
        if (!activate)
        {
            return;
        }

        if (lasso == null)
            return;

        playerPos = playerTr.position;
        Vector2 dir = (Vector2)transform.position - playerPos;
        float dist = dir.magnitude;
        dir = dir.normalized;

        RaycastHit2D hit = Physics2D.Raycast(playerPos, dir, dist, targetLayer);
        if(hit.collider != null)
        {
            if (hit.collider.CompareTag("Planet"))
            {
                lasso.TriggerByBig(null);
                activate = false;
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                if (hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    if (enemy.isGrabable)
                    {
                        lasso.TriggerByMedium(hit.collider);
                        activate = false;

                    }
                    else
                    {
                        lasso.TriggerByBig(hit.collider);
                        activate = false;


                    }
                }
            }
            else if (hit.collider.CompareTag("EnemyHitableProjectile"))
            {
                if (hit.collider.TryGetComponent<Projectile>(out Projectile proj))
                {
                    if (proj.hitByProjectileOn)
                    {
                        lasso.TriggerByMedium(hit.collider);
                        activate = false;

                    }
                }

            }
            
        }
    }
   
}
