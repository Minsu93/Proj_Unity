using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Projectile_Bomb : Projectile
{
    //자손은 Explode 를 상속받아서 사용하자. 

    PM_P_Bomb bombMovement;

    protected override void Awake()
    {
        base.Awake();
        bombMovement = GetComponent<PM_P_Bomb>();
    }

    public override void Init(float damage, float speed, float lifeTime, float distance, int penetrate, int reflect, int guide)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;
        this.penetrateCount = penetrate;
        this.reflectCount = reflect;
        this.guideAmount = guide;

        if (lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();
        bombMovement.StartMovement(speed,distance);
        bombMovement.BombExplodeEvent -= Explode;
        bombMovement.BombExplodeEvent += Explode;
    }


    //폭탄이 터지는 효과, 자손들은 이 부분을 변형하면 된다. 
    public abstract void Explode(Vector2 pos);




    protected override void HitEvent(ITarget target, IHitable hitable)
    {

        Explode(transform.position);

        AfterHitEvent();
    }

    protected override void NonHitEvent(ITarget target)
    {
        Explode(transform.position);

        AfterHitEvent();
    }

    protected override void LifeOver()
    {
        Explode(transform.position);

        AfterHitEvent();
    }

}
