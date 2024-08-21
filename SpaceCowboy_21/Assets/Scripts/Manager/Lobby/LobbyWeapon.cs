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

    /// ���� �رݵǾ� �ִ� ������ ���� üũ�Ѵ�
    /// �رݵǰ� ���ŵ� ������ ����Ʈ
    /// �رݵǰ� ���Ŵ� �ȵ� ������ ����Ʈ
    /// ���� �κ��丮, Shop �� ��ġ (���� on)
    /// ���� �� �ش� ���� ��� ����. static�� ����ϴ°� ������? 

    //�ر��� �������� weaponShop ����Ʈ��, ������ �������� weaponInventory ����Ʈ�� ����ִ´�. 

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
                    //�ر� T ,���� T -> �κ��丮
                    weaponInventoryList.Add(lobbyWeaponStates.states[i]);
                }
                else
                {
                    //�ر� T, ���� F -> Shop
                    weaponShopList.Add(lobbyWeaponStates.states[i]);
                }
            }
        }
    }

    //����Ʈ�� ���� �κ��丮�� �����Ѵ�. 
    
    [SerializeField] List<LobbyWeaponButton> InventoryButtonList = new List<LobbyWeaponButton>();
    [SerializeField] List<LobbyWeaponButton> ShopButtonList = new List<LobbyWeaponButton>();

    void SetInventory()
    {
        //�κ��丮
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

        //����
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
    /// ������ ������ ����Ʈ�� �� ĭ�� ������ 
    /// �������� �߰��ϰ� ��ư�� ��Ȱ��ȭ �Ѵ�. 
    /// ������ ������ ����Ʈ�� �� ĭ�� ������ �۵��� ���� �ʴ´�. 
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

            //���� ��� index�� ã�Ƽ� 
            int invenIndex = weaponInventoryList.FindIndex(x => x.name.Equals(weaponState.name));
            //�ش� ��ư�� ��Ȱ��ȭ�Ѵ�. 
            InventoryButtonList[invenIndex].SetInteractableButton(false);
        }
    }


    //������ ������ �õ��� �� 
    public bool EquipItem(WeaponState state)
    {
        //�������� ���� 4�� ���̸� 
        if(equippedWeaponNames.Count < 4)
        {
            //�������� ����Ʈ�� �߰��ϰ� 
            equippedWeaponNames.Add(state.name);
            //UI�� ������Ʈ�Ѵ�
            UpdateEquippedInventory(equippedWeaponNames);
            //�����Ѵ�
            GameManager.Instance.popperManager.SaveEquippedWeapons(equippedWeaponNames);
            //���� ���� �� true
            return true;
        }

        //���� ���� �� 
        return false;

    }

    //������ ���� ������ �õ��� �� 
    public void DisarmItem(WeaponState state)
    {
        //�������� ����Ʈ�� �ִٸ�
        int index = equippedWeaponStates.FindIndex(x => x.name.Equals(state.name));
        //�������� ����Ʈ���� �����ϰ�
        equippedWeaponNames.RemoveAt(index);
        //UI�� ������Ʈ�Ѵ�
        UpdateEquippedInventory(equippedWeaponNames);
        //�����Ѵ�
        GameManager.Instance.popperManager.SaveEquippedWeapons(equippedWeaponNames);
        
        //������ ��ư�� �ٽ� Ȱ��ȭ�Ѵ�. 
        //���� ��� index�� ã�Ƽ� 
        int invenIndex = weaponInventoryList.FindIndex(x => x.name.Equals(state.name));
        //�ش� ��ư�� ��Ȱ��ȭ�Ѵ�. 
        InventoryButtonList[invenIndex].SetInteractableButton(true);
    }

    ///
    /// ������ ���� �Ϸ� �� �ؾ� �� ��
    /// 
    /// weaponStates ������Ʈ.
    /// Shop ���, Weapon��� ������Ʈ.
    /// ���� ����.
    ///
    

    //������ ���Ÿ� �õ��� �� 
    public void BuyItem(WeaponState state)
    {
        state.bought = true; //�������̶� �ڵ� ������Ʈ �ɵ�? 
        GameManager.Instance.weaponDictionary.SaveWeaponDictionary();//���� ����
        SetWeaponList();    //��� ������Ʈ
        SetInventory();     //��� ������Ʈ
        

    }

}
