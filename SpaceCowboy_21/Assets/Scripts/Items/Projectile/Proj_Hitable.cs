using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Hitable : Projectile, IHitable
{
    //보통은 폭발물?

    Health health;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Health>();
    }

    public void DamageEvent(float damage, Vector2 hitVec)
    {
        if (health.AnyDamage(damage))  //어떠한 피해를 입었으면 true
        {
            HitFeedBack();   //Projectile 에 있다. View 의 HitFeedback을 실행시킨다.
        }

        if (health.IsDead())
        {
            //스스로를 파괴
            NonHitEvent();
            AfterHitEvent();
        }
        else
        {
            KnockBackEvent(hitVec);
        }
    }

    void KnockBackEvent(Vector2 objVel)
    {
        projectileMovement.KnockBackEvent(objVel);
    }

    protected override void LifeTimeOver()
    {
        base.LifeTimeOver();
    }

    protected override void HitEvent(IHitable hitable)
    {
        base.HitEvent(hitable);
    }

    protected override void NonHitEvent()
    {
        base.NonHitEvent();

    }
}
