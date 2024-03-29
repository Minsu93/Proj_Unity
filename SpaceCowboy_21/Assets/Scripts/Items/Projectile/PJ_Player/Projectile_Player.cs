using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile_Player : Projectile
{
    [Tooltip("�Ѿ��� ȭ�� ������ �󸶳� ������ �Ǵ� �� ������")]
    public float screenBorderLimit = 5.0f;
    int reflectCount;

    public override void init(float damage, float speed, float range, float lifeTime)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.lifeTime = lifeTime;
        startPos = transform.position;
        
        infiniteLifeTime = false;
        if (lifeTime == 0)
            infiniteLifeTime = true;
 
        activate = true;
        coll.enabled = true;
        ViewObj.SetActive(true);

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

        //�ʱ�ȭ �̺�Ʈ   >>view�� Gravity��ο��� ����
        InitProjectile();

        //if (gravityOn)
        //{
        //    projectileGravity.activate = true;
        //    //�༺ ����Ʈ �ʱ�ȭ
        //    projectileGravity.gravityPlanets.Clear();
        //}

        projectileMovement.StartMovement(speed);
    }


    protected override void Update()
    {
        base.Update();

        if (!activate)
            return;

        //�Ѿ��� ȭ�� ������ �������� üũ
        if (OutSideScreenBorder())
        {
            AfterHitEvent();
        }

    }

    protected override void LifeTimeOver()
    {
        AfterHitEvent();
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activate)
            return; 

        //���� �ε����� ��
        if (collision.CompareTag("Enemy"))
        {
            if(collision.transform.parent.TryGetComponent(out EnemyBrain enemy))
            {
                enemy.DamageEvent(damage);
                ShowHitEffect();
            }
            ReflectCheck(collision);
        }

        else if (collision.CompareTag("EnemyHitableProjectile"))
        {
            if(collision.transform.parent.TryGetComponent(out Projectile_Enemy proj))
            {
                proj.DamageEvent(damage, rb.velocity);
                ShowHitEffect();
            }
            ReflectCheck(collision);

        }

        //�༺�� �ε����� ��
        else if (collision.CompareTag("Planet") || collision.CompareTag("SpaceBorder"))
        {
            ShowHitEffect();
            ReflectCheck(collision);
        }

        else if (collision.CompareTag("Obstacle"))
        {
            if (collision.TryGetComponent(out Obstacle obstacle))
            {
                obstacle.AnyDamage(damage, rb.velocity);
                ShowHitEffect();
            }
            ReflectCheck(collision);
        }

    }


    void ReflectCheck(Collider2D collision)
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

    //�Ѿ��� ȭ�� ������ ������ �� 
    bool OutSideScreenBorder()
    {
        //ȭ�� ������ �������� üũ
        bool outside = false;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 - screenBorderLimit)
        {
            outside = true;
        }
        else if (screenPosition.x > Camera.main.pixelWidth + screenBorderLimit )
        {
            outside = true;
        }

        if (screenPosition.y < 0 - screenBorderLimit)
        {
            outside = true;

        }
        else if (screenPosition.y > Camera.main.pixelHeight + screenBorderLimit)
        {
            outside = true;
        }

        return outside;
    }
}
