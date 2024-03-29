using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile_Player : Projectile
{
    [Tooltip("총알이 화면 밖으로 얼마나 나가도 되는 지 여유분")]
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

        //초기화 이벤트   >>view나 Gravity모두에게 전달
        InitProjectile();

        //if (gravityOn)
        //{
        //    projectileGravity.activate = true;
        //    //행성 리스트 초기화
        //    projectileGravity.gravityPlanets.Clear();
        //}

        projectileMovement.StartMovement(speed);
    }


    protected override void Update()
    {
        base.Update();

        if (!activate)
            return;

        //총알이 화면 밖으로 나갔는지 체크
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

        //적에 부딪혔을 때
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

        //행성과 부딪혔을 때
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

    //총알이 화면 밖으로 나갔을 때 
    bool OutSideScreenBorder()
    {
        //화면 밖으로 나갔는지 체크
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
