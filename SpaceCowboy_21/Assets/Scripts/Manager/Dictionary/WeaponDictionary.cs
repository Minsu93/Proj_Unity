using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDictionary : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField]
    //List<WeaponData> weaponDataList = new List<WeaponData>();   //리스트에는 모든 weaponData들을 넣어두면 된다.
    //public List<WeaponData> unlockedDataList = new List<WeaponData>();  //해금된 weapon리스트
    List<WeaponData> tier1_WList = new List<WeaponData>();
    List<WeaponData> tier2_WList = new List<WeaponData>();
    List<WeaponData> tier3_WList = new List<WeaponData>();

    WeaponData[] wDataArray;

    Dictionary<string, ItemState> weaponNameDictionary = new Dictionary<string, ItemState>();
    ItemStates myWeaponStates;    //모든 아이템 해금 정보 + weaponData

    [Header("Drones")]
    //[SerializeField] List<GameObject> dronPrefabList = new List<GameObject> ();//리스트에는 모든 dronePrefab들을 넣어두면 된다.
    //public List<GameObject> unlockedDroneList = new List<GameObject> ();

    List<GameObject> tier1_DList = new List<GameObject>();
    List<GameObject> tier2_DList = new List<GameObject>();
    List<GameObject> tier3_DList = new List<GameObject>();
    GameObject[] dDataArray;

    Dictionary<string, ItemState> droneNameDictionary = new Dictionary<string, ItemState>();
    ItemStates myDroneStates;    //모든 아이템 해금 정보 + weaponData

    private void Awake()
    {
        wDataArray = Resources.LoadAll<WeaponData>("WeaponData");
        dDataArray = Resources.LoadAll<GameObject>("DroneObj");

    }

    //해금시
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
        //무기 티어별 리스트
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

        //드론 티어별 리스트 
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

        //PopperManager에 적용
        GameManager.Instance.popperManager.ReadyWeaponPop(listArray,listArray2);

    }

    #region 저장&불러오기

    /// <summary>
    /// 무기 해금 시 실행.
    /// </summary>
    public void SaveWeaponDictionary()
    {
        LoadManager.Save<ItemStates>(myWeaponStates, "ItemData/weaponData.json");
        LoadManager.Save<ItemStates>(myDroneStates, "ItemData/droneData.json");
        
        UpdateUnlockedDataList();
    }

    /// <summary>
    /// 게임 Awake시 실행 
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

        Debug.Log("Weapon정보 불러오기 완료");
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

