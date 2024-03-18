using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJumper : MonoBehaviour
{
    public float force;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //위 방향으로 대상을 날려보낸다. 

        if (collision.CompareTag("Player"))
        {
            //현재 플레이어의 방향 계산
            Vector2 pVel = (transform.position - collision.transform.position).normalized;
            float angle = Vector2.Angle(transform.up, pVel);
            
            if (angle > 90)
                return;

            Vector2 vec = transform.up * force;
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(vec, ForceMode2D.Impulse);

        }
    }
}
