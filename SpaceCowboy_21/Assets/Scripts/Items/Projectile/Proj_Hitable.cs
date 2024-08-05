using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Hitable : Projectile
{
    ////보통은 폭발물?

    //Health health;

    ////public event System.Action ProjectileInitEvent;
    ////public event System.Action ProjectileHitEvent;
    //protected override void Awake()
    //{
    //    base.Awake();

    //    health = GetComponent<Health>();
    //}

    //public void DamageEvent(float damage, Vector2 hitVec)
    //{
    //    if (health.AnyDamage(damage))  //어떠한 피해를 입었으면 true
    //    {
    //        //HitFeedBack();   //Projectile 에 있다. View 의 HitFeedback을 실행시킨다.
    //    }

    //    if (health.IsDead())
    //    {
    //        //스스로를 파괴
    //        AfterHitEvent();
    //    }
    //    else
    //    {
    //        KnockBackEvent(hitVec);
    //    }
    //}

    //void KnockBackEvent(Vector2 objVel)
    //{
    //    //projectileMovement.KnockBackEvent(objVel);
    //}

    //protected override void LifeOver()
    //{
    //    base.LifeOver();
    //}

    //public void KnockBackEvent(Vector2 hitPos, float forceAmount)
    //{
    //    throw new System.NotImplementedException();
    //}
}
