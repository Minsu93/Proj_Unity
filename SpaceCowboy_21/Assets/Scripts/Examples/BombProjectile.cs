using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : MonoBehaviour
{
    public float explosionRadius;

    float flickTimer;
    SpriteRenderer spr;

    public void Awake()
    {

        spr = GetComponentInChildren<SpriteRenderer>();
    }

    public void init(float damage, float lifeTime, float speed, float knockBackForce)
    {
        //base.init(damage, lifeTime, speed, knockBackForce);
        
        flickTimer = lifeTime;
        StartCoroutine(Flick());

    }

    public void updateFunction()
    {
        return;
    }

    IEnumerator Flick()
    {
        while (true)
        {
            yield return StartCoroutine(FlickCoolTime());

            spr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spr.color = Color.black;
        }

    }

    IEnumerator FlickCoolTime()
    {
        flickTimer *= 0.5f;
        yield return new WaitForSeconds(flickTimer);
    }


    void Boom()
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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        return;
    }

    public void DestroyEvent()
    {
        Boom();
        StopAllCoroutines();
    }

}
