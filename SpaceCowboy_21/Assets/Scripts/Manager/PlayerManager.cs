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
    //인터렉트 오브젝트
    public InteractableOBJ curObj { get; set; }

    //플레이어 관련 스크립트
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
    PlayerHealth playerHealth;
    public PlayerBuffs playerBuffs { get; private set; }
    public PlayerWeapon playerWeapon { get; private set; }
    PlayerDrone playerDrone;

    [SerializeField] int weaponSlots = 2;
    [SerializeField] int droneSlots = 3;
    [SerializeField] Sprite emptySprite;


    //플레이어 스크립트 업데이트
    public void UpdatePlayerScripts(GameObject playerObj)
    {
        playerInput = playerObj.GetComponent<PlayerInput>();
        playerBehavior = playerObj.GetComponent<PlayerBehavior>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerWeapon = playerObj.GetComponent<PlayerWeapon>();
        playerBuffs = playerObj.GetComponent<PlayerBuffs>();
        playerDrone = playerObj.GetComponent<PlayerDrone>();

        playerWeapon.weaponSlots = this.weaponSlots;
        playerDrone.droneSlots = this.droneSlots;


        emptySprite = Resources.Load<Sprite>("UI/Empty");

        //UI를 업데이트한다. 
        SpawnPlayerUI();

        //플레이어의 기본 무기를 장착시킨다.
        playerWeapon.BackToBaseWeapon(true);

        UpdateWeaponQueue();
        UpdateDroneUI();

        playerBehavior.InitPlayer();

    }

    //신규 WeaponData로 변경할 때 
    public bool ChangeWeapon(WeaponData weaponData)
    {
        bool canPush = playerWeapon.EnqueueData(weaponData);
        UpdatePlayerGaugeUI(weaponData);
        return canPush;

    }


    #region Player Life 관련

    public int lifeMax = 3;
    public int curLife { get; set; }

    public void InitializeLife()
    {
        curLife = lifeMax;
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
    
    public void StopInteractSomething()
    {
        if (curObj == null) return;
        curObj.StopInteract();
    }

    #endregion

    #region Drone 관련
    
    public bool AddDrone(GameObject droneObj)
    {
        return playerDrone.AddDrone(droneObj);
    }

    public void UseDrone(int index)
    {
        playerDrone.UseDrone(index);
    }


    #endregion

    #region  UI관련
    //플레이어 UI관련
    [SerializeField] private GameObject playerUIPrefab;
    Image healthImg;
    Image shieldImg;
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
    //마우스 포인트를 따라다니는 게이지 UI관련
    Gauge_Weapon gauge_Weapon;
    //UISkillPanel skillPanel;
    //player UI 스폰
    void SpawnPlayerUI()
    {
        GameObject pUI = Instantiate(playerUIPrefab);
        //skillPanel = pUI.GetComponentInChildren<UISkillPanel>();

        healthImg = pUI.transform.Find("StatusPanel/HealthGauge").GetComponent<Image>();
        shieldImg = pUI.transform.Find("StatusPanel/ShieldGauge").GetComponent<Image>();
        boosterImg = pUI.transform.Find("StatusPanel/Booster/BoosterFill").GetComponent<Image>();

        gauge_Weapon = pUI.transform.Find("GaugeController").GetComponent<Gauge_Weapon>();

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

    //변화가 있을 때만 업데이트한다.
    public void UpdatePlayerStatusUI()
    {
        healthImg.fillAmount = playerHealth.currHealth / playerHealth.maxHealth;
        shieldImg.fillAmount = playerHealth.currShield / playerHealth.maxShield;
    }

    //부스터 게이지 관련
    public void UpdateBoosterUI(float fillAmount)
    {
        boosterImg.fillAmount = fillAmount;
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


    public void UpdateWeaponQueue()
    {
        WeaponData curWeaponData = playerWeapon.currWeaponType.weaponData;
        List<WeaponData> weaponDataList = new List<WeaponData>(playerWeapon.wDataQueue);

        Image[] weaponSlotImages = { WeaponSlotImage_A, WeaponSlotImage_B, WeaponSlotImage_C };
        
        //모두 empty로 만듬
        for (int i = 0; i < this.weaponSlots + 1; i++)
        {
            weaponSlotImages[i].sprite = emptySprite;
        }
        
        // baseWeapon 표시
        WeaponSlotImage_A.sprite = curWeaponData.Icon;
        
        // 슬롯에 들어있는 무기 표시
        for ( int j = 0; j < weaponDataList.Count; j++)
        {
            weaponSlotImages[j+1].sprite = weaponDataList[j].Icon;
        }
    }

    public void UpdateDroneUI()
    {
        List<DroneItem> droneList = playerDrone.drones;

        // 드론 슬롯을 배열로 관리
        Image[] droneSlotImages = { DroneSlotImage_A, DroneSlotImage_B, DroneSlotImage_C };

        // 최대슬롯까지만 업데이트
        for (int i = 0; i < this.droneSlots; i++)
        {
            droneSlotImages[i].sprite = emptySprite;
        }

        //슬롯에 들어있는 드론 표시 
        for (int j = 0; j < droneList.Count; j++)
        {
            droneSlotImages[j].sprite = droneList[j].icon;
        }
    }
    #endregion
}

