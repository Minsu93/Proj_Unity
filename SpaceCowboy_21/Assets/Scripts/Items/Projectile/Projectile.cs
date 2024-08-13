using SpaceCowboy;
using System;
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

    protected bool lifeLimitProj = false;
    protected Rigidbody2D rb;
    protected ProjectileMovement projectileMovement;
    protected Collider2D coll;

    public ParticleSystem hitEffect;
    public ParticleSystem nonHitEffect;
    public GameObject ViewObj;
    public TrailRenderer trail;

    public WeaponImpactDelegate weaponImpactDel;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileMovement = GetComponent<ProjectileMovement>();
        //coll = GetComponentsInChildren<Collider2D>()[0];
        coll = GetComponent<Collider2D>();

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

    public virtual void Init(float damage, float speed, float lifeTime, float distance)
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




    #region Hit Event





    protected virtual void NonHitEvent(ITarget target)
    {
        ShowHitEffect(nonHitEffect);
        AfterHitEvent();
    }

    protected virtual void LifeOver()
    {
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

    protected void WeaponImpactEvent()
    {
        if (weaponImpactDel != null) weaponImpactDel();
    }
    #endregion


    protected virtual void Update()
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

        //총알 수명 체크
        if (lifeLimitProj)
        {
            LifeTimeCheck();
        }

        //총알 거리 체크
        else
        {
            float dist = Vector2.Distance(startPos, (Vector2)transform.position);
            DistanceCheck(dist);          
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

    void DistanceCheck(float dist)
    {
        if(distance < dist)
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
