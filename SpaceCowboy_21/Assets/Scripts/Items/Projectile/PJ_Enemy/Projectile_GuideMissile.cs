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
            //�´� ȿ�� 
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
        //���� �߰�
        BoomEvent();

        AfterHitEvent();
    }

    protected override void NonHitEvent(ITarget target)
    {
        //���� �߰�
        BoomEvent();

        AfterHitEvent();
    }

    protected override void LifeOver()
    {
        //���� �߰�
        BoomEvent();

        AfterHitEvent();
    }

    //������ �ֺ� Ÿ�ٿ��� ���ؿ� �˹��� �ش�.
    protected virtual void BoomEvent()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, explodeRadius, targetLayer);
        if(colls.Length > 0)
        {
            Debug.Log("���߿� �¾Ҵ�.");
            for(int i = 0; i< colls.Length; i++)
            {
                if (colls[i].transform.TryGetComponent<IHitable>(out IHitable hitable))
                {
                    hitable.DamageEvent(damage, transform.position);
                    hitable.KnockBackEvent(transform.position, knockBackAmount);
                }
            }
        }

        //���� ����Ʈ ����.
        ShowHitEffect(nonHitEffect);
    }

    //���� �̻����� �÷��̾��� �Ѿ˿� �¾��� ��, ProjMove_Guide�� �����Ͽ� �˹��� �Ѵ�.(����� ������)
    public void KnockBackEvent(Vector2 hitPos, float forceAmount)
    {
        return;
    }

    Collider2D ITarget.GetCollider()
    {
        return coll;
    }
}
