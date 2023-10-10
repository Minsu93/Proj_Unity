using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Player : Projectile
{
    public int reflectCount;    //�ݻ� Ƚ�� 
                                //�÷��̾��� �Ѿ�

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
            //�༺ ����Ʈ �ʱ�ȭ
            projectileGravity.gravityPlanets.Clear();
            projectileGravity.ResetGravityMultiplier();
        }

        projectileMovement.StartMovement(speed);

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //���� �ε����� ��
        if (collision.CompareTag("Enemy"))
        {
            if(collision.TryGetComponent<EnemyBrain>(out EnemyBrain brain))
                brain.DamageEvent(damage);

            if (hitDestroyOn)
            {
                //�ݻ�Ƚ���� 0ȸ ������ �� �ı��ȴ�. 
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
        
        //���� ��� BlockBody �� ���θ�������
        else if (collision.CompareTag("BlockBody"))
        {
            //�浹�� �ٸ� �浹 ����Ʈ�� ���δ�. 
            //hiteffect�� Ǯ���� ��.

            if (hitDestroyOn)
            {
                //�ݻ�Ƚ���� 0ȸ ������ �� �ı��ȴ�. 
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

        //���� �Ѿ˿� �ε����� �� 
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
                    //�ݻ�Ƚ���� 0ȸ ������ �� �ı��ȴ�. 
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

        //�༺�� �ε����� ��
        else if (collision.CompareTag("Planet"))
        {
            if (!collideOnPlanet)
                return;

            if (hitDestroyOn)
            {
                //�ݻ�Ƚ���� 0ȸ ������ �� �ı��ȴ�. 
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
        //�ݻ��
        Vector2 normal = other.ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, normal.normalized);

        // �ݻ�� �������� ���� ���ϱ�
        rb.velocity = reflectDirection * reflectionSpeed;
    }



}
