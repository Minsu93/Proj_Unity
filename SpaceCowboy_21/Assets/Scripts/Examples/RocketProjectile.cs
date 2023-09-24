using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    //explosive 加己
    public float explosionRadius;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //单固瘤甫 林绰 collision
        if (collision.CompareTag("Enemy") || collision.CompareTag("Planet") || collision.CompareTag("Obstacle"))
        {
            DestroyEvent();
        }

        if (collision.CompareTag("EnemyProjectile"))
        {
            if (collision.TryGetComponent<Health>(out Health targetHealth))
            {
                DestroyEvent();
             }
        }
    }

    public void ExplodeProjectile()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent<Health>(out Health targetHealth))
                {
                    //targetHealth.AnyDamage(damage, false);
                    //targetHealth.KnockBack(transform, knockBackForce);
                }
            }
        }
    }

    public void DestroyEvent()
    {
        ExplodeProjectile();

        //Instantiate(hitEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
