 using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    public bool hitByProjectileOn = false;
    public bool isOrbital = false;

    protected Health health;
    protected ProjMov_Orbital orbitMov;


    protected override void Awake()
    {
        base.Awake();

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if(isOrbital) orbitMov = GetComponent<ProjMov_Orbital>();
    }


    public virtual void DamageEvent(float dmg, Vector2 objVel)
    {
        //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

        if (!hitByProjectileOn)     //projectile-player �� ����üũ�̱� �ϴ�. (�ʿ����)
            return;
        if (health == null) return;

        if (health.AnyDamage(dmg))  //��� ���ظ� �Ծ����� true
        {
            HitFeedBack();   //Projectile �� �ִ�. View �� HitFeedback�� �����Ų��.
        }

        if (health.IsDead())
        {
            //�����θ� �ı�
            AfterHitEvent();
        }
        else
        {
            KnockBackEvent(objVel);
        }

    }


    void KnockBackEvent(Vector2 objVel)
    {
       projectileMovement.KnockBackEvent(objVel);
    }

}
