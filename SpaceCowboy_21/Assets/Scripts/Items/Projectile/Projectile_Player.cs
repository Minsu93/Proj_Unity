using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Projectile_Player : Projectile
{
    [SerializeField] protected LayerMask overlapTarget;
    float startDelay;
    float overlapRadius;
    //int overlapLayer;
    protected override void Awake()
    {
        base.Awake();
        CircleCollider2D circleColl = coll as CircleCollider2D;
        overlapRadius = circleColl.radius;
        //overlapLayer = 1 << LayerMask.NameToLayer("ProjHitCollider") | 1 << LayerMask.NameToLayer("Planet") | LayerMask.NameToLayer("EnemyHitableProj");

    }

    protected override void Update()
    {
        base.Update();

        if (!activate) return;

        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }
        //�浹 üũ
        OverlapCheck();
    }


    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        base.Init(damage, speed, lifeTime, distance);
        
        //�Ѿ� ���� �� �浹 üũ(�׽�Ʈ)
        OverlapCheck();
        
    }

    public void Init(float damage, float speed, float lifeTime, float distance, float startDelay)
    {
        base.Init(damage, speed, lifeTime, distance);

        //�Ѿ� ���� �� �浹 üũ(�׽�Ʈ)
        this.startDelay = startDelay;

    }

    //�浹 üũ
    protected void OverlapCheck()
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, overlapRadius, overlapTarget);
        if (col != null)
        {
            OverlapTarget(col);
        }
    }

    protected virtual void OverlapTarget(Collider2D collision)
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
                HitEvent(target, hitable);
            }
            else
            {
                NonHitEvent(target);
            }

            WeaponImpactEvent();
        }
    }

    
    protected virtual void HitEvent(ITarget target, IHitable hitable)
    {

        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        AfterHitEvent();
    }


    //[Tooltip("�Ѿ��� ȭ�� ������ �󸶳� ������ �Ǵ� �� ������")]
    //[SerializeField] float screenBorderLimit = 5.0f;



    //�Ѿ��� ȭ�� ������ ������ �� 
    //bool OutSideScreenBorder()
    //{
    //    //ȭ�� ������ �������� üũ
    //    bool outside = false;
    //    Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

    //    if (screenPosition.x < 0 - screenBorderLimit)
    //    {
    //        outside = true;
    //    }
    //    else if (screenPosition.x > Camera.main.pixelWidth + screenBorderLimit )
    //    {
    //        outside = true;
    //    }

    //    if (screenPosition.y < 0 - screenBorderLimit)
    //    {
    //        outside = true;

    //    }
    //    else if (screenPosition.y > Camera.main.pixelHeight + screenBorderLimit)
    //    {
    //        outside = true;
    //    }

    //    return outside;
    //}
}
