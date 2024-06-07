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
    //Update Ȱ��ȭ ���� 
    bool activate = false;
    float timer;

    //���ͷ�Ʈ ������Ʈ
    public InteractableOBJ curObj { get; set; }

    //�÷��̾� UI����
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject playerUIInteractPrefab;
    Image healthImg;
    Image shieldImg;
    WeaponWheelController weaponWheelController;
    Gauge_Weapon gauge_Weapon;

    //������ �÷��̾� ����
    [SerializeField] WeaponData weaponData;
    [SerializeField] GameObject orbData;
    [SerializeField] GameObject throwData;
    


    //�÷��̾� ���� ��ũ��Ʈ
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    ArtifactPopperSlot playerOrbSlot;
    ArtifactBombSlot playerThrowSlot;
    PlayerHealth playerHealth;
    public PlayerWeapon playerWeapon { get; private set; }



    //�÷��̾ ����ϴ� ���, ����, ���׷��̵� ��ġ ���� �����Ѵ�.
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

    //�÷��̾� ��ũ��Ʈ ������Ʈ

    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerOrbSlot = playerObj.GetComponent<ArtifactPopperSlot>();
        //playerThrowSlot = playerObj.GetComponent<ArtifactBombSlot>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();

        playerObj.GetComponent<PlayerBehavior>().InitPlayer(this);

        //UI�� ������Ʈ�Ѵ�. 
        SpawnPlayerUI();

        //Camera�� ������Ʈ�Ѵ�.
        GameManager.Instance.cameraManager.InitCam();

        //���� ������ �ҷ��´�.
        LoadPlayerInfo();

        //�÷��̾��� NearestPlanet�� �����Ѵ�
        activate = true;

    }

    //�÷��̾� �Է� ���� 
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

    //��ȣ�ۿ� ����
    public void InteractSomething()
    {
        if (curObj == null)
            return;

        //�÷��̾� Cancel�� �����ϵ��� ���� ����.
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

    
    //���� ������ ���� 
    public void PlayerShootTime(float lastShootTime)
    {
        gauge_Weapon.ListenShoot(lastShootTime);
    }
    public void PlayerChangeWeapon(WeaponData data)
    {
        gauge_Weapon.WeaponChange(playerWeapon, data.ShootInterval);
    }

    //�������� reticle ����ٴϰ� �����
    public void SetReticleFollower(GameObject reticleObj)
    {
        gauge_Weapon.setFollowTarget(reticleObj);
    }

    //���� �� ���� ���� �ʱ�ȭ��Ű��
    public void ResetPlayerWeapon()
    {
        weaponWheelController.CancelItem();
    }

    //Orb ����
    public void ChargeFireworkEnergy()
    {
        playerOrbSlot.EnergyIncrease(1f);
    }

    //player UI ����
    void SpawnPlayerUI()
    {
        GameObject pUI = Instantiate(playerUIPrefab);
        healthImg = pUI.transform.Find("StatusPanel/HealthGauge").GetComponent<Image>();
        shieldImg = pUI.transform.Find("StatusPanel/ShieldGauge").GetComponent<Image>();
        gauge_Weapon = pUI.transform.Find("GaugeController").GetComponent<Gauge_Weapon>();
        GameObject pUI_Int = Instantiate(playerUIInteractPrefab);
        weaponWheelController = pUI_Int.transform.Find("WeaponWheel").GetComponent<WeaponWheelController>();
    }

    //��ȭ�� ���� ���� ������Ʈ�Ѵ�.
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