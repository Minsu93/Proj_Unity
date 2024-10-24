using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;



public delegate void WeaponShootDelegate();
public delegate void WeaponImpactDelegate(Transform impactTr);


public abstract class WeaponType : MonoBehaviour
{
    /// <summary>
    /// 부모. 총 쏘는 방식이 들어있다. 
    /// </summary>

    [Header("Weapon Status")]
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

    protected Transform gunTipTr;
    [SerializeField] ParticleSystem muzzleFlashVFX;

    protected float lastShootTime;   //지난 발사 시간
    
    public event System.Action afterShootEvent;     //총쏘기 후처리. ammo감소.


    private void OnEnable()
    {
        GameManager.Instance.playerManager.ChangeTierEvent += InitializeByTier;
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.playerManager.ChangeTierEvent -= InitializeByTier;
    }

    //총 생성 시 실행
    public virtual void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        this.weaponData = weaponData;
        //초기 설정
        //InitializeByTier();


        maxAmmo = weaponData.MaxAmmo;
        showRange = weaponData.ShowRange;
        shootSFX = weaponData.ShootSFX;

        gunTipTr = transform.GetChild(0);
        gunTipTr.localPosition = gunTipLocalPos;
        
    }

    public virtual void InitializeByTier()
    {
        int tier = PM_LuckLevel.itemTier;

        damage = weaponData.ProjectDatas[tier].damage;
        speed = weaponData.ProjectDatas[tier].speed;
        shootInterval = weaponData.ProjectDatas[tier].shootInterval;
        burstInterval = weaponData.ProjectDatas[tier].burstInterval;
        projectileSpread = weaponData.ProjectDatas[tier].projectileSpread;
        randomSpreadAngle = weaponData.ProjectDatas[tier].randomSpreadAngle;
        speedVariation = weaponData.ProjectDatas[tier].speedVariation;
        numberOfProjectile = weaponData.ProjectDatas[tier].numberOfProjectile;
        numberOfBurst = weaponData.ProjectDatas[tier].numberOfBurst;
        lifeTime = weaponData.ProjectDatas[tier].lifeTime;
        range = weaponData.ProjectDatas[tier].range;
        projectilePrefab = weaponData.ProjectDatas[tier].projectilePrefab;
        secondProjectilePrefab = weaponData.ProjectDatas[tier].secondProjectilePrefab;

        //스킨
        GameManager.Instance.playerManager.playerBehavior.TryChangeWeaponSkin(weaponData);
    }



    public abstract void ShootButtonDown(Vector2 pos, Vector3 dir);
    public abstract void ShootButtonUp(Vector2 pos, Vector3 dir);

    
    protected virtual void AfterShootProcess()
    {
        if (afterShootEvent != null) afterShootEvent();
    }


    //실제 쏘는 행동


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
    protected void SingleShoot(Vector2 pos, Vector3 dir, GameObject projectilePrefab)
    {
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //총알 생성
        GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
        projectile.transform.position = pos;
        projectile.transform.rotation = targetRotation * randomRotation;
        Projectile proj = projectile.GetComponent<Projectile>();
        float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
        proj.Init(damage, ranSpd, lifeTime, range);
        //총알에 Impact이벤트 등록
        if (weaponImpact != null)
        {
            proj.weaponImpactDel = weaponImpact;
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


public enum BarrelType { PingPong, InOrder }
