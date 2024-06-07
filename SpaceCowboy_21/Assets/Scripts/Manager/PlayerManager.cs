using SpaceCowboy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //Update 활성화 여부 
    bool activate = false;
    float timer;

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
    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject orbData;
    [SerializeField] GameObject throwData;
    


    //플레이어 관련 스크립트
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    ArtifactPopperSlot playerOrbSlot;
    ArtifactBombSlot playerThrowSlot;
    PlayerHealth playerHealth;
    public PlayerWeapon playerWeapon { get; private set; }



    //플레이어가 사용하는 장비, 유물, 업그레이드 수치 등을 저장한다.
    public void SavePlayerInfo()
    {
        string path = Path.Combine(Application.dataPath , "Data/PlayerData", "playerData.json");
        
        //Test
        
        PlayerData pData = new PlayerData();
        pData.weaponName = weaponData.name;
        pData.orbName = orbData.name;
        pData.throwSatelliteName = throwData.name;

        string str = JsonUtility.ToJson(pData, true);
        File.WriteAllText(path, str);
    }

    public void LoadPlayerInfo()
    {
        string path = Path.Combine(Application.dataPath , "Data/PlayerData", "playerData.json");
        string loadJson = File.ReadAllText(path);
        PlayerData pData = JsonUtility.FromJson<PlayerData>(loadJson);

        weaponData = Resources.Load<WeaponData>("Weapon/" + pData.weaponName);
        orbData = Resources.Load<GameObject>("Orb/" + pData.orbName);
        throwData = Resources.Load<GameObject>("Orb/" + pData.throwSatelliteName);

        playerBehavior.TryChangeWeapon(weaponData);
        playerOrbSlot.ChangePopper(orbData);
        //playerThrowSlot.ChangeBomb(throwData);
        
    }

    //플레이어 스크립트 업데이트

    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerOrbSlot = playerObj.GetComponent<ArtifactPopperSlot>();
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

        //플레이어의 NearestPlanet을 갱신한다
        activate = true;

    }

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


    public void OpenWeaponWheel()
    {
        weaponWheelController.ToggleWheel(true);
    }
    public void CloseWeaponWheel()
    {
        weaponWheelController.ToggleWheel(false);
    }

    
    //무기 게이지 관련 
    public void PlayerShootTime(float lastShootTime)
    {
        gauge_Weapon.ListenShoot(lastShootTime);
    }
    public void PlayerChangeWeapon(WeaponData data)
    {
        gauge_Weapon.WeaponChange(playerWeapon, data.ShootInterval);
    }

    //게이지가 reticle 따라다니게 만들기
    public void SetReticleFollower(GameObject reticleObj)
    {
        gauge_Weapon.setFollowTarget(reticleObj);
    }

    //총을 다 쓰고 나서 초기화시키기
    public void ResetPlayerWeapon()
    {
        weaponWheelController.CancelItem();
    }

    //Orb 충전
    public void ChargeFireworkEnergy()
    {
        playerOrbSlot.EnergyIncrease(1f);
    }

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


}



[Serializable]
public class PlayerData
{
    public string weaponName;
    public string orbName;
    public string throwSatelliteName;
}