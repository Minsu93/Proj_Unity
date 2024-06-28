using SpaceCowboy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class PlayerManager : MonoBehaviour
{
    //인터렉트 오브젝트
    public InteractableOBJ curObj { get; set; }

    //플레이어 UI관련
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerUIInteractPrefab;
    Image healthImg;
    Image shieldImg;
    WeaponWheelController weaponWheelController;
    Gauge_Weapon gauge_Weapon;

    //저장할 플레이어 정보
    [SerializeField] WeaponData firstWeaponData;
    [SerializeField] WeaponData secondWeaponData;
    [SerializeField] WeaponData thirdWeaponData;
    [SerializeField] WeaponData fourthWeaponData;
    //[SerializeField] GameObject orbData;
    //[SerializeField] GameObject throwData;

    //웨펀 인벤토리
    public WeaponInventory[] weaponInventory = new WeaponInventory[4];

    //플레이어 관련 스크립트
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    //ArtifactPopperSlot playerOrbSlot;
    //ArtifactBombSlot playerThrowSlot;
    PlayerHealth playerHealth;
    public PlayerWeapon playerWeapon { get; private set; }



    //플레이어 스크립트 업데이트

    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        //playerOrbSlot = playerObj.GetComponent<ArtifactPopperSlot>();
        //playerThrowSlot = playerObj.GetComponent<ArtifactBombSlot>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();

        playerObj.GetComponent<PlayerBehavior>().InitPlayer(this);

        //UI를 업데이트한다. 
        SpawnPlayerUI();

        //Camera를 업데이트한다.
        GameManager.Instance.cameraManager.InitCam();

        //저장 정보를 불러온다.
        LoadPlayerInfo();

        //플레이어의 무기들을 스폰한다
        playerWeapon.PreInitializeWeapons(weaponInventory);

        //플레이어의 기본 무기를 장착시킨다.
        ChangeWeapon(0);
        playerWeapon.baseWeapon = weaponInventory[0].weaponData;
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

    public void LoadPlayerInfo()
    {
        string path = Path.Combine(Application.dataPath, "Data/PlayerData", "weaponData.json");
        string loadJson = File.ReadAllText(path);
        PlayerData pData = JsonUtility.FromJson<PlayerData>(loadJson);

        WeaponData first = Resources.Load<WeaponData>("Weapon/" + pData.firstWeaponName);
        WeaponData second = Resources.Load<WeaponData>("Weapon/" + pData.secondWeaponName);
        WeaponData third = Resources.Load<WeaponData>("Weapon/" + pData.thirdWeaponName);
        WeaponData fourth = Resources.Load<WeaponData>("Weapon/" + pData.fourthWeaponName);

        //인벤토리를 지정합니다.
        if(first != null)
        {
            SetInventory(first, 0);
        }
        if(second != null)
        {
            SetInventory(second, 1);
        }
        if(third != null)
        {
            SetInventory(third, 2);
        }
        if(fourth != null)
        {
            SetInventory(fourth, 3);
        }

        //인벤토리 UI를 업데이트 합니다.
        weaponWheelController.SetItemData(weaponInventory);
        UpdateWeaponInventoryUI();
    }

    void SetInventory(WeaponData weaponData, int index)
    {
        weaponInventory[index].weaponData = weaponData;
        weaponInventory[index].maxWeaponEnergy = weaponData.WeaponEnergy;
        weaponInventory[index].curWeaponEnergy = 0;
        if (weaponData.MaxAmmo == 0)
        {
            weaponInventory[index].activate = true;
        }
        else
        {
            weaponInventory[index].activate = false;
        }
        
    }

    public void ChangeWeapon(int index)
    {
        if (weaponInventory[index].weaponData != null && weaponInventory[index].activate)
        {
            //무기 변경
            playerWeapon.ChangeWeapon(index);
            //인벤토리 사용 처리
            weaponInventory[index].Use();
            //UI 전달
            UpdatePlayerGaugeUI(weaponInventory[index].weaponData);
            UpdateWeaponInventoryUI();
            //미니 무기휠에서 selected Item의 위치를 바꿉니다. 
            weaponWheelController.MoveSelectedSquarePosition(index);
        }
    }

    public void CollectWeaponEnergy(WeaponData data, int amount)
    {
        //해당 ammo가 몇 번째인지 찾는다
        int index = 0;
        for(int i = 0; i < weaponInventory.Length; i++)
        {
            if(weaponInventory[i].weaponData == data)
            {
                index = i;
                break;
            }
        }

        //에너지를 충전한다
        weaponInventory[index].CollectEnergy(amount);

        //UI를 업데이트한다
        UpdateWeaponInventoryUI();
    }

    public void UpdateWeaponInventoryUI()
    {
        //인벤토리 정보를 UI에 전달합니다. Ammo를 먹을 때, 총기를 사용할 때, 
        //무기 충전 게이지를 바꿉니다.
        weaponWheelController.UpdateWeaponGauge(weaponInventory);
    }

    #region Player관련

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

    //상호작용 관련
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

    public void OpenWeaponWheel()
    {
        weaponWheelController.ToggleWheel(true);
    }
    public void CloseWeaponWheel()
    {
        weaponWheelController.ToggleWheel(false);
    }

    //총을 다 쓰고 나서 초기화시키기
    public void ResetPlayerWeaponUI()
    {
        weaponWheelController.CancelItem();
    }

    //무기 게이지 관련 
    public void UpdateGaugeUIShootTime(float curAmmo)
    {
        gauge_Weapon.ListenShoot(curAmmo);
    }
    //ammo게이지 업데이트
    public void UpdatePlayerGaugeUI(WeaponData data)
    {
        gauge_Weapon.WeaponChange(playerWeapon, data.MaxAmmo);
    }




    //게이지가 reticle 따라다니게 만들기
    public void SetReticleFollower(GameObject reticleObj)
    {
        gauge_Weapon.setFollowTarget(reticleObj);
    }

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
        GameObject pUI_Int = Instantiate(playerUIInteractPrefab);
        weaponWheelController = pUI_Int.transform.Find("WeaponWheel").GetComponent<WeaponWheelController>();
    }

    //변화가 있을 때만 업데이트한다.
    public void UpdatePlayerStatusUI()
    {
        healthImg.fillAmount = playerHealth.currHealth / playerHealth.maxHealth;
        shieldImg.fillAmount = playerHealth.currShield / playerHealth.maxShield;
    }

    #endregion
}


//저장할 데이터

[Serializable]
public class PlayerData
{
    public string firstWeaponName;
    public string secondWeaponName;
    public string thirdWeaponName;
    public string fourthWeaponName;
}

[Serializable]

public class WeaponInventory
{
    public WeaponData weaponData;
    public int maxWeaponEnergy;
    public int curWeaponEnergy;
    public bool activate = false;
    
    public void CollectEnergy(int i)
    {
        curWeaponEnergy += i;

        if(curWeaponEnergy >= maxWeaponEnergy)
        {
            curWeaponEnergy = maxWeaponEnergy;
            activate = true;
        }
    }

    public void Use()
    {
        curWeaponEnergy = 0;
        if(weaponData.MaxAmmo != 0)
        {
            activate = false;
        }
    }
}