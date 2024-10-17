using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDictionary : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField]
    //List<WeaponData> weaponDataList = new List<WeaponData>();   //����Ʈ���� ��� weaponData���� �־�θ� �ȴ�.
    //public List<WeaponData> unlockedDataList = new List<WeaponData>();  //�رݵ� weapon����Ʈ
    List<WeaponData> tier1_WList = new List<WeaponData>();
    List<WeaponData> tier2_WList = new List<WeaponData>();
    List<WeaponData> tier3_WList = new List<WeaponData>();

    WeaponData[] wDataArray;

    Dictionary<string, ItemState> weaponNameDictionary = new Dictionary<string, ItemState>();
    ItemStates myWeaponStates;    //��� ������ �ر� ���� + weaponData

    [Header("Drones")]
    //[SerializeField] List<GameObject> dronPrefabList = new List<GameObject> ();//����Ʈ���� ��� dronePrefab���� �־�θ� �ȴ�.
    //public List<GameObject> unlockedDroneList = new List<GameObject> ();

    List<GameObject> tier1_DList = new List<GameObject>();
    List<GameObject> tier2_DList = new List<GameObject>();
    List<GameObject> tier3_DList = new List<GameObject>();
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
        tier1_WList.Clear();
        tier2_WList.Clear();
        tier3_WList.Clear();

        for (int i  = 0;  i < myWeaponStates.states.Count; i++)
        {
            if (myWeaponStates.states[i].unlocked)
            {
                WeaponData data = FindItemByName(myWeaponStates.states[i].name, wDataArray);
                int tier = myWeaponStates.states[i].tier;
                switch(tier)
                {
                    case 0:
                        tier1_WList.Add(data); break;
                    case 1:
                        tier2_WList.Add(data); break;
                    case 2:
                        tier3_WList.Add(data); break;
                }
            }
        }
        List<WeaponData>[] listArray = new List<WeaponData>[3];
        listArray[0] = tier1_WList;
        listArray[1] = tier2_WList;
        listArray[2] = tier3_WList;

        //��� Ƽ� ����Ʈ 
        tier1_DList.Clear();
        tier2_DList.Clear();
        tier3_DList.Clear();

        for (int i = 0; i < myDroneStates.states.Count; i++)
        {
            if (myDroneStates.states[i].unlocked)
            {
                GameObject data = FindDroneByName(myDroneStates.states[i].name, dDataArray);
                int tier = myDroneStates.states[i].tier;
                switch (tier)
                {
                    case 0:
                        tier1_DList.Add(data); break;
                    case 1:
                        tier2_DList.Add(data); break;
                    case 2:
                        tier3_DList.Add(data); break;
                }
            }
        }
        List<GameObject>[] listArray2 = new List<GameObject>[3];
        listArray2[0] = tier1_DList;
        listArray2[1] = tier2_DList;
        listArray2[2] = tier3_DList;

        //PopperManager�� ����
        GameManager.Instance.popperManager.ReadyWeaponPop(listArray,listArray2);

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
    public int tier;
    public string name;
    public bool unlocked;
}

[Serializable]
public class ItemStates
{
    public List<ItemState> states = new List<ItemState>();
}

