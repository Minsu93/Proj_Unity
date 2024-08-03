using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Proj_Bomb_Basic : Projectile_Player
{
    [Header("Bomb")]
    [SerializeField] private float explodeRadius;   //폭발 반경
    [SerializeField] private LayerMask targetLayer; //대상


    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;

        if (lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();

        PM_Bomb bombMovement = projectileMovement as PM_Bomb;
        if (bombMovement != null)
        {
            bombMovement.BombExplodeEvent -= Explode;
            bombMovement.BombExplodeEvent += Explode;
            bombMovement.StartMovement(speed,distance);
        }
        

    }
    public virtual void Explode(Vector2 pos)
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
