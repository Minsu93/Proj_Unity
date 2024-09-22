using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_E_AntMine : Projectile_GuideMissile
{
    ///// <summary>
    ///// 플레이어가 밟았을 때(닿았을 때), 플레이어의 총에 맞았을 때 터진다. 
    ///// 행성에 맞으면 거기에 붙는다. 
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

    //    //초기화 이벤트   >>view나 Gravity모두에게 전달
    //    InitProjectile();

    //    projectileMovement.StartMovement(speed);

    //    //Projectile 체력 초기화
    //    if (health != null)
    //    {
    //        health.ResetHealth();
    //    }
    //}

    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!activate) return;

    //    //총알이 충돌했을 때 
    //    if (collision.CompareTag("Player"))
    //    {
    //        if (collision.TryGetComponent(out PlayerBehavior behavior))
    //        {
    //            //플레이어가 죽으면 통과
    //            if (!behavior.activate)
    //                return;

    //            //데미지 전달
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

    //    //행성과 부딪혔을 때
    //    else if (collision.CompareTag("Planet") || collision.CompareTag("SpaceBorder"))
    //    {
    //        StickToPlanet();
    //    }

    //}

    ////행성에 달라붙는다.
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
    //    //피격시 발동하는 이벤트

    //    if (!hitByProjectileOn)     //projectile-player 과 더블체크이긴 하다. (필요없음)
    //        return;
    //    if (health.AnyDamage(dmg))  //어떠한 피해를 입었으면 true
    //    {
    //        //HitFeedBack();   //Projectile 에 있다. View 의 HitFeedback을 실행시킨다.
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
