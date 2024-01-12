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

    //�μ����� �� ������
    public float disableDelayTime = 0.5f;

    [Header("Optional Properties")]

    //public bool rotateOn;       //�ӵ��� ���� ȸ���ϴ°�
    public bool gravityOn;      //�߷��� ������ �޴°�
    public bool collideOnPlanet;      //����(Planet)�� �浹�ϴ°�
    public bool hitDestroyOn;   //Ÿ�ٰ� �浹 �� ���ŵǴ°�, ���� ����
    public bool hitByProjectileOn;  //�÷��̾� �Ѿ˿� �´°�

    public bool activate; // �Ѿ��� �۵� �����Ǿ����� �˸��� ����

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
            //�༺ ����Ʈ �ʱ�ȭ
            projectileGravity.gravityPlanets.Clear();
        }

        projectileMovement.StartMovement(speed);
       
    }


    protected virtual void Update()
    {
        if (!activate)
            return;

        //lifeTime�� 0�� �Ǿ��� ��
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            LifeTimeOver();
        }

        //��ġ�� ���� �˻�.
    }

    protected virtual void LifeTimeOver()
    {   //�Ѿ� ������ ������ �� ������ ��
        AfterHitEvent();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       
    }


    protected virtual void AfterHitEvent()
    {
        //��ó��
        Instantiate(hitEffect, transform.position, Quaternion.identity);


        //�������� ����
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


    //Projectile View �� �����ϴ� �̺�Ʈ��

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
