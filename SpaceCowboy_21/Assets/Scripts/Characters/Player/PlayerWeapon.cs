using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData baseWeapon;   //기본 총기


    bool infiniteBullets;    //총알이 무한
    //float baseAmmo; //기본 무기의 총알 수 
    //float subAmmo; //특수 무기의 총알 수 
    //bool isSubWeapon; //특수 무기인가요?

    //리로드 관련
    //bool reloadOn = true;  //리로드를 진행하나요?
    //[Tooltip("발사 후 충전 시작까지 걸리는 시간")]
    //public float reloadTimer = 1.0f;    //발사 후 충전 시작까지 걸리는 시간 
    //[Tooltip("충전 속도")]
    //public float reloadSpeed = 3.0f;    //충전 속도
    //float rTimer;

    //총기 관련
    int burstNumber; //한번 누를 때 연속 발사 수
    int numberOfProjectiles;    //한번 누를 때 멀티샷 수
    float shootInterval;    //발사 쿨타임
    float burstInterval;    //연속 발사 시 사이 시간
    float projectileSpread; //총알 마다 떨어진 각도
    float randomSpreadAngle;    //총구 흔들림때문에 생기는 랜덤값
    AudioClip shootSFX;     //발사시 효과음

    GameObject projectilePrefab;    //총알의 종류
    float damage, speed,lifeTime;  // Projectile 수치들
    public float maxAmmo { get; private set; }    //총알 탄창의 max수치
    public float currAmmo { get; private set; }     //현재 총의 총알
    int reflectionCount;    //반사 횟수

    public float weaponChangeTime = 0.5f; //무기 교체하는데 걸리는 시간

    float lastShootTime;    //지난 발사 시간
    bool isChanging;    //무기를 바꾸는 중인가요?
    public bool shootON { get; set; }  //총 쏘기 시작


    PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
    }


    private void Update()   
    {
        //baseAmmo는 계속 회복된다
        if (!playerBehavior.activate) return;

        ////총알 충전(리로드)
        //if (!shootON)
        //{
        //    if(!isSubWeapon)
        //    {
        //        if (currAmmo < maxAmmo)
        //        {
        //            if (rTimer > 0)
        //            {
        //                rTimer -= Time.deltaTime;
        //            }
        //            else
        //            {
        //                currAmmo += Time.deltaTime * reloadSpeed;
        //            }
        //        }
        //    }

        //    return;
        //}
        if(!shootON) return;

        //무기를 바꾸는 중이면 발사하지 않는다. 
        if (isChanging)
            return;

        //총알이 없는 경우 발사하지 않는다. 
        if(currAmmo < 1)
        {
            return;
        }

        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        TryShoot();

        //발사 중지
        //shootON = false;
    }

    #region Shoot Function


    public void TryShoot()  //이벤트 발생을 위해서 > PlayerWeapon, PlayerView 의 ShootEvent를 발생
    {
        //쏠 때마다 총알 한발씩 제거
        if(!infiniteBullets) currAmmo -= 1;

        lastShootTime = Time.time;
        //rTimer = reloadTimer;

        //발사
        Shoot();

        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

        // 총알을 다 쓴 경우 
        if (currAmmo <= 0)
        {
            //ChangeWeapon(baseWeapon);
            playerBehavior.TryChangeWeapon(0);
        }


    }

    void Shoot()
    {
        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        //Vector3 dir = gunTip.right; //발사 각도
        Vector3 gunTipPos = playerBehavior.gunTipPos;
        Vector3 dir = playerBehavior.aimDirection;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //총알 생성
            GameObject projectile = PoolManager.instance.Get(projectilePrefab);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = tempRot * randomRotation;
            projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);
        }

        AudioManager.instance.PlaySfx(shootSFX);
    }

    #endregion

   
    #region ChangeWeapon

    public void ChangeWeapon(WeaponData weaponData)
    {
        //isSubWeapon = weaponData != baseWeapon;
        lastShootTime = 0f;

        //원하는 총으로 바꿔준다.
        InitializeWeapon(weaponData);

        StartCoroutine(ChangeWeaponRoutine());

    }

    IEnumerator ChangeWeaponRoutine()
    {
        isChanging = true;
        //총 바꾸는 데 걸리는 시간 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }

    public void InitializeWeapon(WeaponData weaponData)
    {
        //스킨을 변경
        //playerBehavior.TryChangeWeapon(weaponData.GunType);

        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        burstNumber = weaponData.BurstNumer; 
        burstInterval = weaponData.BurstInterval;    
        numberOfProjectiles = weaponData.NumberOfProjectiles;  
        shootInterval = weaponData.ShootInterval;   
        projectileSpread = weaponData.ProjectileSpread; 
        randomSpreadAngle = weaponData.RandomSpreadAngle;
        shootSFX = weaponData.ShootSFX;

        projectilePrefab = weaponData.ProjectilePrefab; 
        damage = weaponData.Damage;
        speed = weaponData.Speed;
        lifeTime = weaponData.LifeTime;
        reflectionCount = weaponData.ReflectionCount;

        currAmmo = weaponData.MaxAmmo;
        maxAmmo = weaponData.MaxAmmo;

        infiniteBullets = weaponData.InfiniteAmmo;

    }

    #endregion


}
