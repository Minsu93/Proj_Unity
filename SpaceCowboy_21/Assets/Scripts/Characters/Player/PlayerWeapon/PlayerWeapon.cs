using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerWeapon : MonoBehaviour
{
    //총기 관련
    public WeaponData baseWeapon { get; set; }   //기본 총기
    WeaponData curWeapon;
    bool infiniteBullets;    //총알이 무한
    float range;    //거리 표시 용 
    public float maxAmmo { get; private set; }    //총알 탄창의 max수치
    public float currAmmo { get; private set; }     //현재 총의 총알


    [SerializeField] float weaponChangeTime = 0.5f; //무기 교체하는데 걸리는 시간
    bool isChanging;    //무기를 바꾸는 중인가요?
    public bool shootOn { get; set; }   //총쏘는 중인가요? 

    //WeaponSight관련
    bool showWeaponRange { get; set; }  //총기 사정거리 보이게 할지 말지.
    GameObject weaponRangeSight;     //사정거리 가늠자
    GameObject weaponRangePoint;    //가늠자 끝 spritePoint
    bool pointOn;
    int targetLayer;
    [SerializeField] LineRenderer sightLineRenderer;

    WeaponType[] weaponPrefabs = new WeaponType[4];
 
    
    //스크립트 관련
    PlayerBehavior playerBehavior;
    GameObject weaponSlot;  //WeaponType 보관 장소
    WeaponType currWeaponType;  //현재 웨폰 타입


    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        CreateWeaponRange();

        //weaponSight용 타겟 
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy");

        //무기 슬롯생성.
        weaponSlot = new GameObject();
        weaponSlot.name = "WeaponSlot";
        weaponSlot.transform.parent = transform;

    }
    private void Update()
    {
        UpdateWeaponSight();
    }

    #region WeaponSight
    //총기 사정거리 표시기 생성
    void CreateWeaponRange()
    {
        weaponRangeSight = sightLineRenderer.gameObject;

        weaponRangePoint = new GameObject("WeaponRangeSight");
        weaponRangePoint.transform.parent = weaponRangeSight.transform;
        SpriteRenderer spr = weaponRangePoint.AddComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>("UI/Circle16");
        spr.sortingLayerName = "Effect";

        weaponRangePoint.SetActive(false);
        weaponRangeSight.SetActive(false);
        showWeaponRange = false;
        pointOn = false;

    }
    public void ShowWeaponSight(bool visible)
    {
        weaponRangeSight.SetActive(visible);
        showWeaponRange = visible;
     
    }

    void UpdateWeaponSight()
    {
        if (showWeaponRange)
        {
            float targetDist;
            RaycastHit2D hit = Physics2D.Raycast(playerBehavior.gunTipPos, playerBehavior.aimDirection, range, targetLayer);
            if (hit.collider != null)
            {
                targetDist = hit.distance;
                pointOn = true;
            }
            else
            {
                targetDist = range;
                pointOn = false;
            }

            Vector2 pos0 = playerBehavior.gunTipPos;
            Vector2 pos1 = pos0 + (playerBehavior.aimDirection * targetDist * 0.2f);
            Vector2 pos2 = pos0 + (playerBehavior.aimDirection * targetDist * 0.8f);
            Vector2 pos3 = pos0 + (playerBehavior.aimDirection * targetDist);

            sightLineRenderer.SetPosition(0, pos0);
            sightLineRenderer.SetPosition(1, pos1);
            sightLineRenderer.SetPosition(2, pos2);
            sightLineRenderer.SetPosition(3, pos3);

            if (pointOn)
            {
                if (!weaponRangePoint.activeSelf)
                {
                    weaponRangePoint.SetActive(true);
                }
                weaponRangePoint.transform.position = pos3;
            }
            else
            {
                if (weaponRangePoint.activeSelf)
                {
                    weaponRangePoint.SetActive(false);
                }
            }

        }
    }

    #endregion

    #region Shoot Function

    public void ShootProcess() 
    {
        //무기를 바꾸는 중이면 발사하지 않는다. 
        if (isChanging) return;

        //무한 탄창이 아니며, 총알이 없는 경우 발사하지 않는다. 
        if (!infiniteBullets && currAmmo < 1) return;

        if (!shootOn) shootOn = true;

        //사격
        currWeaponType.ShootButtonDown(playerBehavior.gunTipPos, playerBehavior.aimDirection);

    }

    //총쏘기 초기화
    public void ShootOffProcess()
    {
        if(shootOn) shootOn = false;
        currWeaponType.ShootButtonUp(playerBehavior.gunTipPos, playerBehavior.aimDirection);
    }

    void AfterShootProcess()
    {
        //쏠 때마다 총알 한발씩 제거
        if (!infiniteBullets) currAmmo -= 1;
        //lastShootTime = Time.time;
        GameManager.Instance.playerManager.UpdateGaugeUIShootTime(currAmmo);  //UI 에 전달


        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

        // 총이 기본 총이 아니고, 총알을 다 쓴 경우 
        if (curWeapon != baseWeapon && currAmmo <= 0)
        {
            GameManager.Instance.playerManager.ResetPlayerWeaponUI();     //사용하던 무기 취소
            //GameManager.Instance.playerManager.ChangeWeapon(0);             //무기 기본으로 변경
            ChangeWeapon(0);
            //playerBehavior.TryChangeWeapon(baseWeapon);
        }
    }

    //void Shoot()
    //{
    //    float totalSpread = projectileSpread * (numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

    //    Vector3 gunTipPos = playerBehavior.gunTipPos;
    //    Vector3 dir = playerBehavior.aimDirection;

    //    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
    //    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

    //    //랜덤 각도 추가
    //    float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
    //    Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

    //    //멀티샷
    //    for (int i = 0; i < numberOfProjectile; i++)
    //    {
    //        Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

    //        //총알 생성
    //        GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
    //        projectile.transform.position = gunTipPos;
    //        projectile.transform.rotation = tempRot * randomRotation;
    //        Projectile proj = projectile.GetComponent<Projectile>();
    //        proj.Init(damage, speed, lifeTime, range, projPenetration, projReflection, projGuide);
    //    }

    //    //사운드 생성
    //    GameManager.Instance.audioManager.PlaySfx(shootSFX);
    //}

    #endregion
   
    #region ChangeWeapon
    //게임 시작 시 미리 무기를 생성해둔다. 무기의 종류를 게임중에 바꾸진 않으니까. 
    public void PreInitializeWeapons(WeaponInventory[] weapons)
    {
        if(weapons.Length > 4)
        {
            throw new Exception("무기의 수가 4보다 많습니다");
        }
        for(int i = 0; i < weapons.Length; i++)
        {
            GameObject weaponObj = Instantiate(weapons[i].weaponData.WeaponPrefab, weaponSlot.transform);
            WeaponType weaponType = weaponObj.GetComponent<WeaponType>();
            weaponType.Initialize(weapons[i].weaponData);
            weaponType.afterShootEvent -= AfterShootProcess;
            weaponType.afterShootEvent += AfterShootProcess;
            weaponPrefabs[i] = weaponType;
            weaponObj.SetActive(false);
        }
    }

    public void ChangeWeapon(int index)
    {
        isChanging = true;
        shootOn = false;
        //현재 사용중인 무기는 숨긴다
        if(currWeaponType != null) 
            currWeaponType.gameObject.SetActive(false);

        //바꿀 무기의 현 정보를 불러온다(이때, 불러오는 정보들은 이미 갱신이 완료된 정보이다)
        currWeaponType = weaponPrefabs[index];
        currWeaponType.gameObject.SetActive(true);

        //스킨
        GunType finalSkin = currWeaponType.finalGunType;
        playerBehavior.TryChangeWeaponSkin(finalSkin);

        //이 스크립트(Player Weapon) 에서 필요한 것들을 갱신.
        WeaponStats weaponStats = currWeaponType.weaponStats;
        InitializePlayerWeapon(currWeaponType.weaponData, weaponStats.range, weaponStats.maxAmmo);

        StartCoroutine(ChangePauseRoutine());
    }

    IEnumerator ChangePauseRoutine()
    {
        //총 바꾸는 데 걸리는 시간 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }

    //public void InitializeWeapon(WeaponData weaponData)
    //{
    //    //초기 설정(변수에 스크립터블 오브젝트 들어감)
    //    curWeapon = weaponData;
    //    range = weaponData.Range;

    //    currAmmo = weaponData.MaxAmmo;
    //    maxAmmo = weaponData.MaxAmmo;

    //    ShowWeaponSight(weaponData.ShowRange);

    //    if (maxAmmo == 0) infiniteBullets = true;
    //    else infiniteBullets = false;

    //    //weaponSlot 에 BP 생성. 
    //    if(currWeaponType != null) Destroy(currWeaponType.gameObject);  //이전 웨폰은 제거
    //    GameObject weaponObj = Instantiate(weaponData.WeaponPrefab, weaponSlot.transform);
    //    currWeaponType = weaponObj.GetComponent<WeaponType>();
    //    currWeaponType.Initialize(weaponData);
    //    currWeaponType.afterShootEvent += AfterShootProcess;


    //}
    void InitializePlayerWeapon(WeaponData weaponData, float range,int maxAmmo)
    {
        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        curWeapon = weaponData;

        currAmmo = maxAmmo;
        this.maxAmmo = maxAmmo;

        this.range = range;
        ShowWeaponSight(true);

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



}


