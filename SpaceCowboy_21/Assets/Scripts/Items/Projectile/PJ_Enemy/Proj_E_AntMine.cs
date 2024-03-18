using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_E_AntMine : Projectile_Enemy
{
    /// <summary>
    /// 플레이어가 밟았을 때(닿았을 때), 플레이어의 총에 맞았을 때 터진다. 
    /// 행성에 맞으면 거기에 붙는다. 
    /// </summary>
    /// <param name="collision"></param>
    /// 

    protected ProjectileGravity projGravity;

    protected override void Awake()
    {
        base.Awake();
        projGravity = GetComponent<ProjectileGravity>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //총알이 충돌했을 때 
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out PlayerBehavior behavior))
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
        else if (collision.CompareTag("Planet") || collision.CompareTag("SpaceBorder"))
        {
            StickToPlanet();
        }

    }

    //행성에 달라붙는다.
    void StickToPlanet()
    {
        projectileMovement.StopMovement();
        //rb.position = pos;
        projGravity.activate = false;
    }

    public override void DamageEvent(float dmg, Vector2 objVel)
    {
        //피격시 발동하는 이벤트

        if (!hitByProjectileOn)     //projectile-player 과 더블체크이긴 하다. (필요없음)
            return;
        if (health.AnyDamage(dmg))  //어떠한 피해를 입었으면 true
        {
            //HitFeedBack();   //Projectile 에 있다. View 의 HitFeedback을 실행시킨다.
        }
        if (health.IsDead())
        {
            ShowHitEffect();
            AfterHitEvent();
        }

    }
}
