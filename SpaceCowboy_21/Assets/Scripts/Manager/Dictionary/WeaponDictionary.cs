using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDictionary : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField]
    List<WeaponData> weaponDataList = new List<WeaponData>();   //리스트에는 모든 weaponData들을 넣어두면 된다.
    public List<WeaponData> unlockedDataList = new List<WeaponData>();  //해금된 weapon리스트

    Dictionary<string, UnlockedState> weaponNameDictionary = new Dictionary<string, UnlockedState>();
    UnlockedStates myWeaponStates;    //모든 아이템 해금 정보 + weaponData

    [Header("Drones")]
    [SerializeField] List<GameObject> dronPrefabList = new List<GameObject> ();//리스트에는 모든 dronePrefab들을 넣어두면 된다.
    public List<GameObject> unlockedDroneList = new List<GameObject> ();

    Dictionary<string, UnlockedState> droneNameDictionary = new Dictionary<string, UnlockedState>();
    UnlockedStates myDroneStates;    //모든 아이템 해금 정보 + weaponData

    public void SetWeaponState(string name,bool unlocked)
    {
        weaponNameDictionary.TryGetValue(name, out UnlockedState state);
        state.unlocked = unlocked;

        SaveWeaponDictionary();
    }

    //public UnlockedState GetWeaponState(string name)
    //{
    //    if(!weaponNameDictionary.TryGetValue(name, out UnlockedState state))
    //    {
    //        Debug.Log("해당 weapon이이 Dictionary에 없습니다");
    //    }
    //    return state;
    //}
    //public T GetItemData<T>(string name)
    //{
    //    if(typeof(T) == typeof(WeaponData))
    //    {
    //        return (T)(object)unlockedDataList.Find(item => item.name.Equals(name));
    //    }
    //    else 
    //    {
    //        return (T)(object)unlockedDroneList.Find(item => item.name.Equals(name));
    //    }

    //}

    void UpdateUnlockedDataList()
    {
        for(int i  = 0;  i < myWeaponStates.states.Count; i++)
        {
            if (myWeaponStates.states[i].unlocked)
            {
                WeaponData data = weaponDataList.Find(item => item.name.Equals(myWeaponStates.states[i].name));
                if (data != null)
                {
                    unlockedDataList.Add(data);
                }
            }
        }
        for (int j = 0; j < myDroneStates.states.Count; j++)
        {
            if (myDroneStates.states[j].unlocked)
            {
                GameObject prefab = dronPrefabList.Find(item => item.name.Equals(myDroneStates.states[j].name)); ;
                if (prefab != null)
                {
                    unlockedDroneList.Add(prefab);
                }
            }
        }
    }

    #region 저장&불러오기

    /// <summary>
    /// 무기 해금, 무기 구매 시 실행.
    /// </summary>
    public void SaveWeaponDictionary()
    {
        LoadManager.Save<UnlockedStates>(myWeaponStates, "ItemData/weaponData.json");
        LoadManager.Save<UnlockedStates>(myDroneStates, "ItemData/droneData.json");
        
        UpdateUnlockedDataList();
    }

    /// <summary>
    /// Lobby 상점에 진입시 실행. 
    /// </summary>
    public void LoadWeaponDictionary()
    {
        myWeaponStates = LoadManager.Load<UnlockedStates>("ItemData/weaponData.json");
        myDroneStates =  LoadManager.Load<UnlockedStates>("ItemData/droneData.json");


        for (int i = 0; i < myWeaponStates.states.Count; i++)
        {
            weaponNameDictionary.Add(myWeaponStates.states[i].name, myWeaponStates.states[i]);
        }

        for(int j = 0; j < myDroneStates.states.Count; j++)
        {
            droneNameDictionary.Add(myDroneStates.states[j].name, myDroneStates.states[j]);
        }

        UpdateUnlockedDataList();

        Debug.Log("Weapon정보 불러오기 완료");
    }
    
    #endregion


    //#region EquippedWeapon 장착된 무기 저장 & 불러오기

    //public List<string> equippedNamesList { get; private set; }
    //public void SaveEquippedWeapons(List<string> names)
    //{
    //    equippedNamesList = names;

    //    EquippedWeaponData data = new EquippedWeaponData();
    //    data.equippedWeaponNames = equippedNamesList;

    //    string path = Path.Combine(Application.dataPath + "/Data/PlayerData/equippedWeapon.json");
    //    string str = JsonUtility.ToJson(data, true);
    //    File.WriteAllText(path, str);
    //}

    //public void LoadEquippedWeapons()
    //{

    //    string path = Path.Combine(Application.dataPath + "/Data/PlayerData/equippedWeapon.json");
    //    string loadJson = File.ReadAllText(path);
    //    EquippedWeaponData data = JsonUtility.FromJson<EquippedWeaponData>(loadJson);

    //    equippedNamesList = data.equippedWeaponNames;



    //}


    //#endregion

}

[Serializable]
public class UnlockedState
{
    public string name;
    public bool unlocked;
}

[Serializable]
public class UnlockedStates
{
    public List<UnlockedState> states = new List<UnlockedState>();
}

[Serializable]
public class EquippedWeaponData
{
    public List<string> equippedWeaponNames = new List<string>();
}