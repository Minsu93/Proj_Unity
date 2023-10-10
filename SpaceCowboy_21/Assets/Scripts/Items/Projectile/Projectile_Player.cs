using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Player : Projectile
{
    public int reflectCount;    //반사 횟수 
                                //플레이어의 총알

    public void init(float damage, float lifeTime, float speed, int reflect)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        this.reflectCount = reflect;
        time = 0;

        activate = true;
        coll.enabled = true;
        ViewObj.SetActive(true);

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

        ProjectileViewReset();

        if (gravityOn)
        {
            projectileGravity.activate = true;
            //행성 리스트 초기화
            projectileGravity.gravityPlanets.Clear();
            projectileGravity.ResetGravityMultiplier();
        }

        projectileMovement.StartMovement(speed);

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //적에 부딪혔을 때
        if (collision.CompareTag("Enemy"))
        {
            if(collision.TryGetComponent<EnemyBrain>(out EnemyBrain brain))
                brain.DamageEvent(damage);

            if (hitDestroyOn)
            {
                //반사횟수가 0회 이하일 때 파괴된다. 
                if(reflectCount >0)
                {
                    ReflectProjectile(collision);

                }
                else
                {
                    AfterHitEvent();

                }
            }

        }
        
        //적의 방어 BlockBody 에 가로막혔을때
        else if (collision.CompareTag("BlockBody"))
        {
            //충돌시 다른 충돌 이펙트가 보인다. 
            //hiteffect는 풀링할 것.

            if (hitDestroyOn)
            {
                //반사횟수가 0회 이하일 때 파괴된다. 
                if (reflectCount > 0)
                {
                    ReflectProjectile(collision);

                }
                else
                {
                    AfterHitEvent();

                }
            }
        }

        //적의 총알에 부딪혔을 때 
        else if (collision.CompareTag("EnemyProjectile"))
        {
            if(collision.TryGetComponent(out Projectile_Enemy proj))
            {
                if (!proj.hitByProjectileOn)
                {
                    return;
                }

                proj.DamageEvent(damage, rb.velocity);
                //proj.KnockBackEvent(rb.velocity);

                if (hitDestroyOn)
                {
                    //반사횟수가 0회 이하일 때 파괴된다. 
                    if (reflectCount > 0)
                    {
                        ReflectProjectile(collision);
                    }
                    else
                    {
                        AfterHitEvent();

                    }
                }

            }

        }

        //행성과 부딪혔을 때
        else if (collision.CompareTag("Planet"))
        {
            if (!collideOnPlanet)
                return;

            if (hitDestroyOn)
            {
                //반사횟수가 0회 이하일 때 파괴된다. 
                if (reflectCount > 0)
                {
                    ReflectProjectile(collision);
                    
                }
                else
                {
                    AfterHitEvent();

                }
            }
        }

    }

    void ReflectProjectile(Collider2D other)
    {
        reflectCount--;

        float reflectionSpeed = rb.velocity.magnitude;
        //반사됨
        Vector2 normal = other.ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, normal.normalized);

        // 반사된 방향으로 힘을 가하기
        rb.velocity = reflectDirection * reflectionSpeed;
    }



}
