using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_E_AntMine : Projectile_Enemy
{
    /// <summary>
    /// �÷��̾ ����� ��(����� ��), �÷��̾��� �ѿ� �¾��� �� ������. 
    /// �༺�� ������ �ű⿡ �ٴ´�. 
    /// </summary>
    /// <param name="collision"></param>
    /// 

    protected ProjectileGravity projGravity;

    protected override void Awake()
    {
        base.Awake();
        projGravity = GetComponent<ProjectileGravity>();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //�Ѿ��� �浹���� �� 
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out PlayerBehavior behavior))
            {
                //�÷��̾ ������ ���
                if (behavior.state == PlayerState.Die)
                    return;

                //������ ����
                behavior.DamageEvent(damage);

                ShowHitEffect();
                AfterHitEvent();
            }
        }

        else if (collision.CompareTag("Obstacle"))
        {
            if (collision.TryGetComponent(out Obstacle obstacle))
            {
                obstacle.AnyDamage(damage, rb.velocity);
                ShowHitEffect();
                AfterHitEvent();
            }
        }

        //�༺�� �ε����� ��
        else if (collision.CompareTag("Planet") || collision.CompareTag("SpaceBorder"))
        {
            StickToPlanet();
        }

    }

    //�༺�� �޶�ٴ´�.
    void StickToPlanet()
    {
        projectileMovement.StopMovement();
        //rb.position = pos;
        projGravity.activate = false;
    }

    public override void DamageEvent(float dmg, Vector2 objVel)
    {
        //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

        if (!hitByProjectileOn)     //projectile-player �� ����üũ�̱� �ϴ�. (�ʿ����)
            return;
        if (health.AnyDamage(dmg))  //��� ���ظ� �Ծ����� true
        {
            //HitFeedBack();   //Projectile �� �ִ�. View �� HitFeedback�� �����Ų��.
        }
        if (health.IsDead())
        {
            ShowHitEffect();
            AfterHitEvent();
        }

    }
}
