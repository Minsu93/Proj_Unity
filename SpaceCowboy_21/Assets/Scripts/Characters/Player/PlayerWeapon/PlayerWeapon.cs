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
    public float maxAmmo { get; private set; }    //총알 탄창의 max수치
    public float currAmmo { get; private set; }     //현재 총의 총알
    [SerializeField] float weaponChangeTime = 0.5f; //무기 교체하는데 걸리는 시간
    public bool shootOn { get; set; }   //총쏘는 중인가요? 
    bool infiniteBullets;    //총알이 무한
    bool isChanging;    //무기를 바꾸는 중인가요?
    float range;    //거리 표시 용 

    //WeaponSight관련
    GameObject weaponRangeSight;     //사정거리 가늠자
    GameObject weaponRangePoint;    //가늠자 끝 spritePoint
    bool showWeaponRange { get; set; }  //총기 사정거리 보이게 할지 말지.
    bool pointOn;
    int targetLayer;
    [SerializeField] LineRenderer sightLineRenderer;

    //무기 슬롯
    Dictionary<WeaponData, WeaponType> weaponDictionary = new Dictionary<WeaponData, WeaponType>();
    
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
        GameManager.Instance.playerManager.UpdateGaugeUIShootTime(currAmmo);  //UI 에 전달

        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

        // 총알을 다 쓴 경우 
        if (!infiniteBullets && currAmmo <= 0)
        {
            GameManager.Instance.playerManager.ChangeWeapon(0);             //무기 기본으로 변경
        }
    }

    #endregion
   
    #region ChangeWeapon

    public void InitializeWeapon(WeaponData data)
    {
        WeaponType wtype;

        //현재 만들어진 무기가 있으면 가져온다. 그렇지 않으면 새로 생성한다. 
        if (weaponDictionary.ContainsKey(data))
        {
            weaponDictionary.TryGetValue(data, out wtype);
        }
        else
        {
            //생성
            GameObject weaponObj = Instantiate(data.WeaponPrefab, weaponSlot.transform);
            wtype = weaponObj.GetComponent<WeaponType>();
            wtype.Initialize(data);
            wtype.afterShootEvent -= AfterShootProcess;
            wtype.afterShootEvent += AfterShootProcess;
            weaponObj.SetActive(false);

            //Dict에 추가
            weaponDictionary.Add(data, wtype);
        }

        ChangeWeapon(wtype);
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        isChanging = true;
        shootOn = false;

        //현재 사용중인 무기는 숨긴다
        if(currWeaponType != null) 
            currWeaponType.gameObject.SetActive(false);

        //바꿀 무기의 현 정보를 불러온다(이때, 불러오는 정보들은 이미 갱신이 완료된 정보이다)
        currWeaponType = weaponType;
        currWeaponType.gameObject.SetActive(true);

        //스킨
        GunType finalSkin = currWeaponType.finalGunType;
        playerBehavior.TryChangeWeaponSkin(finalSkin);

        //이 스크립트(Player Weapon) 에서 필요한 Ammo, Range 갱신
        WeaponStats weaponStats = currWeaponType.weaponStats;

        currAmmo = weaponStats.maxAmmo;
        maxAmmo = weaponStats.maxAmmo;
        range = weaponStats.range;
        ShowWeaponSight(true);

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

        //무기 교체 쿨타임
        StartCoroutine(ChangePauseRoutine());
    }

    IEnumerator ChangePauseRoutine()
    {
        //총 바꾸는 데 걸리는 시간 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }


    #endregion



}


