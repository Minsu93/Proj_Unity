using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    Health health;

    protected override void Awake()
    {
        base.Awake();

        if (hitByProjectileOn)
        {
            health = GetComponent<Health>();
        }
    }

    public override void init(float damage, float lifeTime, float speed)
    {
        base.init(damage, lifeTime, speed); 

        if (hitByProjectileOn)
        {   //Projectile ü�� �ʱ�ȭ
            health.ResetHealth();
        }

        ProjectileViewReset();

    }

    //���� �Ѿ�
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //�Ѿ��� �浹���� �� 
        if (collision.CompareTag("Player"))
        {
            if(collision.TryGetComponent(out PlayerBehavior behavior))
            {
                //�÷��̾ ������ ���
                if (behavior.state == PlayerState.Die)
                    return;

                //������ ����
                behavior.DamageEvent(damage);

                if (hitDestroyOn)
                    AfterHitEvent();

            }
        }

        //�༺�� �ε����� ��
        else if (collision.CompareTag("Planet"))
        {
            if (!collideOnPlanet)
                return;

            if (hitDestroyOn)
                AfterHitEvent();
        }

    }


    // �Ѿ� ������ �¾��� �� 

    public void DamageEvent(float dmg, Vector2 objVel)
    {
        //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

        if (!hitByProjectileOn)     //projectile-player �� ����üũ�̱� �ϴ�. (�ʿ����)
            return;

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
