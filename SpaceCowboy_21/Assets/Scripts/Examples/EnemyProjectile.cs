using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public enum E_ProjectileType { Normal, Pierce, High_G, ChasingExplosive }
    
    public GameObject hitEffect;
    [Header("Projectile Status")]
    public E_ProjectileType type = E_ProjectileType.Normal;
    public float damage;
    public float lifeTime;
    public float speed;
    [Header("Chasing Explosive Property")]
    public float maxSpeed ;
    public float chasingSpeed;
    public float explosionRadius;
    public float explosionForce;

    float time;

    Rigidbody2D rb;
    GravityFinder gravityFinder;
    TrailRenderer trailRenderer;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityFinder = GetComponent<GravityFinder>();
        trailRenderer = GetComponent<TrailRenderer>();  
    }

    public void Init(float damage, float lifeTime, float speed)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        time = 0;
        rb.velocity = Vector2.zero;
        trailRenderer.Clear();
        switch (type)
        {
            case E_ProjectileType.Normal:
                gravityFinder.activate = true;
                
                break;
            case E_ProjectileType.Pierce:
                gravityFinder.activate = false;
                break;
            case E_ProjectileType.High_G:
                gravityFinder.activate = true;
                break;
            case E_ProjectileType.ChasingExplosive:
                gravityFinder.activate = false;
                //GetComponent<Health>().ResetHealth();
                break;
        }


        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }

    void Update()
    {
        RotateProjectile();

        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            DestroyFunction();
        }

        switch (type)
        {
            case E_ProjectileType.ChasingExplosive:
                ChasePlayer();
                break;
        }
    }

    void RotateProjectile()
    {
        //투사체는 속도에 따라서 회전 회전
        Vector2 direction = rb.velocity.normalized;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = targetRotation;
    }

    void ChasePlayer()
    {
        Vector2 dir = (GameManager.Instance.player.position - transform.position).normalized;
        rb.AddForce(dir * chasingSpeed * 100f * Time.deltaTime);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //데미지를 주는 collision
        if (collision.CompareTag("Player") )
        {
            switch (type)
            {
                case E_ProjectileType.Normal:
                case E_ProjectileType.Pierce:
                case E_ProjectileType.High_G:

                    if (collision.TryGetComponent<Health>(out Health targetHealth))
                    {
                        targetHealth.AnyDamage(damage);
                        DestroyFunction();
                    }
                    break;

                case E_ProjectileType.ChasingExplosive:
                    DestroyFunction();
                    break;
            } 
        }
        
        if(collision.CompareTag("Planet") || collision.CompareTag("Obstacle"))
        {
            switch (type)
            {
                case E_ProjectileType.Normal:
                case E_ProjectileType.High_G:

                    if (collision.TryGetComponent<Health>(out Health targetHealth))
                        targetHealth.AnyDamage(damage);
                        

                    DestroyFunction();
                    break;

                case E_ProjectileType.ChasingExplosive:
                    DestroyFunction();
                    break;
            }
        }
        

    }


    public void ExplodeProjectile()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Player"));

        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent<Health>(out Health targetHealth))
                {
                    targetHealth.AnyDamage(damage);
                }
            }
        }


    }

    void DestroyFunction()
    {
        switch (type)
        {
            case E_ProjectileType.Normal:
            case E_ProjectileType.Pierce:
            case E_ProjectileType.High_G:
                break;
            case E_ProjectileType.ChasingExplosive:
                ExplodeProjectile();
                break;
        }

        Instantiate(hitEffect, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

}
