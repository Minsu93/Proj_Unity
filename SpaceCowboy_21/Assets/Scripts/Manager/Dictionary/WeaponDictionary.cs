using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDictionary : MonoBehaviour
{
    [Header("Guns")]
    [SerializeField]
    List<WeaponData> weaponDataList = new List<WeaponData>();   //����Ʈ���� ��� weaponData���� �־�θ� �ȴ�.
    public List<WeaponData> unlockedDataList = new List<WeaponData>();  //�رݵ� weapon����Ʈ

    Dictionary<string, UnlockedState> weaponNameDictionary = new Dictionary<string, UnlockedState>();
    UnlockedStates myWeaponStates;    //��� ������ �ر� ���� + weaponData

    [Header("Drones")]
    [SerializeField] List<GameObject> dronPrefabList = new List<GameObject> ();//����Ʈ���� ��� dronePrefab���� �־�θ� �ȴ�.
    public List<GameObject> unlockedDroneList = new List<GameObject> ();

    Dictionary<string, UnlockedState> droneNameDictionary = new Dictionary<string, UnlockedState>();
    UnlockedStates myDroneStates;    //��� ������ �ر� ���� + weaponData

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
    //        Debug.Log("�ش� weapon���� Dictionary�� �����ϴ�");
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

    #region ����&�ҷ�����

    /// <summary>
    /// ���� �ر�, ���� ���� �� ����.
    /// </summary>
    public void SaveWeaponDictionary()
    {
        LoadManager.Save<UnlockedStates>(myWeaponStates, "ItemData/weaponData.json");
        LoadManager.Save<UnlockedStates>(myDroneStates, "ItemData/droneData.json");
        
        UpdateUnlockedDataList();
    }

    /// <summary>
    /// Lobby ������ ���Խ� ����. 
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

        Debug.Log("Weapon���� �ҷ����� �Ϸ�");
    }
    
    #endregion


    //#region EquippedWeapon ������ ���� ���� & �ҷ�����

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