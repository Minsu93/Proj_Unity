using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile_Player : Projectile
{
    [Tooltip("�Ѿ��� ȭ�� ������ �󸶳� ������ �Ǵ� �� ������")]
    public float screenBorderLimit = 5.0f;
    int reflectCount;

    void ReflectCheck(Collider2D collision)
    {
        if (reflectCount > 0) ReflectProjectile(collision);
        else AfterHitEvent();
    }

    void ReflectProjectile(Collider2D other)
    {
        reflectCount--;

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
