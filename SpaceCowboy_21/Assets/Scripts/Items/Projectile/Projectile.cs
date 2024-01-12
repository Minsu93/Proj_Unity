using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [Header("Default Projectile Properties")]
    public GameObject hitEffect;
    protected float damage;
    protected float lifeTime;
    protected float time;
    public float speed { get; set; }



    [Space]

    //부서지기 전 딜레이
    public float disableDelayTime = 0.5f;

    [Header("Optional Properties")]

    //public bool rotateOn;       //속도에 따라 회전하는가
    public bool gravityOn;      //중력의 영향을 받는가
    public bool collideOnPlanet;      //지면(Planet)과 충돌하는가
    public bool hitDestroyOn;   //타겟과 충돌 시 제거되는가, 관통 여부
    public bool hitByProjectileOn;  //플레이어 총알에 맞는가

    public bool activate; // 총알이 작동 정지되었음을 알리는 변수

    protected Rigidbody2D rb;
    protected ProjectileGravity projectileGravity;
    protected ProjectileMovement projectileMovement;
    protected Collider2D coll;

    public GameObject ViewObj;
    public TrailRenderer trail;

    public event System.Action ResetProjectile;
    public event System.Action ProjectileHitEvent;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileMovement = GetComponent<ProjectileMovement>();
        coll = GetComponent<Collider2D>();

        if (gravityOn)
        {
            projectileGravity = GetComponent<ProjectileGravity>();
        }

    }

    public virtual void init(float damage, float lifeTime, float speed)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        time = 0;

        activate = true;
        coll.enabled = true;
        ViewObj.SetActive(true);

        if (trail != null){
            trail.enabled = true;
            trail.Clear();
        }


        if (gravityOn)
        {
            projectileGravity.activate = true;
            //행성 리스트 초기화
            projectileGravity.gravityPlanets.Clear();
        }

        projectileMovement.StartMovement(speed);
       
    }


    protected virtual void Update()
    {
        if (!activate)
            return;

        //lifeTime이 0이 되었을 때
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            LifeTimeOver();
        }

        //위치에 따른 검사.
    }

    protected virtual void LifeTimeOver()
    {   //총알 수명이 끝났을 때 벌어질 일
        AfterHitEvent();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       
    }


    protected virtual void AfterHitEvent()
    {
        //후처리
        Instantiate(hitEffect, transform.position, Quaternion.identity);


        //움직임을 정지
        projectileMovement.StopMovement();

        activate = false;
        coll.enabled = false;
        ViewObj.SetActive(false);

        if (gravityOn)
            projectileGravity.activate = false;

        //rb.velocity = Vector2.zero;


        StartCoroutine(DisableDelay());
    }


    IEnumerator DisableDelay()
    {
        yield return new WaitForSeconds(disableDelayTime);
        if(trail != null)
            trail.enabled = false;
        gameObject.SetActive(false);

    }


    //Projectile View 에 전달하는 이벤트들

    protected void HitFeedBack()
    {
        if (ProjectileHitEvent != null)
            ProjectileHitEvent();
    }

    protected void ProjectileViewReset()
    {
        if (ResetProjectile != null)
            ResetProjectile();
    }



    
}
