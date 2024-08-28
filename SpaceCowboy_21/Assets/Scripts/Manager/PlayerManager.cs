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
    Image healthImg;
    Image shieldImg;
    [SerializeField] Image slot_A;
    [SerializeField] Image slot_B;
    [SerializeField] Image slot_C;
    //마우스 포인트를 따라다니는 게이지 UI관련
    Gauge_Weapon gauge_Weapon;

    //플레이어 관련 스크립트
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
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
    }

    //신규 WeaponData로 변경할 때 
    
    public bool ChangeWeapon(WeaponData weaponData)
    {
        //bool canPush = playerWeapon.PushWeapon(weaponData);
        ////Ammo 게이지 UI전달
        //UpdatePlayerGaugeUI(weaponData);
        //return canPush;
        
        WeaponType wType = playerWeapon.InitializeWeapon(weaponData);
        playerWeapon.ChangeWeapon(wType, wType.maxAmmo);
        return true;    //임시로 무조건 true를 반환하게 함.
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
    
    public void StopInteractSomething()
    {
        if (curObj == null) return;
        curObj.StopInteract();
    }

    #endregion

    #region  UI관련

    //player UI 스폰
    void SpawnPlayerUI()
    {
        GameObject pUI = Instantiate(playerUIPrefab);
        healthImg = pUI.transform.Find("StatusPanel/HealthGauge").GetComponent<Image>();
        shieldImg = pUI.transform.Find("StatusPanel/ShieldGauge").GetComponent<Image>();
        gauge_Weapon = pUI.transform.Find("GaugeController").GetComponent<Gauge_Weapon>();
        slot_A = pUI.transform.Find("WeaponPanel/SlotA/WeaponImage").GetComponent<Image>();
        slot_B = pUI.transform.Find("WeaponPanel/SlotB/WeaponImage").GetComponent<Image>();
        slot_C = pUI.transform.Find("WeaponPanel/SlotC/WeaponImage").GetComponent<Image>();
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


    public void UpdateAmmoStack(Stack<AmmoInventory> stack)
    {
        List<AmmoInventory> stackList = new List<AmmoInventory>(stack);

        for(int i = 0; i < stackList.Count && i<3 ; i++)
        {
            switch (i)
            {
                case 0:
                    if (stackList[i] != null)
                        slot_A.sprite = stackList[i].weaponData.Icon;
                    else
                        slot_A.sprite = null;
                    break;
                case 1:
                    if (stackList[i] != null)
                        slot_B.sprite = stackList[i].weaponData.Icon;
                    else
                        slot_A.sprite = null;
                    break;
                case 2:
                    if (stackList[i] != null)
                        slot_C.sprite = stackList[i].weaponData.Icon;
                    else
                        slot_A.sprite = null;
                    break;
                default:
                    Debug.Log("Slot OverFlow");
                    break;
            }
        }
    }
    #endregion
}

