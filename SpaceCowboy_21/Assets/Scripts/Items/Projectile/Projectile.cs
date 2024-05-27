using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Default")]
    protected bool activate;
    protected float damage;
    protected float lifeTime;
    protected float distance;
    protected float speed;

    float disableDelayTime = 0.5f;    //부서지기 전 딜레이
    float delayTimer;
    Vector2 startPos;

    private bool lifeLimitProj = false;
    protected Rigidbody2D rb;
    protected ProjectileMovement projectileMovement;
    protected Collider2D coll;

    public ParticleSystem hitEffect;
    public ParticleSystem nonHitEffect;
    public GameObject ViewObj;
    public TrailRenderer trail;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileMovement = GetComponent<ProjectileMovement>();
        coll = GetComponentsInChildren<Collider2D>()[0];

    }

    public virtual void ResetProjectile()
    {
        activate = true;
        coll.enabled = true;
        ViewObj.SetActive(true);
        delayTimer = disableDelayTime;
        startPos = transform.position;

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

    }

    public void Init(float damage, float speed, float lifeTime, float distance)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;
        if(lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();
        projectileMovement.StartMovement(speed);
    }

    public virtual void Init(float damage, float speed, float lifeTime, float distance, int penetrate, int reflect, int guide)
    {
        return;
    }




    #region Hit Event

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<ITarget>(out ITarget target))
        {
            if (collision.TryGetComponent<IHitable>(out IHitable hitable))
            {
                HitEvent(target, hitable);
            }
            else
            {
                NonHitEvent(target);
            }
        }
    }


    protected virtual void HitEvent(ITarget target,IHitable hitable)
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
        NonHitEvent(null);
        AfterHitEvent();
    }


    protected void ShowHitEffect(ParticleSystem particle)
    {
        if (particle != null)
            GameManager.Instance.particleManager.GetParticle(particle, transform.position, transform.rotation);
    }

    protected void AfterHitEvent()
    {
        projectileMovement.StopMovement();

        activate = false;
        coll.enabled = false;
        ViewObj.SetActive(false);

        //HitFeedBack();
    }

    #endregion



    void Update()
    {
        if (!activate)
        {
            if(delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                if(delayTimer<= 0) DisableObject();
            }
            return;
        }

        
        if (lifeLimitProj)
        {
            //총알에 수명이 있을 때 
            LifeTimeCheck();
        }
        else
        {
            //총알에 거리가 있을 때 
            DistanceCheck();
        }
        
    }

    void LifeTimeCheck()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            LifeOver();
        }
    }

    void DistanceCheck()
    {
        if(distance < Vector2.Distance(startPos, (Vector2)transform.position))
        {
            LifeOver();
        }
    }


    void DisableObject()
    {
        if(trail != null)
            trail.enabled = false;
        gameObject.SetActive(false);
    }


}
