using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    Health health;

    protected override void Awake()
    {
        base.Awake();

        if (hitByProjectileOn)
        {
            health = GetComponent<Health>();
        }
    }

    public override void init(float damage, float lifeTime, float speed)
    {
        base.init(damage, lifeTime, speed); 

        if (hitByProjectileOn)
        {   //Projectile 체력 초기화
            health.ResetHealth();
        }

        ProjectileViewReset();

    }

    //적의 총알
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

                if (hitDestroyOn)
                    AfterHitEvent();

            }
        }

        //행성과 부딪혔을 때
        else if (collision.CompareTag("Planet"))
        {
            if (!collideOnPlanet)
                return;

            if (hitDestroyOn)
                AfterHitEvent();
        }

    }


    // 총알 본인이 맞았을 때 

    public void DamageEvent(float dmg, Vector2 objVel)
    {
        //피격시 발동하는 이벤트

        if (!hitByProjectileOn)     //projectile-player 과 더블체크이긴 하다. (필요없음)
            return;

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
