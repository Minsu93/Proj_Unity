using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Projectile_Player : Projectile
{
    [Tooltip("�Ѿ��� ȭ�� ������ �󸶳� ������ �Ǵ� �� ������")]
    [SerializeField] float screenBorderLimit = 5.0f;


    //public event System.Action<Vector2> ProjImpactEvent;

    PM_P_Normal projMov_P;

    protected override void Awake()
    {
        base.Awake();
        projMov_P = GetComponent<PM_P_Normal>();
    }

    public override void Init(float damage, float speed, float lifeTime, float distance, int penetrate, int reflect, int guide)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;
        this.penetrateCount = penetrate;
        this.reflectCount = reflect;
        this.guideAmount = guide;

        ResetProjectile();

        if(guide > 0)
        {
            //���� ����
            projMov_P.StartMovement(speed, guideAmount);
        }
        else
        {
            //���� ������
            projectileMovement.StartMovement(speed);
        }
    }
    

    protected override void HitEvent(ITarget target, IHitable hitable)
    {
        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        //��������
        if (penetrateCount > 0)
        {
            penetrateCount--;
            return;
        }
        //�ݻ�����
        if(reflectCount > 0)
        {
            reflectCount--;
            
            ReflectProjectile(target.GetCollider());
            return;
        }

        AfterHitEvent();

        
    }

    protected override void NonHitEvent(ITarget target)
    {

        ShowHitEffect(nonHitEffect);

        //�ݻ�����
        if (target != null && reflectCount > 0)
        {
            reflectCount--;

            ReflectProjectile(target.GetCollider());
            return;
        }

        AfterHitEvent();

    }

    void ReflectProjectile(Collider2D other)
    {
        float reflectionSpeed = rb.velocity.magnitude;
        Vector2 normal = other.ClosestPoint(transform.position) - (Vector2)transform.position;
        Vector2 reflectDirection = Vector2.Reflect(rb.velocity.normalized, normal.normalized);

        rb.velocity = reflectDirection * reflectionSpeed;
    }

    //�Ѿ��� ȭ�� ������ ������ �� 
    bool OutSideScreenBorder()
    {
        //ȭ�� ������ �������� üũ
        bool outside = false;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 - screenBorderLimit)
        {
            outside = true;
        }
        else if (screenPosition.x > Camera.main.pixelWidth + screenBorderLimit )
        {
            outside = true;
        }

        if (screenPosition.y < 0 - screenBorderLimit)
        {
            outside = true;

        }
        else if (screenPosition.y > Camera.main.pixelHeight + screenBorderLimit)
        {
            outside = true;
        }

        return outside;
    }
}
