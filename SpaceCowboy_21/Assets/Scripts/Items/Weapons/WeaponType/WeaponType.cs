using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;



public delegate void WeaponShootDelegate();
public delegate void WeaponImpactDelegate();


public abstract class WeaponType : MonoBehaviour
{
    /// <summary>
    /// 부모. 총 쏘는 방식이 들어있다. 
    /// </summary>

    [Header("Weapon Status")]
    //public int itemID;
    protected WeaponStats baseStats = new WeaponStats();
    protected WeaponStats weaponStats = new WeaponStats();
    protected float lifeTime;
    protected float projectileSpread;
    protected float randomSpreadAngle;
    protected float speedVariation;
    protected float burstInterval;
    protected int numberOfProjectile;
    protected int numberOfBurst;

    protected GameObject projectilePrefab;
    protected AudioClip shootSFX;  

    //PlayerWeapon에서 필요한 변수
    public WeaponData weaponData { get; set; }
    public bool showRange { get; set; }
    public float range { get; set; }
    public int maxAmmo { get; set; }
    public float weaponDuration { get; private set; }


    protected Transform gunTipTr;
    [SerializeField] ParticleSystem muzzleFlashVFX;


    protected float lastShootTime;   //지난 발사 시간
    
    public event System.Action afterShootEvent;     //총쏘기 후처리. ammo감소.
    

    //총 생성 시 실행
    public virtual void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        this.weaponData = weaponData;
        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        baseStats.damage = weaponData.Damage;
        baseStats.speed = weaponData.Speed;
        baseStats.shootInterval = weaponData.ShootInterval;
        
        burstInterval = weaponData.BurstInterval;
        projectileSpread = weaponData.ProjectileSpread;
        randomSpreadAngle = weaponData.RandomSpreadAngle;
        speedVariation = weaponData.SpeedVariation;
        numberOfProjectile = weaponData.NumberOfProjectile;
        numberOfBurst = weaponData.NumberOfBurst;
        lifeTime = weaponData.LifeTime;
        range = weaponData.Range;
        maxAmmo = weaponData.MaxAmmo;
        weaponDuration = weaponData.Duration;
        
        //기타 설정
        projectilePrefab = weaponData.ProjectilePrefab;
        showRange = weaponData.ShowRange;
        shootSFX = weaponData.ShootSFX;

        gunTipTr = transform.GetChild(0);
        gunTipTr.localPosition = gunTipLocalPos;
        
    }

    //이미 생성된 무기를 다시 꺼낼 때 , 버프 물약을 먹었을 때, 버프 물약을 해제했을 때.
    public virtual void ResetWeapon(WeaponStats bonusStats)
    {
        //베이스 Stat + 보너스 Stat
        WeaponStats totalStats = baseStats;

        if(bonusStats != null)
        {
            totalStats.damage *= (100 + bonusStats.damage) / 100;
            totalStats.speed *= (100 + bonusStats.speed) / 100;
            totalStats.shootInterval *= 100 / (100 + bonusStats.shootInterval);

            totalStats.weaponShoot = bonusStats.weaponShoot;
            totalStats.weaponImpact = bonusStats.weaponImpact;
        }

        //최종 적용한다.
        weaponStats = totalStats;
    }

    public abstract void ShootButtonDown(Vector2 pos, Vector3 dir);
    public abstract void ShootButtonUp(Vector2 pos, Vector3 dir);

    
    protected virtual void AfterShootProcess()
    {
        if (afterShootEvent != null) afterShootEvent();
    }


    //실제 쏘는 행동
    protected virtual void Shoot(Vector2 pos, Vector3 dir)
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab,0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            float ranSpd = UnityEngine.Random.Range(weaponStats.speed - speedVariation, weaponStats.speed + speedVariation);
            proj.Init(weaponStats.damage, ranSpd, lifeTime, range);
            //총알에 Impact이벤트 등록
            if(weaponStats.weaponImpact != null)
            {
                proj.weaponImpactDel = weaponStats.weaponImpact;
            }

        }

        //발사 시 이벤트 실행
        WeaponShootEvent();
        //MuzzleFlash 발사
        MuzzleFlashEvent(pos, targetRotation);
        //쏜 시간 체크
        lastShootTime = Time.time;
        //사운드 생성
        GameManager.Instance.audioManager.PlaySfx(shootSFX);

    }

    protected IEnumerator burstShootRoutine(Vector2 pos, Vector3 dir, int repeatNumber, float interval)
    {
        for (int i = 0; i < repeatNumber; i++)
        {
            Shoot(pos, dir);
            yield return new WaitForSeconds(interval);
        }
    }

    protected void WeaponShootEvent()
    {
        if(weaponStats.weaponShoot != null)
        {
            weaponStats.weaponShoot();
        }
    }

    protected void ShowHitEffect(ParticleSystem particle, Vector2 pos)
    {
        if (particle != null)
            GameManager.Instance.particleManager.GetParticle(particle, pos, transform.rotation);
    }

    //총구 불빛 VFX 
    protected void MuzzleFlashEvent(Vector3 pos, Quaternion quat)
    {
        if(muzzleFlashVFX == null) { return; }
        GameManager.Instance.particleManager.GetParticle(muzzleFlashVFX, pos, quat);
    }

}

//자손에 전달하여 기술큐브로 변화시킬 무기의 스텟들
[System.Serializable]
public class WeaponStats
{
    public float damage;
    public float speed;
    public float shootInterval;

    public WeaponShootDelegate weaponShoot;
    public WeaponImpactDelegate weaponImpact;
}

public enum BarrelType { PingPong, InOrder }
