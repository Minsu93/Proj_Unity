 using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Enemy : Projectile
{
    public bool hitByProjectileOn = false;
    public bool isOrbital = false;

    protected Health health;
    protected ProjMov_Orbital orbitMov;


    protected override void Awake()
    {
        base.Awake();

        if (health == null)
        {
            health = GetComponent<Health>();
        }

        if(isOrbital) orbitMov = GetComponent<ProjMov_Orbital>();
    }

    public override void init(float damage, float speed, float range, float lifeTime)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.lifeTime = lifeTime;
        startPos = transform.position;

        activate = true;
        coll.enabled = true;
        
        ViewObj.SetActive(true);

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

        //�ʱ�ȭ �̺�Ʈ   >>view�� Gravity��ο��� ����
        InitProjectile();

        projectileMovement.StartMovement(speed);
        if (health != null) health.ResetHealth();
    }

    public override void init(float damage, float speed, float range, float lifeTime, Planet planet, bool isRight)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.lifeTime = lifeTime;
        startPos = transform.position;

        activate = true;
        coll.enabled = true;

        ViewObj.SetActive(true);

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

        //�ʱ�ȭ �̺�Ʈ   >>view�� Gravity��ο��� ����
        InitProjectile();

        if (isOrbital)
        {
            orbitMov.centerPlanet = planet;
            orbitMov.isRight = isRight;
        }

        projectileMovement.StartMovement(speed);


        //Projectile ü�� �ʱ�ȭ
        if (health != null)
        {
            health.ResetHealth();
        }
    }

    //�Ѿ� ���� �Ⱓ ���ȿ�
    protected override void Update()
    {
        base.Update();
    }

    //�Ѿ� ������ ������ �� 
    protected override void LifeTimeOver()
    {
        AfterHitEvent();
    }

    //�Ѿ��� ������ �¾��� �� 
    protected override void AfterHitEvent()
    {
        base.AfterHitEvent();
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //�Ѿ��� �浹���� �� 
        if (collision.CompareTag("Player"))
        {
            if(collision.TryGetComponent(out PlayerBehavior behavior))
            {
                //�÷��̾ ������ ���
                if (!behavior.activate)
                    return;

                //������ ����
                behavior.DamageEvent(damage);

                ShowHitEffect();
                AfterHitEvent();
            }
        }

        else if (collision.CompareTag("Obstacle"))
        {
            if (collision.TryGetComponent(out Obstacle obstacle))
            {
                obstacle.AnyDamage(damage, rb.velocity);
                ShowHitEffect();
                AfterHitEvent();
            }
        }

        //�༺�� �ε����� ��
        else if (collision.CompareTag("Planet")|| collision.CompareTag("SpaceBorder"))
        {
            ShowHitEffect();
            AfterHitEvent();
        }

    }



    // �Ѿ� ������ �¾��� �� 

    public virtual void DamageEvent(float dmg, Vector2 objVel)
    {
        //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

        if (!hitByProjectileOn)     //projectile-player �� ����üũ�̱� �ϴ�. (�ʿ����)
            return;
        if (health == null) return;

        if (health.AnyDamage(dmg))  //��� ���ظ� �Ծ����� true
        {
            HitFeedBack();   //Projectile �� �ִ�. View �� HitFeedback�� �����Ų��.
        }

        if (health.IsDead())
        {
            //�����θ� �ı�
            AfterHitEvent();
        }
        else
        {
            KnockBackEvent(objVel);
        }

    }


    void KnockBackEvent(Vector2 objVel)
    {
       projectileMovement.KnockBackEvent(objVel);
    }

}
