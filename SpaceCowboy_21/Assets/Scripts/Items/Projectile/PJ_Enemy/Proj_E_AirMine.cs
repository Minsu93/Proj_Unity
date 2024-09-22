using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_E_AirMine : Projectile_GuideMissile
{


    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //총알이 충돌했을 때 
    //    if (collision.CompareTag("Player"))
    //    {
    //        if (collision.TryGetComponent(out PlayerBehavior behavior))
    //        {
    //            //플레이어가 죽으면 통과
    //            if (behavior.activate)
    //                return;

    //            //데미지 전달
    //            behavior.DamageEvent(damage);

    //            ShowHitEffect();
    //            AfterHitEvent();
    //        }
    //    }

    //    if (collision.CompareTag("SpaceBorder"))
    //    {
    //        ShowHitEffect();
    //        AfterHitEvent();
    //    }
                
    //}


    //public override void DamageEvent(float dmg, Vector2 objVel)
    //{
    //    //피격시 발동하는 이벤트

    //    if (!hitByProjectileOn)     //projectile-player 과 더블체크이긴 하다. (필요없음)
    //        return;
    //    if (health.AnyDamage(dmg))  //어떠한 피해를 입었으면 true
    //    {
    //        //HitFeedBack();   //Projectile 에 있다. View 의 HitFeedback을 실행시킨다.
    //    }
    //    if (health.IsDead())
    //    {
    //        ShowHitEffect();
    //        AfterHitEvent();
    //    }

    //}
}
