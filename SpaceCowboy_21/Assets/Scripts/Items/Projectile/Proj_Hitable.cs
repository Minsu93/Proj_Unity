using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Hitable : Projectile, IHitable
{
    //������ ���߹�?

    Health health;

    protected override void Awake()
    {
        base.Awake();

        health = GetComponent<Health>();
    }

    public void DamageEvent(float damage, Vector2 hitVec)
    {
        if (health.AnyDamage(damage))  //��� ���ظ� �Ծ����� true
        {
            HitFeedBack();   //Projectile �� �ִ�. View �� HitFeedback�� �����Ų��.
        }

        if (health.IsDead())
        {
            //�����θ� �ı�
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
