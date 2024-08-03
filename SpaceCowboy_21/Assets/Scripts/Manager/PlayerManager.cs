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
    //���ͷ�Ʈ ������Ʈ
    public InteractableOBJ curObj { get; set; }

    //�÷��̾� UI����
    [SerializeField] private GameObject playerUIPrefab;
    //[SerializeField] private GameObject playerUIInteractPrefab;
    Image healthImg;
    Image shieldImg;
    //WeaponWheelController weaponWheelController;
    //���콺 ����Ʈ�� ����ٴϴ� ������ UI����
    Gauge_Weapon gauge_Weapon;

    //������ �÷��̾� ����
    //[SerializeField] WeaponData firstWeaponData;
    //[SerializeField] WeaponData secondWeaponData;
    //[SerializeField] WeaponData thirdWeaponData;
    //[SerializeField] WeaponData fourthWeaponData;

    //���� �κ��丮
    //public WeaponInventory[] weaponInventory = new WeaponInventory[4];
    //WeaponData[] weaponInventory = new WeaponData[4];
    //public WeaponData baseWeaponData;

    //�÷��̾� ���� ��ũ��Ʈ
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    //ArtifactPopperSlot playerOrbSlot;
    //ArtifactBombSlot playerThrowSlot;
    PlayerHealth playerHealth;
    public PlayerBuffs playerBuffs { get; private set; }
    public PlayerWeapon playerWeapon { get; private set; }



    //�÷��̾� ��ũ��Ʈ ������Ʈ

    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();
        playerBuffs = playerObj.GetComponent<PlayerBuffs>();

        playerObj.GetComponent<PlayerBehavior>().InitPlayer(this);

        //UI�� ������Ʈ�Ѵ�. 
        SpawnPlayerUI();

        //Camera�� ������Ʈ�Ѵ�.
        GameManager.Instance.cameraManager.InitCam();

        //���� ������ �ҷ��´�.
        //LoadPlayerInfo();

        //�÷��̾��� �⺻ ���⸦ ������Ų��.
        //ChangeWeapon(0);
        playerWeapon.BackToBaseWeapon();
    }

    //�÷��̾ ����ϴ� ���, ����, ���׷��̵� ��ġ ���� �����Ѵ�.
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

    //    //�κ��丮�� �����մϴ�.
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

    //BaseWeapon ��� ������ �� 
    //public void ChangeWeapon(int index)
    //{
    //    WeaponData data = weaponInventory[index];
    //    //���� ����
    //    playerWeapon.InitializeWeapon(data);

    //    //UI ����
    //    UpdatePlayerGaugeUI(data);
    //}

    //�ű� WeaponData�� ������ �� 
    public bool ChangeWeapon(WeaponData weaponData)
    {
        bool canPush = playerWeapon.PushWeapon(weaponData);
        //Ammo ������ UI����
        UpdatePlayerGaugeUI(weaponData);
        return canPush;
    }

    public bool HealthUp(float amount)
    {
        return playerBehavior.healEvent(amount);
    }
    #region Player Input ����

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
    #endregion

    #region Interaction

    //��ȣ�ۿ� ����

    public void SetInteractableObj(InteractableOBJ iObj)
    {
        //��ȣ�ۿ��ϴ� ������ �ִٸ� ��ȣ�ۿ� ���
        if(curObj != null)
        {
            curObj.StopInteract();
        }
        //���� �Ҵ�
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

        //�÷��̾� Cancel�� �����ϵ��� ���� ����.
        //DisablePlayerInput();
        curObj.InteractAction();
    }

    #endregion

    #region  UI����

    //public void OpenWeaponWheel()
    //{
    //    weaponWheelController.ToggleWheel(true);
    //}
    //public void CloseWeaponWheel()
    //{
    //    weaponWheelController.ToggleWheel(false);
    //}

    //���� �� ���� ���� �ʱ�ȭ��Ű��
    //public void ResetPlayerWeaponUI()
    //{
    //    weaponWheelController.CancelItem();
    //}


    //Orb ����
    //public void ChargeFireworkEnergy()
    //{
    //    playerOrbSlot.EnergyIncrease(1f);
    //}


    //player UI ����
    void SpawnPlayerUI()
    {
        GameObject pUI = Instantiate(playerUIPrefab);
        healthImg = pUI.transform.Find("StatusPanel/HealthGauge").GetComponent<Image>();
        shieldImg = pUI.transform.Find("StatusPanel/ShieldGauge").GetComponent<Image>();
        gauge_Weapon = pUI.transform.Find("GaugeController").GetComponent<Gauge_Weapon>();
        //GameObject pUI_Int = Instantiate(playerUIInteractPrefab);
        //weaponWheelController = pUI_Int.transform.Find("WeaponWheel").GetComponent<WeaponWheelController>();
    }

    //��ȭ�� ���� ���� ������Ʈ�Ѵ�.
    public void UpdatePlayerStatusUI()
    {
        healthImg.fillAmount = playerHealth.currHealth / playerHealth.maxHealth;
        shieldImg.fillAmount = playerHealth.currShield / playerHealth.maxShield;
    }


    //���� ������ ���� 
    public void UpdateGaugeUIShootTime(float curAmmo)
    {
        gauge_Weapon.ListenShoot(curAmmo);
    }

    //���Ⱑ �ٲ���� �� ������ ����
    public void UpdatePlayerGaugeUI(WeaponData data)
    {
        gauge_Weapon.WeaponChange(playerWeapon, data.MaxAmmo);
    }

    //�������� reticle ����ٴϰ� �����
    public void SetReticleFollower(GameObject reticleObj)
    {
        gauge_Weapon.setFollowTarget(reticleObj);
    }

    #endregion
}


//������ ������

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