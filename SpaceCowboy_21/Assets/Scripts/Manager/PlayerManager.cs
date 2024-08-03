using SpaceCowboy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class PlayerManager : MonoBehaviour
{
    //인터렉트 오브젝트
    public InteractableOBJ curObj { get; set; }

    //플레이어 UI관련
    [SerializeField] private GameObject playerUIPrefab;
    //[SerializeField] private GameObject playerUIInteractPrefab;
    Image healthImg;
    Image shieldImg;
    //WeaponWheelController weaponWheelController;
    //마우스 포인트를 따라다니는 게이지 UI관련
    Gauge_Weapon gauge_Weapon;

    //저장할 플레이어 정보
    //[SerializeField] WeaponData firstWeaponData;
    //[SerializeField] WeaponData secondWeaponData;
    //[SerializeField] WeaponData thirdWeaponData;
    //[SerializeField] WeaponData fourthWeaponData;

    //웨펀 인벤토리
    //public WeaponInventory[] weaponInventory = new WeaponInventory[4];
    //WeaponData[] weaponInventory = new WeaponData[4];
    //public WeaponData baseWeaponData;

    //플레이어 관련 스크립트
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    //ArtifactPopperSlot playerOrbSlot;
    //ArtifactBombSlot playerThrowSlot;
    PlayerHealth playerHealth;
    public PlayerBuffs playerBuffs { get; private set; }
    public PlayerWeapon playerWeapon { get; private set; }



    //플레이어 스크립트 업데이트

    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();
        playerBuffs = playerObj.GetComponent<PlayerBuffs>();

        playerObj.GetComponent<PlayerBehavior>().InitPlayer(this);

        //UI를 업데이트한다. 
        SpawnPlayerUI();

        //Camera를 업데이트한다.
        GameManager.Instance.cameraManager.InitCam();

        //저장 정보를 불러온다.
        //LoadPlayerInfo();

        //플레이어의 기본 무기를 장착시킨다.
        //ChangeWeapon(0);
        playerWeapon.BackToBaseWeapon();
    }

    //플레이어가 사용하는 장비, 유물, 업그레이드 수치 등을 저장한다.
    //public void SavePlayerInfo()
    //{
    //    string path = Path.Combine(Application.dataPath , "Data/PlayerData", "playerData.json");

    //    //Test

    //    PlayerData pData = new PlayerData();
    //    pData.weaponName = weaponData.name;
    //    pData.orbName = orbData.name;
    //    pData.throwSatelliteName = throwData.name;

    //    string str = JsonUtility.ToJson(pData, true);
    //    File.WriteAllText(path, str);
    //}

    //public void LoadPlayerInfo()
    //{
    //    string path = Path.Combine(Application.dataPath, "Data/PlayerData", "weaponData.json");
    //    string loadJson = File.ReadAllText(path);
    //    PlayerData pData = JsonUtility.FromJson<PlayerData>(loadJson);

    //    WeaponData first = Resources.Load<WeaponData>("Weapon/" + pData.firstWeaponName);
    //    WeaponData second = Resources.Load<WeaponData>("Weapon/" + pData.secondWeaponName);
    //    WeaponData third = Resources.Load<WeaponData>("Weapon/" + pData.thirdWeaponName);
    //    WeaponData fourth = Resources.Load<WeaponData>("Weapon/" + pData.fourthWeaponName);

    //    //인벤토리를 지정합니다.
    //    if(first != null)
    //    {
    //        SetInventory(first, 0);
    //    }
    //    if(second != null)
    //    {
    //        SetInventory(second, 1);
    //    }
    //    if(third != null)
    //    {
    //        SetInventory(third, 2);
    //    }
    //    if(fourth != null)
    //    {
    //        SetInventory(fourth, 3);
    //    }
    //}

    //void SetInventory(WeaponData weaponData, int index)
    //{
    //    weaponInventory[index] = weaponData;
    //}

    //BaseWeapon 들로 변경할 때 
    //public void ChangeWeapon(int index)
    //{
    //    WeaponData data = weaponInventory[index];
    //    //무기 변경
    //    playerWeapon.InitializeWeapon(data);

    //    //UI 전달
    //    UpdatePlayerGaugeUI(data);
    //}

    //신규 WeaponData로 변경할 때 
    public bool ChangeWeapon(WeaponData weaponData)
    {
        bool canPush = playerWeapon.PushWeapon(weaponData);
        //Ammo 게이지 UI전달
        UpdatePlayerGaugeUI(weaponData);
        return canPush;
    }

    public bool HealthUp(float amount)
    {
        return playerBehavior.healEvent(amount);
    }
    #region Player Input 관련

    //플레이어 입력 관련 
    public void DisablePlayerInput()
    {
        playerInput.inputDisabled = true;
    }
    public void EnablePlayerInput()
    {
        playerInput.inputDisabled = false;
    }
    public void DisablePlayerShoot()
    {
        playerInput.shootDisabled = true;
    }
    public void EnablePlayerShoot()
    {
        playerInput.shootDisabled = false;
    }
    #endregion

    #region Interaction

    //상호작용 관련

    public void SetInteractableObj(InteractableOBJ iObj)
    {
        //상호작용하던 물건이 있다면 상호작용 취소
        if(curObj != null)
        {
            curObj.StopInteract();
        }
        //새로 할당
        curObj = iObj;
    }

    public void RemoveInteractableObj(InteractableOBJ iObj)
    {
        if(curObj == iObj)
        {
            curObj.StopInteract();
            curObj = null;
        }
    }
    public void InteractSomething()
    {
        if (curObj == null)
            return;

        //플레이어 Cancel만 가능하도록 조작 변경.
        //DisablePlayerInput();
        curObj.InteractAction();
    }

    #endregion

    #region  UI관련

    //public void OpenWeaponWheel()
    //{
    //    weaponWheelController.ToggleWheel(true);
    //}
    //public void CloseWeaponWheel()
    //{
    //    weaponWheelController.ToggleWheel(false);
    //}

    //총을 다 쓰고 나서 초기화시키기
    //public void ResetPlayerWeaponUI()
    //{
    //    weaponWheelController.CancelItem();
    //}


    //Orb 충전
    //public void ChargeFireworkEnergy()
    //{
    //    playerOrbSlot.EnergyIncrease(1f);
    //}


    //player UI 스폰
    void SpawnPlayerUI()
    {
        GameObject pUI = Instantiate(playerUIPrefab);
        healthImg = pUI.transform.Find("StatusPanel/HealthGauge").GetComponent<Image>();
        shieldImg = pUI.transform.Find("StatusPanel/ShieldGauge").GetComponent<Image>();
        gauge_Weapon = pUI.transform.Find("GaugeController").GetComponent<Gauge_Weapon>();
        //GameObject pUI_Int = Instantiate(playerUIInteractPrefab);
        //weaponWheelController = pUI_Int.transform.Find("WeaponWheel").GetComponent<WeaponWheelController>();
    }

    //변화가 있을 때만 업데이트한다.
    public void UpdatePlayerStatusUI()
    {
        healthImg.fillAmount = playerHealth.currHealth / playerHealth.maxHealth;
        shieldImg.fillAmount = playerHealth.currShield / playerHealth.maxShield;
    }


    //무기 게이지 관련 
    public void UpdateGaugeUIShootTime(float curAmmo)
    {
        gauge_Weapon.ListenShoot(curAmmo);
    }

    //무기가 바뀌었을 때 게이지 수정
    public void UpdatePlayerGaugeUI(WeaponData data)
    {
        gauge_Weapon.WeaponChange(playerWeapon, data.MaxAmmo);
    }

    //게이지가 reticle 따라다니게 만들기
    public void SetReticleFollower(GameObject reticleObj)
    {
        gauge_Weapon.setFollowTarget(reticleObj);
    }

    #endregion
}


//저장할 데이터

//[Serializable]
//public class PlayerData
//{
//    public string firstWeaponName;
//    public string secondWeaponName;
//    public string thirdWeaponName;
//    public string fourthWeaponName;
//}

//[Serializable]

//public class WeaponInventory
//{
//    public WeaponData weaponData;
//    public int maxWeaponEnergy;
//    public int curWeaponEnergy;
//    public bool activate = false;
    
//    public void CollectEnergy(int i)
//    {
//        curWeaponEnergy += i;

//        if(curWeaponEnergy >= maxWeaponEnergy)
//        {
//            curWeaponEnergy = maxWeaponEnergy;
//            activate = true;
//        }
//    }

//    public void Use()
//    {
//        curWeaponEnergy = 0;
//        if(weaponData.MaxAmmo != 0)
//        {
//            activate = false;
//        }
//    }
//}