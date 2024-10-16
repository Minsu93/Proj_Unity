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
    //protected WeaponStats baseStats = new WeaponStats();
    //protected WeaponStats weaponStats = new WeaponStats();
    public WeaponShootDelegate weaponShoot;
    public WeaponImpactDelegate weaponImpact;
    protected float damage;
    protected float speed;
    protected float shootInterval;
    protected float lifeTime;
    protected float projectileSpread;
    protected float randomSpreadAngle;
    protected float speedVariation;
    protected float burstInterval;
    protected int numberOfProjectile;
    protected int numberOfBurst;

    protected GameObject projectilePrefab;
    protected GameObject secondProjectilePrefab;
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
        //초기 설정
        damage = weaponData.Damage;
        speed = weaponData.Speed;
        shootInterval = weaponData.ShootInterval;
        
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
        secondProjectilePrefab = weaponData.SecondProjectilePrefab;
        showRange = weaponData.ShowRange;
        shootSFX = weaponData.ShootSFX;

        gunTipTr = transform.GetChild(0);
        gunTipTr.localPosition = gunTipLocalPos;
        
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
            float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
            proj.Init(damage, ranSpd, lifeTime, range);
            //총알에 Impact이벤트 등록
            if(weaponImpact != null)
            {
                proj.weaponImpactDel = weaponImpact;
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


    protected virtual void Shoot(Vector2 pos, Vector3 dir, GameObject projectilePrefab)
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
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
            proj.Init(damage, ranSpd, lifeTime, range);
            //총알에 Impact이벤트 등록
            if (weaponImpact != null)
            {
                proj.weaponImpactDel = weaponImpact;
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

    protected virtual IEnumerator burstShootRoutine(Vector2 pos, Vector3 dir, int repeatNumber, float interval)
    {
        for (int i = 0; i < repeatNumber; i++)
        {
            Shoot(pos, dir);
            yield return new WaitForSeconds(interval);
        }
    }

    protected void WeaponShootEvent()
    {
        if(weaponShoot != null)
        {
            weaponShoot();
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
//[System.Serializable]
//public class WeaponStats
//{
//    public float damage;
//    public float speed;
//    public float shootInterval;

//    public WeaponShootDelegate weaponShoot;
//    public WeaponImpactDelegate weaponImpact;
//}

public enum BarrelType { PingPong, InOrder }
