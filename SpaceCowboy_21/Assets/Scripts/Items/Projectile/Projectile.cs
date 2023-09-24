using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileTarget
    {
        Player,
        Enemy
    }
    
    [Header("Default Projectile Properties")]
    public GameObject hitEffect;
    float damage;
    float lifeTime;
    float time;
    public float speed { get; private set; }

    [Space]

    //이 오브젝트가 넉백을 얼마나 할 것인지 정해주는 변수들
    public float knockBackTime = 0.2f;  
    public float knockBackForce = 2f;

    [Space]

    //부서지기 전 딜레이
    public float disableDelayTime = 0.5f;

    [Header("Optional Properties")]
    public ProjectileTarget target;
    public bool rotateOn;       //속도에 따라 회전하는가
    public bool gravityOn;      //중력의 영향을 받는가
    public bool collideOnPlanet;      //지면(Planet)과 충돌하는가
    public bool hitDestroyOn;   //타겟과 충돌 시 제거되는가, 관통 여부
    public bool hitByProjectileOn;  //플레이어 총알에 맞는가

    bool activate; // 총알이 작동 정지되었음을 알리는 변수
    int targetLayer;    //뭔가 RayCast로 체크할게 있다면...target으로 쓸 수 있음
    protected Rigidbody2D rb;
    ProjectileGravity projectileGravity;
    ProjectileMovement projectileMovement;
    Health health;
    Collider2D coll;

    public GameObject ViewObj;
    public TrailRenderer trail;

    public event System.Action ResetProjectile;
    public event System.Action ProjectileHitEvent;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileMovement = GetComponent<ProjectileMovement>();
        coll = GetComponent<Collider2D>();

        if (gravityOn)
        {
            projectileGravity = GetComponent<ProjectileGravity>();
        }

        if(hitByProjectileOn)
        {
            health = GetComponent<Health>();
        }

        //target에 따라 targetLayer 전체묶음이 달라짐
        switch (target)
        {
            case ProjectileTarget.Player:
                targetLayer = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Planet");

                break;

            case ProjectileTarget.Enemy:
                targetLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyProjectile") | 1 << LayerMask.NameToLayer("Planet");
                
                break;
        }

    }

    public void init(float damage, float lifeTime, float speed)
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
        if(ResetProjectile!= null)
            ResetProjectile();

        if (gravityOn)
        {
            projectileGravity.activate = true;
            //행성 리스트 초기화
            projectileGravity.gravityPlanets.Clear();
        }

        if (hitByProjectileOn)
        {   //Projectile 체력 초기화
            health.ResetHealth();
        }

        projectileMovement.StartMovement(speed);
       
    }

    void Update()
    {
        if (!activate)
            return;

        //lifeTime이 0이 되었을 때
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            AfterHitEvent();
        }

        if(rotateOn)
        RotateProjectile();
    }


    void RotateProjectile()
    {
        //투사체는 속도에 따라서 회전 회전
        Vector2 direction = rb.velocity.normalized;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = targetRotation;
    }
   

    void OnTriggerEnter2D(Collider2D collision)
    {
        //타겟에 부딪혔을 때
        switch (target)
        {
            case ProjectileTarget.Player:
                if (collision.CompareTag("Player"))
                {
                    PlayerBehavior behavior = collision.GetComponent<PlayerBehavior>();

                    //플레이어가 죽으면 통과
                    if (behavior.state == PlayerState.Die)
                        return;

                    //맞았을 때 폭발 
                    HitEvent(behavior);

                    AfterHitEvent();

                    //여러번 때리는 경우에는?
                }
                break;

            case ProjectileTarget.Enemy:
                if (collision.CompareTag("Enemy"))
                {
                    EnemyBrain brain = collision.transform.GetComponent<EnemyBrain>();

                    HitEvent(brain);

                    AfterHitEvent();

                }

                else if (collision.CompareTag("EnemyProjectile"))
                {
                    Projectile proj = collision.transform.GetComponent<Projectile>();

                    if (!proj.hitByProjectileOn)
                    {
                        return;
                    }

                    HitEvent(proj);

                    AfterHitEvent();

                }
                break;
        }
        //행성과 부딪혔을 때
        if (collision.CompareTag("Planet"))
        {
            if (!collideOnPlanet)
                return;
            //HitEvent(collision);
            AfterHitEvent();
        }

    }

    void HitEvent(PlayerBehavior behavior)
    {
        behavior.DamageEvent(damage);


    }

    void HitEvent(EnemyBrain brain)
    {
        brain.DamageEvent(damage);

    }

    void HitEvent(Projectile proj)
    {
        proj.DamageEvent(damage);
        proj.KnockBackEvent(rb.velocity);

    }




    void AfterHitEvent()
    {

        if (!hitDestroyOn)
            return;

        //후처리
        Instantiate(hitEffect, transform.position, Quaternion.identity);



        //움직임을 정지
        projectileMovement.StopMovement();

        activate = false;
        coll.enabled = false;
        ViewObj.SetActive(false);

        if (gravityOn)
            projectileGravity.activate = false;

        rb.velocity = Vector2.zero;


        StartCoroutine(DisableDelay());
    }


    IEnumerator DisableDelay()
    {
        yield return new WaitForSeconds(disableDelayTime);
        if(trail != null)
            trail.enabled = false;
        gameObject.SetActive(false);

    }





    // 총알 본인이 맞았을 때 
    public void DamageEvent(float dmg)
    {
        //피격시 발동하는 이벤트

        if (!hitByProjectileOn)
            return;



        if (health.AnyDamage(dmg))  //체력이 모두 닳면 true 반환.
        {
            if (ProjectileHitEvent != null)
                ProjectileHitEvent();
        }

        if (health.IsDead())
        {
            //스스로를 파괴
            AfterHitEvent();
        }
    }


    public void KnockBackEvent(Vector2 objVel)
    {
        StartCoroutine(ProjectileKnockBack(objVel));
    }

    public IEnumerator ProjectileKnockBack(Vector2 vel)
    {
        //움직임을 정지
        projectileMovement.StopMovement();

        //총알 속도의 반대 벡터를 구함
        Vector2 negativeVel = vel.normalized;
        negativeVel *= knockBackForce;
        projectileMovement.AddImpulseToProj(negativeVel);

        yield return new WaitForSeconds(knockBackTime);
        projectileMovement.StartMovement(speed);
    }
}
