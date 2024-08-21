using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyWeapon : MonoBehaviour
{
    WeaponStates lobbyWeaponStates;
    void Start()
    {
        GameManager.Instance.weaponDictionary.LoadWeaponDictionary();
        lobbyWeaponStates = GameManager.Instance.weaponDictionary.myWeaponStates;
        equippedWeaponNames = GameManager.Instance.popperManager.LoadEquippedWeapons();

        SetWeaponList();
        SetInventory();

        UpdateEquippedInventory(equippedWeaponNames);

    }

    /// 현재 해금되어 있는 무기의 수를 체크한다
    /// 해금되고 구매된 아이템 리스트
    /// 해금되고 구매는 안된 아이템 리스트
    /// 각각 인벤토리, Shop 에 배치 (슬롯 on)
    /// 구매 시 해당 정보 즉시 저장. static을 사용하는게 나을듯? 

    //해금한 아이템은 weaponShop 리스트에, 구매한 아이템은 weaponInventory 리스트에 집어넣는다. 

    List<WeaponState> weaponInventoryList = new List<WeaponState>();
    List<WeaponState> weaponShopList = new List<WeaponState>();

    void SetWeaponList()
    {
        weaponInventoryList.Clear();
        weaponShopList.Clear();

        for( int i = 0; i < lobbyWeaponStates.states.Count; i++ )
        {
            if (lobbyWeaponStates.states[i].unlocked)
            {
                if (lobbyWeaponStates.states[i].bought)
                {
                    //해금 T ,구매 T -> 인벤토리
                    weaponInventoryList.Add(lobbyWeaponStates.states[i]);
                }
                else
                {
                    //해금 T, 구매 F -> Shop
                    weaponShopList.Add(lobbyWeaponStates.states[i]);
                }
            }
        }
    }

    //리스트에 따라서 인벤토리를 관리한다. 
    
    [SerializeField] List<LobbyWeaponButton> InventoryButtonList = new List<LobbyWeaponButton>();
    [SerializeField] List<LobbyWeaponButton> ShopButtonList = new List<LobbyWeaponButton>();

    void SetInventory()
    {
        //인벤토리
        if (weaponInventoryList.Count < InventoryButtonList.Count)
        {
            for (int i = 0; i < InventoryButtonList.Count; i++)
            {
                if( i < weaponInventoryList.Count)
                {
                    InventoryButtonList[i].gameObject.SetActive(true);
                    InventoryButtonList[i].SetEquipButton(weaponInventoryList[i], this);
                }
                else
                {
                    InventoryButtonList[i].gameObject.SetActive(false);
                } 
            }
        }

        //상점
        if (weaponShopList.Count < ShopButtonList.Count)
        {
            for (int i = 0; i < ShopButtonList.Count; i++)
            {
                if (i < weaponShopList.Count)
                {
                    ShopButtonList[i].gameObject.SetActive(true);
                    ShopButtonList[i].SetShopButton(weaponShopList[i], this);
                }
                else
                {
                    ShopButtonList[i].gameObject.SetActive(false);
                }
            }
        }
    }


    /// 
    /// 장착된 아이템 리스트에 빈 칸이 있으면 
    /// 아이템을 추가하고 버튼을 비활성화 한다. 
    /// 장착된 아이템 리스트에 빈 칸이 없으면 작동을 하지 않는다. 
    /// 
    List<string> equippedWeaponNames = new List<string>();
    List<WeaponState> equippedWeaponStates = new List<WeaponState>();
    [SerializeField] List<LobbyWeaponButton> equipmentButton = new List<LobbyWeaponButton>();

    void UpdateEquippedInventory(List<string> names)
    {
        equippedWeaponStates.Clear();

        foreach(var button in equipmentButton)
        {
            button.SetInteractableButton(false);
        }
        for (int i = 0; i < names.Count; i++)
        {
            WeaponState weaponState = GameManager.Instance.weaponDictionary.GetWeaponState(names[i]);
            equippedWeaponStates.Add(weaponState);
            equipmentButton[i].SetDisarmButton(weaponState, this);
            equipmentButton[i].SetInteractableButton(true);

            //무기 목록 index를 찾아서 
            int invenIndex = weaponInventoryList.FindIndex(x => x.name.Equals(weaponState.name));
            //해당 버튼을 비활성화한다. 
            InventoryButtonList[invenIndex].SetInteractableButton(false);
        }
    }


    //아이템 장착을 시도할 때 
    public bool EquipItem(WeaponState state)
    {
        //아이템의 수가 4개 밑이면 
        if(equippedWeaponNames.Count < 4)
        {
            //아이템을 리스트에 추가하고 
            equippedWeaponNames.Add(state.name);
            //UI를 업데이트한다
            UpdateEquippedInventory(equippedWeaponNames);
            //저장한다
            GameManager.Instance.popperManager.SaveEquippedWeapons(equippedWeaponNames);
            //장착 성공 시 true
            return true;
        }

        //장착 실패 시 
        return false;

    }

    //아이템 장착 해제를 시도할 때 
    public void DisarmItem(WeaponState state)
    {
        //아이템이 리스트에 있다면
        int index = equippedWeaponStates.FindIndex(x => x.name.Equals(state.name));
        //아이템을 리스트에서 제거하고
        equippedWeaponNames.RemoveAt(index);
        //UI를 업데이트한다
        UpdateEquippedInventory(equippedWeaponNames);
        //저장한다
        GameManager.Instance.popperManager.SaveEquippedWeapons(equippedWeaponNames);
        
        //해제한 버튼을 다시 활성화한다. 
        //무기 목록 index를 찾아서 
        int invenIndex = weaponInventoryList.FindIndex(x => x.name.Equals(state.name));
        //해당 버튼을 재활성화한다. 
        InventoryButtonList[invenIndex].SetInteractableButton(true);
    }

    ///
    /// 아이템 구매 완료 시 해야 할 일
    /// 
    /// weaponStates 업데이트.
    /// Shop 목록, Weapon목록 업데이트.
    /// 정보 저장.
    ///
    

    //아이템 구매를 시도할 때 
    public void BuyItem(WeaponState state)
    {
        state.bought = true; //참조형이라서 자동 업데이트 될듯? 
        GameManager.Instance.weaponDictionary.SaveWeaponDictionary();//정보 저장
        SetWeaponList();    //목록 업데이트
        SetInventory();     //목록 업데이트
        

    }

}
