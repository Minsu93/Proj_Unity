using SpaceCowboy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//자손에 전달하여 기술큐브로 변화시킬 무기의 스텟들
[System.Serializable]
public class WeaponStats
{
    //public GameObject projectilePrefab;
    public float damage;
    public float speed;
    public float lifeTime;
    public float range;
    public float shootInterval;
    public float projectileSpread;
    public float randomSpreadAngle;
    public int numberOfProjectile;
    public int projPenetration;
    public int projReflection;
    public int projGuide;
    public int maxAmmo;

    public WeaponShootDelegate weaponShoot;
    public WeaponImpactDelegate weaponImpact;
}

public delegate void WeaponShootDelegate();
public delegate void WeaponImpactDelegate();

public abstract class WeaponType : MonoBehaviour
{
    /// <summary>
    /// 부모. 총 쏘는 방식이 들어있다. 
    /// </summary>


    [Header("Weapon Status")] 
    public WeaponStats weaponStats = new WeaponStats();

    //protected int numberOfProjectile;    
    //protected float shootInterval;    
    //protected float projectileSpread; 
    //protected float randomSpreadAngle;   
    //protected float damage;
    //protected float speed;
    //protected float lifeTime;
    //protected int projPenetration;
    //protected int projReflection;
    //protected int projGuide;
    protected GameObject projectilePrefab;
    protected AudioClip shootSFX;     //발사시 효과음

    //PlayerWeapon에서 필요한 변수
    public WeaponData weaponData { get; set; }
    //public float range { get; set; }
    //public int maxammo { get; set; }
    public bool showRange { get; set; }
    public GunType finalGunType { get; set; }

    protected float lastShootTime;   //지난 발사 시간
    public event System.Action afterShootEvent;     //총쏘기 후처리. ammo감소.
    

    //protected WeaponCubeSlots cubeSlots;

    private void Awake()
    {
        //cubeSlots = GetComponent<WeaponCubeSlots>();    
    }


    //총 생성 시, 큐브 슬롯 갱신 시 실행
    public virtual void Initialize(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        WeaponStats baseStats = new WeaponStats();
        baseStats.damage = weaponData.Damage;
        baseStats.speed = weaponData.Speed;
        baseStats.lifeTime = weaponData.LifeTime;
        baseStats.range = weaponData.Range;
        baseStats.shootInterval = weaponData.ShootInterval;
        baseStats.projectileSpread = weaponData.ProjectileSpread;
        baseStats.randomSpreadAngle = weaponData.RandomSpreadAngle;
        baseStats.numberOfProjectile = weaponData.NumberOfProjectile;
        baseStats.projPenetration = weaponData.ProjPenetration;
        baseStats.projReflection = weaponData.ProjReflection;
        baseStats.projGuide = weaponData.ProjGuide;
        baseStats.maxAmmo = weaponData.MaxAmmo;

        projectilePrefab = weaponData.ProjectilePrefab;
        showRange = weaponData.ShowRange;
        finalGunType = weaponData.GunType;

        shootSFX = weaponData.ShootSFX;

        //현재 갖고있는 기술Cube들의 스텟을 적용한다. 
        //weaponStats = cubeSlots.UpdateStats(baseStats);

        //Combination 을 확인한다
        //CheckCombination();
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
        float totalSpread = weaponStats.projectileSpread * (weaponStats.numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-weaponStats.randomSpreadAngle * 0.5f, weaponStats.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < weaponStats.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, weaponStats.projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(weaponStats.damage, weaponStats.speed, weaponStats.lifeTime, weaponStats.range, weaponStats.projPenetration, weaponStats.projReflection, weaponStats.projGuide);
            
            if(weaponStats.weaponImpact != null)
            {
                proj.weaponImpactDel = weaponStats.weaponImpact;
            }

        }

        //발사 시 이벤트 실행
        WeaponShootEvent();
        //쏜 시간 체크
        lastShootTime = Time.time;
        //사운드 생성
        GameManager.Instance.audioManager.PlaySfx(shootSFX);

    }

    
    void WeaponShootEvent()
    {
        if(weaponStats.weaponShoot != null)
        {
            weaponStats.weaponShoot();
        }
    }

    //void CheckCombination()
    //{
    //    int index = cubeSlots.CheckSpecialCombinations();
    //    switch (index)
    //    {
    //        case 0:
    //            //변화형 없음
    //            break;
    //        case 1:
    //            //변화형 A
    //            Varitation_A();
    //            break;
    //        case 2:
    //            //변화형 A
    //            Varitation_B();
    //            break;
    //        case 3:
    //            //변화형 A
    //            Varitation_C();
    //            break;
    //    }
    //}

    ////변화형
    //protected virtual void Varitation_A()
    //{
    //    Debug.Log("AB 결합 무기");
    //}
    //protected virtual void Varitation_B()
    //{
    //    Debug.Log("AC 결합 무기");
    //}
    //protected virtual void Varitation_C()
    //{
    //    Debug.Log("BC 결합 무기");
    //}
}
