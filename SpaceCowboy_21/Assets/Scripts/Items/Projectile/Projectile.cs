using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Default Projectile Properties")]
    public ParticleSystem hitEffect;
    protected float damage;
    protected float range;
    protected float lifeTime;
    protected bool infiniteLifeTime;
    public float speed { get; set; }
    protected Planet planetToOrbit;

    protected Vector2 startPos;

    [Space]
    public float disableDelayTime = 0.5f;    //부서지기 전 딜레이
    float delayTimer;

    //public bool rotateOn;       //속도에 따라 회전하는가
    //public bool gravityOn;      //중력의 영향을 받는가
    //public bool collideOnPlanet;      //지면(Planet)과 충돌하는가
    //public bool hitDestroyOn;   //타겟과 충돌 시 제거되는가, 관통 여부
    //public bool hitByProjectileOn;  //플레이어 총알에 맞는가

    [Space]
    protected bool activate; // 총알이 작동 정지되었음을 알리는 변수

    protected Rigidbody2D rb;
    protected ProjectileMovement projectileMovement;
    protected Collider2D coll;
    public GameObject ViewObj;
    public TrailRenderer trail;

    public event System.Action ProjectileInitEvent;
    public event System.Action DeactivateProjectileEvent;
    public event System.Action ProjectileHitEvent;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileMovement = GetComponent<ProjectileMovement>();
        coll = GetComponentsInChildren<Collider2D>()[0];

    }

    //총알 생성
    public virtual void init(float damage, float speed, float range, float lifeTime)
    {
        return;
    }

    public virtual void init(float damage, float speed, float range, float lifeTime, Planet planet, bool isRight)
    {
        return;
    }

    //총알이 맞았을 때 
    protected abstract void OnTriggerEnter2D(Collider2D collision);


    protected virtual void Update()
    {
        if (!activate)
        {
            if(delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                if(delayTimer<= 0)
                {
                    DisableObject();
                }
            }
            return;
        }

        if (!infiniteLifeTime)
        {
            //총알에 수명이 있을 때 
            LifeTimeCheck();
        }
        else
        {
            //총알에 수명이 없을 때 
            RangeCheck();
        }

    }


    void LifeTimeCheck()
    {
        lifeTime -= Time.deltaTime;
    
        if (lifeTime <= 0)
        {
            LifeTimeOver();
        }
    }

    //총알 수명이 끝났을 때 > 사라지느냐, 혹은 그 자리에서 터지느냐.
    protected virtual void LifeTimeOver()
    {
        AfterHitEvent();
    }

    //거리 측정
    void RangeCheck()
    {
        float dist = Vector2.Distance(startPos, transform.position);
        if (dist > range)
            AfterHitEvent();
    }

    //충돌 이펙트
    protected void ShowHitEffect()
    {
        //이펙트 표시
        //Instantiate(hitEffect, transform.position, transform.rotation);
        ParticleHolder.instance.GetParticle(hitEffect, transform.position, transform.rotation);
    }

    //충돌 후처리
    protected virtual void AfterHitEvent()
    {
        //움직임을 정지
        projectileMovement.StopMovement();
                
        activate = false;
        coll.enabled = false;
        ViewObj.SetActive(false);

        //다른 스크립트들에 이벤트 실행 
        HitFeedBack();

        //최종 제거 딜레이 초기화 
        delayTimer = disableDelayTime;
        
    }

    //딜레이 후 오브젝트를 완전히 제거 
    void DisableObject()
    {
        if(trail != null)
            trail.enabled = false;
        gameObject.SetActive(false);
    }



    //총알 충돌 시 
    protected void HitFeedBack()
    {
        if (ProjectileHitEvent != null)
            ProjectileHitEvent();
    }

    //총알 비활성화시 
    protected void DeactivateProjectile()
    {
        if (DeactivateProjectileEvent != null)
            DeactivateProjectileEvent();
    }

    //총알 시작 시 
    protected void InitProjectile()
    {
        if(ProjectileInitEvent != null) 
            ProjectileInitEvent();
    }


}
