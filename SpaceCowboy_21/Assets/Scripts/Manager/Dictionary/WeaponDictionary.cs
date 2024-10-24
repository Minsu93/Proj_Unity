using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDictionary : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField]
    List<WeaponData> WeaponList = new List<WeaponData>();

    WeaponData[] wDataArray;

    Dictionary<string, ItemState> weaponNameDictionary = new Dictionary<string, ItemState>();
    ItemStates myWeaponStates;    //��� ������ �ر� ���� + weaponData

    [Header("Drones")]

    List<GameObject> DroneList = new List<GameObject>();

    GameObject[] dDataArray;

    Dictionary<string, ItemState> droneNameDictionary = new Dictionary<string, ItemState>();
    ItemStates myDroneStates;    //��� ������ �ر� ���� + weaponData

    private void Awake()
    {
        wDataArray = Resources.LoadAll<WeaponData>("WeaponData");
        dDataArray = Resources.LoadAll<GameObject>("DroneObj");

    }

    //�رݽ�
    public void SetItemUnlock(int index, string name, bool unlocked)
    {
        switch (index)
        {
            case 0:
                if (weaponNameDictionary.TryGetValue(name, out ItemState wState))
                {
                    wState.unlocked = unlocked;
                }
                break;
            case 1:
                if (droneNameDictionary.TryGetValue(name, out ItemState dState))
                {
                    dState.unlocked = unlocked;
                }
                break;
            
                default: break;
        }
       
        SaveWeaponDictionary();
    }

    void UpdateUnlockedDataList()
    {
        //���� Ƽ� ����Ʈ
        WeaponList.Clear();

        for (int i  = 0;  i < myWeaponStates.states.Count; i++)
        {
            if (myWeaponStates.states[i].unlocked)
            {
                WeaponData data = FindItemByName(myWeaponStates.states[i].name, wDataArray);
                WeaponList.Add(data);
            }
        }

        //��� Ƽ� ����Ʈ 
        DroneList.Clear();

        for (int i = 0; i < myDroneStates.states.Count; i++)
        {
            if (myDroneStates.states[i].unlocked)
            {
                GameObject data = FindDroneByName(myDroneStates.states[i].name, dDataArray);
                DroneList.Add(data);
            }
        }

        //PopperManager�� ����
        GameManager.Instance.popperManager.ReadyWeaponPop(WeaponList, DroneList);

    }

    #region ����&�ҷ�����

    /// <summary>
    /// ���� �ر� �� ����.
    /// </summary>
    public void SaveWeaponDictionary()
    {
        LoadManager.Save<ItemStates>(myWeaponStates, "ItemData/weaponData.json");
        LoadManager.Save<ItemStates>(myDroneStates, "ItemData/droneData.json");
        
        UpdateUnlockedDataList();
    }

    /// <summary>
    /// ���� Awake�� ���� 
    /// </summary>
    public void LoadWeaponDictionary()
    {
        myWeaponStates = LoadManager.Load<ItemStates>("ItemData/weaponData.json");
        myDroneStates =  LoadManager.Load<ItemStates>("ItemData/droneData.json");


        for (int i = 0; i < myWeaponStates.states.Count; i++)
        {
            weaponNameDictionary.Add(myWeaponStates.states[i].name, myWeaponStates.states[i]);
        }

        for (int j = 0; j < myDroneStates.states.Count; j++)
        {
            droneNameDictionary.Add(myDroneStates.states[j].name, myDroneStates.states[j]);
        }

        UpdateUnlockedDataList();

        Debug.Log("Weapon���� �ҷ����� �Ϸ�");
    }
    
    #endregion

        WeaponData FindItemByName(string name, WeaponData[] itemArray) 
        {
            for(int i = 0; i < itemArray.Length; i++)
            {
                if (name == itemArray[i].name)
                {
                    return itemArray[i];
                }
            }
            return null;
        }

        GameObject FindDroneByName(string name, GameObject[] itemArray)
        {
            for (int i = 0; i < itemArray.Length; i++)
            {
                if (name == itemArray[i].name)
                {
                    return itemArray[i];
                }
            }
            return null;
        }

}

[Serializable]
public class ItemState
{
    public string name;
    public bool unlocked;
}

[Serializable]
public class ItemStates
{
    public List<ItemState> states = new List<ItemState>();
}

