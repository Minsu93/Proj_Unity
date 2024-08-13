using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WType_DangerCircleGun : WeaponType
{
    /// <summary>
    /// 공격 일정 거리에 DangerCircle이 생겨난다.
    /// 버튼을 누르면 해당 Circle내부의 적들에게 피해를 준다. 
    /// </summary>
    /// 

    [SerializeField] float dangerCircleDiameter = 1.0f;
    [SerializeField] float dangerCircleDistance = 5.0f;
    [SerializeField] Sprite circleSprite;
    [SerializeField] Material circleMat;
    [SerializeField] GameObject circleEffectObj;
    [SerializeField] ParticleSystem circleEffect;
    GameObject dangerCircleObj;
    int layerMask;

    public override void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        base.Initialize(weaponData, gunTipLocalPos);

        dangerCircleObj = new GameObject();

        SpriteRenderer circleSpr = dangerCircleObj.AddComponent<SpriteRenderer>();
        circleSpr.sprite = circleSprite;
        circleSpr.material = circleMat;
        circleSpr.sortingLayerName = "Effect";

        circleEffectObj.transform.parent = dangerCircleObj.transform;
        circleEffectObj.transform.localPosition = Vector3.zero;
        var mainModule = circleEffect.main;
        mainModule.startSize = dangerCircleDiameter;
       
        dangerCircleObj.transform.parent = this.transform;
        dangerCircleObj.transform.localPosition = gunTipLocalPos + (Vector3.right * dangerCircleDistance);
        dangerCircleObj.transform.localRotation = Quaternion.identity;
        dangerCircleObj.transform.localScale = Vector3.one * dangerCircleDiameter;

        layerMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyHitableProj") | 1 << LayerMask.NameToLayer("StageObject");


    }

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        //데미지 주기 
        Collider2D[] results = new Collider2D[15]; // 결과를 저장할 배열
        int count = Physics2D.OverlapCircleNonAlloc(dangerCircleObj.transform.position, dangerCircleDiameter * 0.5f, results, layerMask);
        for(int i = 0; i < count; i++)
        {
            Collider2D targetColl = results[i];
            if (targetColl.transform.TryGetComponent<IHitable>(out IHitable hitable))
            {
                hitable.DamageEvent(weaponStats.damage, targetColl.transform.position);

                ShowHitEffect(hitEffect, targetColl.transform.position);
            }
        }

        lastShootTime = Time.time;

        circleEffect.Play();

        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }
}
