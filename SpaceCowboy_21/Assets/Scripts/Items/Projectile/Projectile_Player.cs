using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Projectile_Player : Projectile
{

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activate) return;

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
