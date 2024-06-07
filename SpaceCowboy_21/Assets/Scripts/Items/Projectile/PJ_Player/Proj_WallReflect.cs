using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Proj_WallReflect : Projectile
{
    //벽(이라 인식하는) 에 부딪히면 튕겨다닌다. 반사와 다른 점은 행성 뿐만 아니라 화면의 경계에서도 튕겨난다는 것. 
    //적에게 직접 피해를 주기보다는 화면에 계속 유지되면서 지속 피해를 준다. 
    //그냥 PlayerProjectile 로 해결하기로 함. (보류)

    Vector2 curVelocity;
    Vector2 lastPos;

    public override void Init(float damage, float speed, float lifeTime, float distance, int penetrate, int reflect, int guide)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;
        this.penetrateCount = penetrate;
        this.reflectCount = reflect;
        this.guideAmount = guide;

        if (lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();
        lastPos = transform.position;
        projectileMovement.StartMovement(speed);
    }


    private void FixedUpdate()
    {
        curVelocity = (Vector2)transform.position - lastPos;

        ScreenBorderCheck();
    }

    void ScreenBorderCheck()
    {
        //화면 밖으로 나갔는지 체크
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 && curVelocity.x < 0)
        {
            ReflectByNormal(new Vector2(1, 0));
        }
        else if (screenPosition.x > Camera.main.pixelWidth && curVelocity.x > 0)
        {
            ReflectByNormal(new Vector2(-1, 0));
        }

        if (screenPosition.y < 0 && curVelocity.y < 0)
        {
            ReflectByNormal(new Vector2(0, 1));
        }
        else if (screenPosition.y > Camera.main.pixelHeight && curVelocity.y > 0)
        {
            ReflectByNormal(new Vector2(0, -1));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Planet"))
        {
            ReflectByNormal(collision.contacts[0].normal);
        }
    }


    void ReflectByNormal(Vector2 normal)
    {
        var dir = Vector2.Reflect(curVelocity.normalized, normal);

        //대상 방향으로 회전.
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * dir);
        transform.rotation = rot;
        projectileMovement.StartMovement(speed);
        
    }



    protected virtual void HitEvent(ITarget target, IHitable hitable)
    {

        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        AfterHitEvent();
    }

    protected virtual void NonHitEvent(ITarget target)
    {
        ShowHitEffect(nonHitEffect);
        AfterHitEvent();
    }

    protected virtual void LifeOver()
    {
        AfterHitEvent();
    }

}
