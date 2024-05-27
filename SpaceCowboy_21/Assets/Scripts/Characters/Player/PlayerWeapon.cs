using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData baseWeapon;   //기본 총기
    WeaponData curWeapon;
    //public GameObject artifactWeaponPrefab; //착용중인 무기 유물 프리팹. 

    bool infiniteBullets;    //총알이 무한
    int numberOfProjectile;    //한번 누를 때 멀티샷 수
    float shootInterval;    //발사 쿨타임
    float projectileSpread; //총알 마다 떨어진 각도
    float randomSpreadAngle;    //총구 흔들림때문에 생기는 랜덤값
    GameObject projectilePrefab;    //총알의 종류
    float damage;
    float speed;
    float lifeTime;
    float range;
    int projPenetration;
    int projReflection;
    int projGuide;
    AudioClip shootSFX;     //발사시 효과음

    public float maxAmmo { get; private set; }    //총알 탄창의 max수치
    public float currAmmo { get; private set; }     //현재 총의 총알

    //유물 추가 스텟
    float damagePlus;
    float fireRatePlus;
    float projSpeedPlus;
    float projRangePlus;
    int projNumberPlus;


    [SerializeField] float weaponChangeTime = 0.5f; //무기 교체하는데 걸리는 시간

    float lastShootTime;    //지난 발사 시간
    bool isChanging;    //무기를 바꾸는 중인가요?
    public bool shootON { get; set; }  //총 쏘기 시작

    PlayerBehavior playerBehavior;
    //ArtifactWeapon currArtifactWeapon; //현재 착용중인 무기 유물

    //public event System.Action weaponShoot;
    //public event System.Action<Vector2> weaponImpact;

    //Test
    //[SerializeField] GameObject awp;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

    }

    private void Start()
    {
        //ChangeArtifact(awp);
        playerBehavior.TryChangeWeapon(baseWeapon);
    }


    private void Update()   
    {
        if (!playerBehavior.activate) return;

        if (!shootON) return;

        //무기를 바꾸는 중이면 발사하지 않는다. 
        if (isChanging) return;

        //무한 탄창이 아니며, 총알이 없는 경우 발사하지 않는다. 
        if (!infiniteBullets && currAmmo < 1) return;

        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        TryShoot();
    }

    #region Shoot Function


    public void TryShoot() 
    {
        //쏠 때마다 총알 한발씩 제거
        if(!infiniteBullets) currAmmo -= 1;
        lastShootTime = Time.time;
        Shoot();

        ////유물 발사 효과 발동
        //ArtifactShoot();

        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

        // 총이 기본 총이 아니고, 총알을 다 쓴 경우 
        if (curWeapon != baseWeapon && currAmmo <= 0)
        {
            playerBehavior.TryChangeWeapon(baseWeapon);
        }
    }

    void Shoot()
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        //Vector3 dir = gunTip.right; //발사 각도
        Vector3 gunTipPos = playerBehavior.gunTipPos;
        Vector3 dir = playerBehavior.aimDirection;

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
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(damage, speed, lifeTime, range, projPenetration, projReflection, projGuide);
            
            ////오류 방지를 위해 한번 빼고 넣는다. 
            //proj.ProjImpactEvent -= ArtifactImpact;
            //proj.ProjImpactEvent += ArtifactImpact;
        }

        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }

    #endregion
   

    #region ChangeWeapon

    public void ChangeWeapon(WeaponData weaponData)
    {
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
        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        curWeapon = weaponData;
        projectilePrefab = weaponData.ProjectilePrefab;
        numberOfProjectile = weaponData.NumberOfProjectile;  
        shootInterval = weaponData.ShootInterval;   
        damage = weaponData.Damage;
        speed = weaponData.Speed;
        lifeTime = weaponData.LifeTime;
        range = weaponData.Range;
        projPenetration = weaponData.ProjPenetration;
        projReflection = weaponData.ProjReflection;
        projGuide = weaponData.ProjGuide;

        projectileSpread = weaponData.ProjectileSpread;
        randomSpreadAngle = weaponData.RandomSpreadAngle;
        shootSFX = weaponData.ShootSFX;



        currAmmo = weaponData.MaxAmmo;
        maxAmmo = weaponData.MaxAmmo;

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

    }

    //현재 총기스텟 적용.
    //void GetFinalStat()
    //{
    //    damage = damage * ((100 + damagePlus)/100);
    //    shootInterval = shootInterval * (100 / (100 + fireRatePlus));
    //    speed = speed * (100 + projSpeedPlus) / 100;
    //    range = range * (100 + projRangePlus) / 100;
    //    numberOfProjectile = projNumberPlus + 1;
    //}

    #endregion

    #region ArtifactWeapon

    //public void ChangeArtifact(GameObject artifactWeaponPrefab)
    //{
    //    if (currArtifactWeapon != null)
    //    {
    //        currArtifactWeapon.RemoveArtifactWeapon();
    //        Destroy(currArtifactWeapon.gameObject);
    //    }
    //    //유물 인스턴싱
    //    currArtifactWeapon = Instantiate(artifactWeaponPrefab, this.transform).GetComponent<ArtifactWeapon>();
    //    currArtifactWeapon.CreateArtifactWeapon(this);

    //    //유물 총기 스텟 적용
    //    this.damagePlus = currArtifactWeapon.damagePlus;
    //    this.fireRatePlus = currArtifactWeapon.fireRatePlus;
    //    this.projSpeedPlus = currArtifactWeapon.projSpeedPlus;
    //    this.projRangePlus = currArtifactWeapon.projRangePlus;
    //    this.projNumberPlus = currArtifactWeapon.projNumberPlus;
    //    this.projPenetrationPlus = currArtifactWeapon.projPenetrationPlus;
    //    this.projReflectionPlus = currArtifactWeapon.projReflectionPlus;
    //    this.projGuidePlus = currArtifactWeapon.projGuidePlus;

    //    //유물 기본 총기 적용
    //    baseWeapon = currArtifactWeapon.changedWeapon;
    //    //총기 교체
    //    playerBehavior.TryChangeWeapon(baseWeapon);


    //}

    

    //void ArtifactShoot()
    //{
    //    if (weaponShoot != null) weaponShoot();
    //}

    //public void ArtifactImpact(Vector2 pos)
    //{
    //    if(weaponImpact!= null) weaponImpact(pos); 
    //}
    #endregion

    //#region SkillShot
    //public void AddSkillAmmo(int amount)
    //{
    //    currSkillAmmo += amount;
    //    if (currSkillAmmo > maxSkillAmmo)
    //    {
    //        currSkillAmmo = maxSkillAmmo;
    //    }
    //}

    ////스킬을 사용한다
    //public void UseSkillShot(Vector3 pos, Vector2 dir)
    //{
    //    if (currSkillAmmo <= 0) return;
    //    currSkillAmmo -= 1;

    //    skillArtifact.SkillOperation(pos, dir);
    //}

    //#endregion


}
