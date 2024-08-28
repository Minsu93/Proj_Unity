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
    Image healthImg;
    Image shieldImg;
    [SerializeField] Image slot_A;
    [SerializeField] Image slot_B;
    [SerializeField] Image slot_C;
    //���콺 ����Ʈ�� ����ٴϴ� ������ UI����
    Gauge_Weapon gauge_Weapon;

    //�÷��̾� ���� ��ũ��Ʈ
    PlayerInput playerInput;
    public PlayerBehavior playerBehavior { get; private set; }
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
    }

    //�ű� WeaponData�� ������ �� 
    
    public bool ChangeWeapon(WeaponData weaponData)
    {
        //bool canPush = playerWeapon.PushWeapon(weaponData);
        ////Ammo ������ UI����
        //UpdatePlayerGaugeUI(weaponData);
        //return canPush;
        
        WeaponType wType = playerWeapon.InitializeWeapon(weaponData);
        playerWeapon.ChangeWeapon(wType, wType.maxAmmo);
        return true;    //�ӽ÷� ������ true�� ��ȯ�ϰ� ��.
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
    
    public void StopInteractSomething()
    {
        if (curObj == null) return;
        curObj.StopInteract();
    }

    #endregion

    #region  UI����

    //player UI ����
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

