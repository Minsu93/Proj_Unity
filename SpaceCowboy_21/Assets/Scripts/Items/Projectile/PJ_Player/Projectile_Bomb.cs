using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Projectile_Bomb : Projectile
{
    //폭탄은 내가 원하는 방향으로, 정해진 거리 만큼 날아간다. 일단은 가깝게 던지고 싶어도 정해진 거리만큼 날아간다. 
    //폭탄이 터지는 순간은 수명이 끝났을 때, 무언가와 부딪혔을 때이다. 
    //폭탄이 터지는 순간 특수한 효과를 생성하는 폭탄들이 있다. 

    [SerializeField] BombType type = BombType.Instant;
    [SerializeField] GameObject usingBomb; //플레이어가 사용할 투척 위성 >> orb

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


    //폭탄이 터지는 효과 
    public virtual void Explode(Vector2 pos)
    {
        switch (type)
        {
            case BombType.Instant:
                //폭발 피해
                Debug.Log("Bomb!");
                break;

            case BombType.Bubble:
                //터진 자리에 오브 생성.
                GameObject newOrb = GameManager.Instance.poolManager.GetItem(usingBomb);
                newOrb.transform.position = pos;
                newOrb.transform.rotation = Quaternion.identity;
                break;
        }

    }


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

public enum BombType
{
    Instant,
    Bubble
}