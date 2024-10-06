using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Boomerang : Projectile_Player
{
    List<Collider2D> hitList = new List<Collider2D>();//�̹� ���� �� ����Ʈ

    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;

        ResetProjectile();
        PM_Boomerang boomerang = projectileMovement as PM_Boomerang;
        boomerang.StartMovement(speed, distance);

        //Projectile_Player �� �Լ�
        OverlapCheck();

        //Ÿ�� �� ��� �ʱ�ȭ
        ResetHitList();
    }

    protected override void OverlapTarget(Collider2D collision)
    {

        Transform tr = collision.transform;

        if (collision.CompareTag("ProjHitCollider"))
        {
            tr = collision.transform.parent;
        }

        if (tr.TryGetComponent<ITarget>(out ITarget target))
        {
            if (tr.TryGetComponent<IHitable>(out IHitable hitable))
            {
                if (!hitList.Contains(collision))
                {
                    hitList.Add(collision);
                    HitEvent(target, hitable);
                }
            }
            else
            {
                NonHitEvent(target);
            }

            WeaponImpactEvent();
        }


    }

    protected override void HitEvent(ITarget target, IHitable hitable)
    {

        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        //AfterHitEvent();

    }
    protected override void Update()
    {
        if (!activate) return;

        //�浹 üũ
        OverlapCheck();
    }

    //�� Ÿ�� ����Ʈ�� �����Ѵ�. 
    public void ResetHitList()
    {
        hitList.Clear();
    }

    //�θ޶��� ��� ���ƿ��⸦ ������ �� 
    public void ReturnFinish()
    {
        AfterHitEvent();
    }
}
