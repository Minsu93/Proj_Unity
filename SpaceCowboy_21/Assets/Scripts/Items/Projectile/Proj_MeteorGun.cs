using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_MeteorGun : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IGravitable>(out IGravitable gravitable))
        {
            gravitable.GravityOnEvent();
            ShowHitEffect(hitEffect);
        }
        else
        {
            NonHitEvent();
        }
        AfterHitEvent();
    }
}
