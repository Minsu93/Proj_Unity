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
    //인터렉트 오브젝트
    public InteractableOBJ curObj { get; set; }

    //플레이어 UI관련
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerUIInteractPrefab;
    Image healthImg;
    Image shieldImg;
    GameObject weaponWheel;
    WeaponWheelController weaponWheelController;

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
    public Planet playerNearestPlanet;

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
        playerThrowSlot.ChangeBomb(throwData);
        
    }

    //플레이어 스크립트 업데이트

    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerOrbSlot = playerObj.GetComponent<ArtifactPopperSlot>();
        playerThrowSlot = playerObj.GetComponent<ArtifactBombSlot>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();

        //저장 정보를 불러온다.
        LoadPlayerInfo();

        playerObj.GetComponent<PlayerBehavior>().InitPlayer();


        //Camera를 업데이트한다.
        GameManager.Instance.cameraManager.InitCam();
        //UI를 업데이트한다. 
        SpawnPlayerUI();

        
    }

    //플레이어 입력 관련 
    public void DisablePlayerInput()
    {
        if (playerInput == null) return;
        playerInput.inputDisabled = true;
    }
    public void EnablePlayerInput()
    {
        if (playerInput == null) return;
        playerInput.inputDisabled = false;
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
        GameObject pUI_Int = Instantiate(playerUIInteractPrefab);
        weaponWheel = pUI_Int.transform.Find("WeaponWheel").gameObject;
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