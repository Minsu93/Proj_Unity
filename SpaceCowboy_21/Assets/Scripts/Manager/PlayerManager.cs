using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class PlayerManager : MonoBehaviour
{
    //���ͷ�Ʈ ������Ʈ
    public InteractableOBJ curObj { get; set; }

    //�÷��̾� ���� ��ũ��Ʈ
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    PlayerHealth playerHealth;
    //public PlayerBuffs playerBuffs { get; private set; }
    public PlayerWeapon playerWeapon { get; private set; }
    PlayerDrone playerDrone;
    HyperBoost playerHyperBoost;
    PM_LuckLevel luckLevel;
    public Planet playerNearestPlanet { get; set; } //�÷��̾�� ���� ����� �༺�� �����Ѵ�. null�� ������ �ʴ� �׽� ���� ����� �༺�� ǥ���Ѵ�. ������ ���� �뵵�� ����Ѵ�. 
    
    [SerializeField] int weaponSlots = 2;
    [SerializeField] int droneSlots = 3;
    [SerializeField] Sprite emptySprite;

    private void Awake()
    {
        luckLevel = GetComponent<PM_LuckLevel>();
    }

    //�÷��̾� ��ũ��Ʈ ������Ʈ
    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();
        //playerBuffs = playerObj.GetComponent<PlayerBuffs>();
        playerDrone = playerObj.GetComponent<PlayerDrone>();
        playerHyperBoost = playerObj.GetComponent<HyperBoost>();

        playerWeapon.weaponSlots = this.weaponSlots;
        playerDrone.droneSlots = this.droneSlots;

        emptySprite = Resources.Load<Sprite>("UI/Empty");

        //UI�� ������Ʈ�Ѵ�. 
        SpawnPlayerUI();

        //�÷��̾��� �⺻ ���⸦ ������Ų��.
        playerWeapon.BackToBaseWeapon(true);

        UpdateWeaponQueue();
        UpdateDroneUI();

        playerBehavior.InitPlayer();
        //playerObj.SetActive(false);

        luckLevel.InitializeLevel();
    }

    //�ű� WeaponData�� ������ �� 
    public bool ChangeWeapon(WeaponData weaponData)
    {
        bool canPush = playerWeapon.EnqueueData(weaponData);
        return canPush;

    }

    public void ChangeBaseWeapon(WeaponData newData)
    {
        playerWeapon.ChangeBaseWeapon(newData);

    }

    #region HyperBoost �������� �̵�, é�� �̵�
    public delegate void ChangeSceneDel();
    public void SuperBoost(Vector2 targetPosition, Transform flagTr, bool boostIn, ChangeSceneDel changeSceneDel)
    {
        if (boostIn)
        {
            StartCoroutine(playerHyperBoost.SuperBoostIn(targetPosition, flagTr, changeSceneDel));
        }
        else
        {
            StartCoroutine(playerHyperBoost.SuperBoostOut(flagTr, targetPosition, changeSceneDel));
        }
    }
    public void ReadyHyperBoost()
    {
        //�� �����µ��� �ִϸ��̼� ��� 
    }
    public void LaunchhHyperBoost()
    {
        //�ν��͸� Ÿ�� ���ư�������. é�� �̵�.
    }

    

    #endregion

    #region Player Life ����

    public int bonusLife = 3;
    public int curLife { get; set; }

    public void InitializeLife()
    {
        curLife = bonusLife;
        Debug.Log("curLife is : " + curLife);
    }

    public bool CanRespawn()
    {
        if (--curLife < 0)
        {
            Debug.Log(curLife + " life left");
            return false;
        }
        else
        {
            Debug.Log(curLife + " life left");
            return true;
        }
    }

    public bool HealthUp(float amount)
    {
        return playerBehavior.healEvent(amount);
    }

    #endregion

    #region Player ����

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

    public void DeactivatePlayerBehavior(bool isActivate)
    {
        playerBehavior.DeactivatePlayer(isActivate);
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
    
    public void StopInteractSomething()
    {
        if (curObj == null) return;
        curObj.StopInteract();
    }

    #endregion

    #region Drone ����
    
    public bool AddDrone(GameObject droneObj)
    {
        return playerDrone.AddDrone(droneObj);
    }

    public void UseDrone(int index)
    {
        playerDrone.UseDrone(index);
    }

    public bool RemoveDrone()
    {
        return playerDrone.RemoveDrone();
    }

    public void DeactivateDrone()
    {
        playerDrone.DeactivateDrones();
    }
    public void MoveAndActivateDrone(Vector2 pos)
    {
        playerDrone.MoveAndActivateDrones(pos);
    }

    public bool IsDroneDropPossible()
    {
        return droneSlots > GetDroneCount();
    }
    int GetDroneCount()
    {
        return playerDrone.drones.Count;
    }
    #endregion

    #region  UI����
    //�÷��̾� UI����
    [SerializeField] private GameObject playerUIPrefab;
    Image healthImg;
    //Image shieldImg;
    Image boosterImg;
    Image WeaponSlotImage_A;
    Image WeaponSlotImage_B;
    Image WeaponSlotImage_C;
    Image DroneSlotImage_A;
    Image DroneSlotImage_B;
    Image DroneSlotImage_C;
    GameObject WeaponSlotObj_A;
    GameObject WeaponSlotObj_B;
    GameObject WeaponSlotObj_C;
    GameObject DroneSlotObj_A;
    GameObject DroneSlotObj_B;
    GameObject DroneSlotObj_C;
    //���콺 ����Ʈ�� ����ٴϴ� ������ UI����
    Gauge_Weapon gauge_Weapon;
    GameObject boosterObj;
    UI_LevelPanel levelPanel;
    //player UI ����

    void SpawnPlayerUI()
    {
        GameObject pUI = Instantiate(playerUIPrefab);
        //skillPanel = pUI.GetComponentInChildren<UISkillPanel>();

        healthImg = pUI.transform.Find("DronePanel/HealthGauge").GetComponent<Image>();
        //shieldImg = pUI.transform.Find("StatusPanel/ShieldGauge").GetComponent<Image>();

        boosterObj = pUI.transform.Find("Booster").gameObject;
        boosterImg = pUI.transform.Find("Booster/BoosterFill").GetComponent<Image>();
        boosterObj.GetComponentInChildren<FollowOBJ>().followingObj = GameManager.Instance.player.gameObject;
        HideBoosterUI();

        levelPanel = pUI.GetComponentInChildren<UI_LevelPanel>();
        gauge_Weapon = pUI.transform.Find("GaugeController").GetComponent<Gauge_Weapon>();
        SetReticleFollower();

        WeaponSlotImage_A = pUI.transform.Find("WeaponPanel/SlotA/Back/WeaponImage").GetComponent<Image>();
        WeaponSlotImage_B = pUI.transform.Find("WeaponPanel/SlotB/Back/WeaponImage").GetComponent<Image>();
        WeaponSlotImage_C = pUI.transform.Find("WeaponPanel/SlotC/Back/WeaponImage").GetComponent<Image>();

        WeaponSlotObj_A = pUI.transform.Find("WeaponPanel/SlotA").gameObject;
        WeaponSlotObj_B = pUI.transform.Find("WeaponPanel/SlotB").gameObject;
        WeaponSlotObj_C = pUI.transform.Find("WeaponPanel/SlotC").gameObject;

        DroneSlotImage_A = pUI.transform.Find("DronePanel/SlotA/Back/Image").GetComponent<Image>();
        DroneSlotImage_B = pUI.transform.Find("DronePanel/SlotB/Back/Image").GetComponent<Image>();
        DroneSlotImage_C = pUI.transform.Find("DronePanel/SlotC/Back/Image").GetComponent<Image>();

        DroneSlotObj_A = pUI.transform.Find("DronePanel/SlotA").gameObject;
        DroneSlotObj_B = pUI.transform.Find("DronePanel/SlotB").gameObject;
        DroneSlotObj_C = pUI.transform.Find("DronePanel/SlotC").gameObject;

        UpdateSlots();
    }

    void UpdateSlots()
    {
        GameObject[] weaponSlotObjs = { WeaponSlotObj_A, WeaponSlotObj_B, WeaponSlotObj_C };
        GameObject[] droneSlotObjs = { DroneSlotObj_A, DroneSlotObj_B, DroneSlotObj_C };

        for (int i  = 3; i > weaponSlots + 1 && i > 0; i--)
        {
            weaponSlotObjs[i-1].SetActive(false);
        }
        for(int j = 3; j > droneSlots && j> 0; j--)
        {
            droneSlotObjs[j-1].SetActive(false);
        }
    }

    //��ȭ�� ���� ���� ������Ʈ�Ѵ�.
    public void UpdatePlayerStatusUI()
    {
        healthImg.fillAmount = playerHealth.currHealth / playerHealth.maxHealth;
        //shieldImg.fillAmount = playerHealth.currShield / playerHealth.maxShield;
    }

    //�ν��� ������ ����
    public void UpdateBoosterUI(float fillAmount)
    {
        if(!boosterObj.activeSelf)
        {
            boosterObj.SetActive(true);
        }
        boosterImg.fillAmount = fillAmount;
    }

    public void HideBoosterUI()
    {
        if (boosterObj.activeSelf)
        {
            boosterObj.SetActive(false);
        }
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
    public void SetReticleFollower()
    {
        gauge_Weapon.setFollowTarget(GameManager.Instance.cameraManager.reticle);
    }


    public void UpdateWeaponQueue()
    {
        WeaponData curWeaponData = playerWeapon.currWeaponType.weaponData;
        List<WeaponData> weaponDataList = new List<WeaponData>(playerWeapon.wDataQueue);

        Image[] weaponSlotImages = { WeaponSlotImage_A, WeaponSlotImage_B, WeaponSlotImage_C };
        
        //��� empty�� ����
        for (int i = 0; i < this.weaponSlots + 1; i++)
        {
            weaponSlotImages[i].sprite = emptySprite;
        }
        
        // baseWeapon ǥ��
        WeaponSlotImage_A.sprite = curWeaponData.Icon;
        
        // ���Կ� ����ִ� ���� ǥ��
        for ( int j = 0; j < weaponDataList.Count; j++)
        {
            weaponSlotImages[j+1].sprite = weaponDataList[j].Icon;
        }
    }

    public void UpdateDroneUI()
    {
        List<DroneItem> droneList = playerDrone.drones;

        // ��� ������ �迭�� ����
        Image[] droneSlotImages = { DroneSlotImage_A, DroneSlotImage_B, DroneSlotImage_C };

        // �ִ뽽�Ա����� ������Ʈ
        for (int i = 0; i < this.droneSlots; i++)
        {
            droneSlotImages[i].sprite = emptySprite;
        }

        //���Կ� ����ִ� ��� ǥ�� 
        for (int j = 0; j < droneList.Count; j++)
        {
            droneSlotImages[j].sprite = droneList[j].icon;
        }
    }

    //public void UpdateEscapeUI(float amount)
    //{
    //    escapeUI.UpdateGauge(amount);
    //}
    public void GainExp(int index, float amount)
    {
        luckLevel.GainExp(index, amount);
    }

    public void LossExp()
    {
        luckLevel.LossExp();
    }

    public void UpdateLuckyLevel(int index, int level, float curExp, float maxExp, bool isGain)
    {
        levelPanel.SetLevelPanel(index, level, curExp, maxExp, isGain);
    }
    #endregion
}

