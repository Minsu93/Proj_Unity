 using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    public bool hitByProjectileOn = false;
    public bool isOrbital = false;

    protected Health health;
    protected ProjMov_Orbital orbitMov;


    protected override void Awake()
    {
        base.Awake();

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if(isOrbital) orbitMov = GetComponent<ProjMov_Orbital>();
    }


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
