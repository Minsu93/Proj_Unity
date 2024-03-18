 using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    public bool hitByProjectileOn = false; 
    protected Health health;

    protected override void Awake()
    {
        base.Awake();

        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    public override void init(float damage, float speed, float range, float lifeTime)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.lifeTime = lifeTime;
        startPos = transform.position;

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

        projectileMovement.StartMovement(speed);

        //Projectile 체력 초기화
        if (health != null)
        {   
            health.ResetHealth();
        }
    }

    //총알 수명 기간 동안에
    protected override void Update()
    {
        base.Update();
    }

    //총알 수명이 끝났을 때 
    protected override void LifeTimeOver()
    {
        AfterHitEvent();
    }

    //총알이 뭔가에 맞았을 때 
    protected override void AfterHitEvent()
    {
        base.AfterHitEvent();
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //총알이 충돌했을 때 
        if (collision.CompareTag("Player"))
        {
            if(collision.TryGetComponent(out PlayerBehavior behavior))
            {
                //플레이어가 죽으면 통과
                if (behavior.state == PlayerState.Die)
                    return;

                //데미지 전달
                behavior.DamageEvent(damage);

                ShowHitEffect();
                AfterHitEvent();
            }
        }

        else if (collision.CompareTag("Obstacle"))
        {
            if (collision.TryGetComponent(out Obstacle obstacle))
            {
                obstacle.AnyDamage(damage, rb.velocity);
                ShowHitEffect();
                AfterHitEvent();
            }
        }

        //행성과 부딪혔을 때
        else if (collision.CompareTag("Planet")|| collision.CompareTag("SpaceBorder"))
        {
            ShowHitEffect();
            AfterHitEvent();
        }

    }



    // 총알 본인이 맞았을 때 

    public virtual void DamageEvent(float dmg, Vector2 objVel)
    {
        //피격시 발동하는 이벤트

        if (!hitByProjectileOn)     //projectile-player 과 더블체크이긴 하다. (필요없음)
            return;
        if (health == null) return;

        if (health.AnyDamage(dmg))  //어떠한 피해를 입었으면 true
        {
            HitFeedBack();   //Projectile 에 있다. View 의 HitFeedback을 실행시킨다.
        }

        if (health.IsDead())
        {
            //스스로를 파괴
            AfterHitEvent();
        }
        else
        {
            KnockBackEvent(objVel);
        }

    }


    void KnockBackEvent(Vector2 objVel)
    {
       projectileMovement.KnockBackEvent(objVel);
    }

}
