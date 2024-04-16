using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Default")]
    protected bool activate;
    protected float damage;
    protected float lifeTime;
    protected float speed;

    float disableDelayTime = 0.5f;    //부서지기 전 딜레이
    float delayTimer;
    
    protected Rigidbody2D rb;
    protected ProjectileMovement projectileMovement;
    protected Collider2D coll;

    public ParticleSystem hitEffect;
    public ParticleSystem nonHitEffect;
    public GameObject ViewObj;
    public TrailRenderer trail;

    public event System.Action ProjectileInitEvent;
    public event System.Action ProjectileHitEvent;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileMovement = GetComponent<ProjectileMovement>();
        coll = GetComponentsInChildren<Collider2D>()[0];

    }

    public void Init(float damage, float speed, float lifeTime)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        
        ResetProjectile();
        InitProjectile();
        projectileMovement.StartMovement(speed);
    }

    public virtual void ResetProjectile()
    {
        activate = true;
        coll.enabled = true;
        ViewObj.SetActive(true);
        delayTimer = disableDelayTime;

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

    }

    #region Hit Event

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IHitable>(out IHitable hitable))
        {
            HitEvent(hitable);
        }
        else
        {
            NonHitEvent();
        }
        AfterHitEvent();

    }

    protected virtual void HitEvent(IHitable hitable)
    {

        hitable.DamageEvent(damage, transform.position);
        ShowHitEffect(hitEffect);
    }
    protected virtual void NonHitEvent()
    {

        ShowHitEffect(nonHitEffect);

    }

    protected void ShowHitEffect(ParticleSystem particle)
    {
        if (particle != null)
            ParticleHolder.instance.GetParticle(particle, transform.position, transform.rotation);
    }

    protected void AfterHitEvent()
    {

        projectileMovement.StopMovement();

        activate = false;
        coll.enabled = false;
        ViewObj.SetActive(false);

        HitFeedBack();
    }

    #endregion

    #region LifeTime

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

        //총알에 수명이 있을 때 
        LifeTimeCheck();
    }

    void LifeTimeCheck()
    {
        lifeTime -= Time.deltaTime;
    
        if (lifeTime <= 0)
        {
            LifeTimeOver();
        }
    }

    protected virtual void LifeTimeOver()
    {
        NonHitEvent();
        AfterHitEvent();
    }

    #endregion


    void DisableObject()
    {
        if(trail != null)
            trail.enabled = false;
        gameObject.SetActive(false);
    }

    
    protected void HitFeedBack()
    {
        if (ProjectileHitEvent != null)
            ProjectileHitEvent();
    }
    
    protected void InitProjectile()
    {
        if(ProjectileInitEvent != null) 
            ProjectileInitEvent();
    }


}
