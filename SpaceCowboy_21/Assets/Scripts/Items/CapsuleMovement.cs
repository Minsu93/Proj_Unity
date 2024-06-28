using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    public WeaponData weaponData;   //담당하는 무기 
    public Collider2D CollisionBox;

    public Vector2 startVec = new Vector2(1, 1);
    public float speed = 1;

    Rigidbody2D rigid;
    Vector3 lastVelocity;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        lastVelocity = startVec * speed;
        rigid.velocity = lastVelocity;  
    }
    private void FixedUpdate()
    {
        lastVelocity = rigid.velocity;

        ScreenBorderCheck();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollisionBox.enabled = false;
            //플레이어와 부딪혔을 경우
            UseCapsule(collision.GetComponent<PlayerBehavior>());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("SpaceBorder"))
        {
            ReflectByNormal(collision.contacts[0].normal); 
        }
    }
    void ScreenBorderCheck()
    {
        //화면 밖으로 나갔는지 체크
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 && lastVelocity.x < 0)
        {
            ReflectByNormal(new Vector2(1, 0));
        }
        else if(screenPosition.x > Camera.main.pixelWidth && lastVelocity.x > 0)
        {
            ReflectByNormal(new Vector2(-1, 0));
        }

        if(screenPosition.y < 0 && lastVelocity.y < 0)
        {
            ReflectByNormal(new Vector2(0, 1));
        }
        else if(screenPosition.y > Camera.main.pixelHeight && lastVelocity.y > 0)
        {
            ReflectByNormal(new Vector2(0, -1));
        }
    }

    void ReflectByNormal(Vector2 normal)
    {
        var dir = Vector2.Reflect(lastVelocity.normalized, normal);

        rigid.velocity = dir * speed;
    }

    void UseCapsule(PlayerBehavior playerBehavior)
    {
        ////무기를 장착시킨다 
        //playerBehavior.TryChangeWeapon(weaponData);
        //이 오브젝트를 없엔다 
        DisableObject();
    }

    void DisableObject()
    {
        Destroy(gameObject);
    }
}
