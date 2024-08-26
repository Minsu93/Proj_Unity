using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_DamageCircle : Projectile_Player
{
    ///규칙적으로 범위 내 데미지
    ///
    [SerializeField] float circleDiameter = 1f;
    [SerializeField] float damageInterval = 0.5f;
    //[SerializeField] GameObject circleEffectObj;
    [SerializeField] ParticleSystem circleEffect;
    float timer = 0f;
    int layerMask;

    protected override void Awake()
    {
        base.Awake();
        
        layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyHitableProj") | 1 << LayerMask.NameToLayer("StageObject");
        
        ViewObj.transform.localScale = Vector3.one * circleDiameter;
        var mainModule = circleEffect.main;
        mainModule.startSize = circleDiameter;
    }
    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;

        if (lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();
        projectileMovement.StartMovement(speed);
    }
    public override void ResetProjectile()
    {
        base.ResetProjectile();
        timer = 0;
    }

    protected override void Update()
    {
        base.Update();
        if(timer < damageInterval)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            DamageCheck();
        }
    }

    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    return;
    //}

    void DamageCheck()
    {
        //데미지 주기 
        Collider2D[] results = new Collider2D[10]; // 결과를 저장할 배열
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, circleDiameter * 0.5f, results, layerMask);
        for (int i = 0; i < count; i++)
        {
            Collider2D targetColl = results[i];

            if (targetColl.TryGetComponent<ITarget>(out ITarget target))
            {
                if (targetColl.TryGetComponent<IHitable>(out IHitable hitable))
                {
                    HitEvent(target, hitable);
                }
            }
        }

        circleEffect.Play();
    }

    protected override void HitEvent(ITarget target, IHitable hitable)
    {
        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

    }
}
