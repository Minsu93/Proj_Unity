using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Proj_WallReflect : Projectile_Player
{
    //��(�̶� �ν��ϴ�) �� �ε����� ƨ�ܴٴѴ�. �ݻ�� �ٸ� ���� �༺ �Ӹ� �ƴ϶� ȭ���� ��迡���� ƨ�ܳ��ٴ� ��. 
    //������ ���� ���ظ� �ֱ⺸�ٴ� ȭ�鿡 ��� �����Ǹ鼭 ���� ���ظ� �ش�. 
    //�׳� PlayerProjectile �� �ذ��ϱ�� ��. (����)

    Vector2 curVelocity;
    Vector2 lastPos;

    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        base.Init(damage, speed, lifeTime, distance);

        lastPos = transform.position;

    }


    private void FixedUpdate()
    {
        curVelocity = (Vector2)transform.position - lastPos;

        ScreenBorderCheck();
    }

    void ScreenBorderCheck()
    {
        //ȭ�� ������ �������� üũ
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

        //��� �������� ȸ��.
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * dir);
        transform.rotation = rot;
        projectileMovement.StartMovement(speed);
        
    }



    protected override void HitEvent(ITarget target, IHitable hitable)
    {

        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        AfterHitEvent();
    }

    protected override void NonHitEvent(ITarget target)
    {
        ShowHitEffect(nonHitEffect);
        AfterHitEvent();
    }

    protected override void LifeOver()
    {
        AfterHitEvent();
    }

}
