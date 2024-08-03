using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Penetrate : Projectile_Player
{
    protected int penetrateCount = 1;  //관통 횟수


    protected override void HitEvent(ITarget target, IHitable hitable)
    {
        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        //관통적용
        if (penetrateCount > 0)
        {
            penetrateCount--;
            return;
        }
       
        AfterHitEvent();

    }




}
