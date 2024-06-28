 using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_GuideMissile : Projectile , ITarget, IHitable
{
    [Header("Explode")]
    public float explodeRadius = 3.0f;
    public float knockBackAmount = 5.0f;
    public LayerMask targetLayer;


    Health health;
    //ProjMov_Guide guideMovement;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Health>();
        //guideMovement = GetComponent<ProjMov_Guide>();
    }


    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (health.AnyDamage(damage))
        {
            //맞는 효과 
            if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

            if (health.IsDead())
            {
                BoomEvent();
                ShowHitEffect(nonHitEffect);
                AfterHitEvent();

            }
        }
    }

    protected override void HitEvent(ITarget target, IHitable hitable)
    {
        //폭발 추가
        BoomEvent();

        AfterHitEvent();
    }

    protected override void NonHitEvent(ITarget target)
    {
        //폭발 추가
        BoomEvent();

        AfterHitEvent();
    }

    protected override void LifeOver()
    {
        //폭발 추가
        BoomEvent();

        AfterHitEvent();
    }

    //폭발은 주변 타겟에게 피해와 넉백을 준다.
    protected virtual void BoomEvent()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, explodeRadius, targetLayer);
        if(colls.Length > 0)
        {
            Debug.Log("폭발에 맞았다.");
            for(int i = 0; i< colls.Length; i++)
            {
                if (colls[i].transform.TryGetComponent<IHitable>(out IHitable hitable))
                {
                    hitable.DamageEvent(damage, transform.position);
                    hitable.KnockBackEvent(transform.position, knockBackAmount);
                }
            }
        }

        //폭발 이펙트 생성.
        ShowHitEffect(nonHitEffect);
    }

    //적의 미사일이 플레이어의 총알에 맞았을 때, ProjMove_Guide에 전달하여 넉백을 한다.(현재는 제외함)
    public void KnockBackEvent(Vector2 hitPos, float forceAmount)
    {
        return;
    }

    Collider2D ITarget.GetCollider()
    {
        return coll;
    }
}
