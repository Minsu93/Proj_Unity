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

    //�� ������Ʈ�� �˹��� �󸶳� �� ������ �����ִ� ������
    public float knockBackTime = 0.2f;  
    public float knockBackForce = 2f;

    [Space]

    //�μ����� �� ������
    public float disableDelayTime = 0.5f;

    [Header("Optional Properties")]
    public ProjectileTarget target;
    public bool rotateOn;       //�ӵ��� ���� ȸ���ϴ°�
    public bool gravityOn;      //�߷��� ������ �޴°�
    public bool collideOnPlanet;      //����(Planet)�� �浹�ϴ°�
    public bool hitDestroyOn;   //Ÿ�ٰ� �浹 �� ���ŵǴ°�, ���� ����
    public bool hitByProjectileOn;  //�÷��̾� �Ѿ˿� �´°�

    bool activate; // �Ѿ��� �۵� �����Ǿ����� �˸��� ����
    int targetLayer;    //���� RayCast�� üũ�Ұ� �ִٸ�...target���� �� �� ����
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

        //target�� ���� targetLayer ��ü������ �޶���
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
            //�༺ ����Ʈ �ʱ�ȭ
            projectileGravity.gravityPlanets.Clear();
        }

        if (hitByProjectileOn)
        {   //Projectile ü�� �ʱ�ȭ
            health.ResetHealth();
        }

        projectileMovement.StartMovement(speed);
       
    }

    void Update()
    {
        if (!activate)
            return;

        //lifeTime�� 0�� �Ǿ��� ��
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
        //����ü�� �ӵ��� ���� ȸ�� ȸ��
        Vector2 direction = rb.velocity.normalized;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = targetRotation;
    }
   

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Ÿ�ٿ� �ε����� ��
        switch (target)
        {
            case ProjectileTarget.Player:
                if (collision.CompareTag("Player"))
                {
                    PlayerBehavior behavior = collision.GetComponent<PlayerBehavior>();

                    //�÷��̾ ������ ���
                    if (behavior.state == PlayerState.Die)
                        return;

                    //�¾��� �� ���� 
                    HitEvent(behavior);

                    AfterHitEvent();

                    //������ ������ ��쿡��?
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
        //�༺�� �ε����� ��
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

        //��ó��
        Instantiate(hitEffect, transform.position, Quaternion.identity);



        //�������� ����
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





    // �Ѿ� ������ �¾��� �� 
    public void DamageEvent(float dmg)
    {
        //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

        if (!hitByProjectileOn)
            return;



        if (health.AnyDamage(dmg))  //ü���� ��� ��� true ��ȯ.
        {
            if (ProjectileHitEvent != null)
                ProjectileHitEvent();
        }

        if (health.IsDead())
        {
            //�����θ� �ı�
            AfterHitEvent();
        }
    }


    public void KnockBackEvent(Vector2 objVel)
    {
        StartCoroutine(ProjectileKnockBack(objVel));
    }

    public IEnumerator ProjectileKnockBack(Vector2 vel)
    {
        //�������� ����
        projectileMovement.StopMovement();

        //�Ѿ� �ӵ��� �ݴ� ���͸� ����
        Vector2 negativeVel = vel.normalized;
        negativeVel *= knockBackForce;
        projectileMovement.AddImpulseToProj(negativeVel);

        yield return new WaitForSeconds(knockBackTime);
        projectileMovement.StartMovement(speed);
    }
}
