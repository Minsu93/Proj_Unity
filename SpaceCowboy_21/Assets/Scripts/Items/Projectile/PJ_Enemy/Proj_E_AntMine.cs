using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_E_AntMine : Projectile_GuideMissile
{
    ///// <summary>
    ///// �÷��̾ ����� ��(����� ��), �÷��̾��� �ѿ� �¾��� �� ������. 
    ///// �༺�� ������ �ű⿡ �ٴ´�. 
    ///// </summary>
    ///// <param name="collision"></param>
    ///// 

    ////protected ProjectileGravity projGravity;
    //public float maxScale = 2.0f;
    //public float expandTime = 3.0f;

    //Collider2D hitColl;

    //protected override void Awake()
    //{
    //    base.Awake();
    //    //projGravity = GetComponent<ProjectileGravity>();
    //    hitColl = GetComponentsInChildren<Collider2D>()[1];
    //}
    //public override void init(float damage, float speed, float range, float lifeTime)
    //{
    //    this.damage = damage;
    //    this.speed = speed;
    //    this.range = range;
    //    this.lifeTime = lifeTime;
    //    startPos = transform.position;

    //    activate = true;
    //    coll.enabled = true;
    //    //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyHitableProj"), LayerMask.NameToLayer("PlayerProj"), true);

    //    ViewObj.SetActive(true);
    //    hitColl.enabled = false;
    //    transform.localScale = Vector3.one;

    //    if (trail != null)
    //    {
    //        trail.enabled = true;
    //        trail.Clear();
    //    }

    //    //�ʱ�ȭ �̺�Ʈ   >>view�� Gravity��ο��� ����
    //    InitProjectile();

    //    projectileMovement.StartMovement(speed);

    //    //Projectile ü�� �ʱ�ȭ
    //    if (health != null)
    //    {
    //        health.ResetHealth();
    //    }
    //}

    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!activate) return;

    //    //�Ѿ��� �浹���� �� 
    //    if (collision.CompareTag("Player"))
    //    {
    //        if (collision.TryGetComponent(out PlayerBehavior behavior))
    //        {
    //            //�÷��̾ ������ ���
    //            if (!behavior.activate)
    //                return;

    //            //������ ����
    //            behavior.DamageEvent(damage);

    //            ShowHitEffect();
    //            AfterHitEvent();
    //        }
    //    }

    //    else if (collision.CompareTag("Obstacle"))
    //    {
    //        if (collision.TryGetComponent(out Obstacle obstacle))
    //        {
    //            obstacle.AnyDamage(damage, rb.velocity);
    //            ShowHitEffect();
    //            AfterHitEvent();
    //        }
    //    }

    //    //�༺�� �ε����� ��
    //    else if (collision.CompareTag("Planet") || collision.CompareTag("SpaceBorder"))
    //    {
    //        StickToPlanet();
    //    }

    //}

    ////�༺�� �޶�ٴ´�.
    //void StickToPlanet()
    //{
    //    projectileMovement.StopMovement();
    //    //projGravity.activate = false;
    //    //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyHitableProj"), LayerMask.NameToLayer("PlayerProj"), false);
    //    hitColl.enabled = true;
    //    StartCoroutine(expandRoutine());

    //}

    //IEnumerator expandRoutine()
    //{
    //    float scale = 1f;

    //    while(scale <= maxScale)
    //    {
    //        scale += Time.deltaTime / expandTime * (maxScale -1);
    //        transform.localScale = Vector3.one * scale;
    //        yield return null;
    //    }
    //}
    //public override void DamageEvent(float dmg, Vector2 objVel)
    //{
    //    //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

    //    if (!hitByProjectileOn)     //projectile-player �� ����üũ�̱� �ϴ�. (�ʿ����)
    //        return;
    //    if (health.AnyDamage(dmg))  //��� ���ظ� �Ծ����� true
    //    {
    //        //HitFeedBack();   //Projectile �� �ִ�. View �� HitFeedback�� �����Ų��.
    //    }
    //    if (health.IsDead())
    //    {
    //        ShowHitEffect();
    //        AfterHitEvent();
    //    }

    //}

    //protected override void AfterHitEvent()
    //{
    //    base.AfterHitEvent();
    //    hitColl.enabled = false;
    //    StopAllCoroutines();
    //}
}
