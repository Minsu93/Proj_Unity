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
    public float disableDelayTime = 0.5f;    //�μ����� �� ������
    float delayTimer;

    //public bool rotateOn;       //�ӵ��� ���� ȸ���ϴ°�
    //public bool gravityOn;      //�߷��� ������ �޴°�
    //public bool collideOnPlanet;      //����(Planet)�� �浹�ϴ°�
    //public bool hitDestroyOn;   //Ÿ�ٰ� �浹 �� ���ŵǴ°�, ���� ����
    //public bool hitByProjectileOn;  //�÷��̾� �Ѿ˿� �´°�

    [Space]
    protected bool activate; // �Ѿ��� �۵� �����Ǿ����� �˸��� ����

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

    //�Ѿ� ����
    public virtual void init(float damage, float speed, float range, float lifeTime)
    {
        return;
    }

    public virtual void init(float damage, float speed, float range, float lifeTime, Planet planet, bool isRight)
    {
        return;
    }

    //�Ѿ��� �¾��� �� 
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
            //�Ѿ˿� ������ ���� �� 
            LifeTimeCheck();
        }
        else
        {
            //�Ѿ˿� ������ ���� �� 
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

    //�Ѿ� ������ ������ �� > ���������, Ȥ�� �� �ڸ����� ��������.
    protected virtual void LifeTimeOver()
    {
        AfterHitEvent();
    }

    //�Ÿ� ����
    void RangeCheck()
    {
        float dist = Vector2.Distance(startPos, transform.position);
        if (dist > range)
            AfterHitEvent();
    }

    //�浹 ����Ʈ
    protected void ShowHitEffect()
    {
        //����Ʈ ǥ��
        //Instantiate(hitEffect, transform.position, transform.rotation);
        ParticleHolder.instance.GetParticle(hitEffect, transform.position, transform.rotation);
    }

    //�浹 ��ó��
    protected virtual void AfterHitEvent()
    {
        //�������� ����
        projectileMovement.StopMovement();
                
        activate = false;
        coll.enabled = false;
        ViewObj.SetActive(false);

        //�ٸ� ��ũ��Ʈ�鿡 �̺�Ʈ ���� 
        HitFeedBack();

        //���� ���� ������ �ʱ�ȭ 
        delayTimer = disableDelayTime;
        
    }

    //������ �� ������Ʈ�� ������ ���� 
    void DisableObject()
    {
        if(trail != null)
            trail.enabled = false;
        gameObject.SetActive(false);
    }



    //�Ѿ� �浹 �� 
    protected void HitFeedBack()
    {
        if (ProjectileHitEvent != null)
            ProjectileHitEvent();
    }

    //�Ѿ� ��Ȱ��ȭ�� 
    protected void DeactivateProjectile()
    {
        if (DeactivateProjectileEvent != null)
            DeactivateProjectileEvent();
    }

    //�Ѿ� ���� �� 
    protected void InitProjectile()
    {
        if(ProjectileInitEvent != null) 
            ProjectileInitEvent();
    }


}
