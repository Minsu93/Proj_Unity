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


    //[Tooltip("총알이 화면 밖으로 얼마나 나가도 되는 지 여유분")]
    //[SerializeField] float screenBorderLimit = 5.0f;



    //총알이 화면 밖으로 나갔을 때 
    //bool OutSideScreenBorder()
    //{
    //    //화면 밖으로 나갔는지 체크
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
