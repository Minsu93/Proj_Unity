using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Proj_Bomb_Basic : Projectile_Bomb
{
    [Header("Bomb")]
    [SerializeField] private float explodeRadius;   //폭발 반경
    [SerializeField] private LayerMask targetLayer; //대상

    public override void Explode(Vector2 pos)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, explodeRadius, targetLayer);
        if(colls.Length > 0 )
        {
            foreach(Collider2D coll in colls)
            {
                if (coll.TryGetComponent<IHitable>(out IHitable hitable))
                {
                    hitable.DamageEvent(damage, transform.position);
                }
            }

        }

        if (hitEffect != null)
            GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);
    }


}
