using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponDictionary : MonoBehaviour
{
    [SerializeField]
    List<WeaponData> weaponDataList = new List<WeaponData>();   //리스트에는 모든 weaponData들을 넣어두면 된다.
    Dictionary<string, WeaponState> weaponNameDictionary = new Dictionary<string, WeaponState>();
    public WeaponStates myWeaponStates;

    public void SetWeaponState(string name,bool unlocked, bool bought)
    {
        WeaponState newState = new WeaponState();
        newState.name = name;
        newState.unlocked = unlocked;
        newState.bought = bought;

        for(int i  = 0; i < myWeaponStates.states.Count; i++)
        {
            if(myWeaponStates.states[i].name == name)
            {
                newState.price = myWeaponStates.states[i].price;
                newState.weaponData = myWeaponStates.states[i].weaponData;
                myWeaponStates.states[i] = newState;
            }
        }
    }

    public WeaponState GetWeaponState(string name)
    {
        if(!weaponNameDictionary.TryGetValue(name, out WeaponState state))
        {
            Debug.Log("해당 weapon이이 Dictionary에 없습니다");
        }
        return state;
    }

    #region 저장&불러오기

    /// <summary>
    /// 무기 해금, 무기 구매 시 실행.
    /// </summary>
    public void SaveWeaponDictionary()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "weaponData.json");
        string str = JsonUtility.ToJson(myWeaponStates, true);
        File.WriteAllText(path, str);
    }

    /// <summary>
    /// Lobby 상점에 진입시 실행. 
    /// </summary>
    public void LoadWeaponDictionary()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "weaponData.json");
        string loadJson = File.ReadAllText(path);
        myWeaponStates = JsonUtility.FromJson<WeaponStates>(loadJson);

        for (int i = 0; i < myWeaponStates.states.Count; i++)
        {
            string weaponName = myWeaponStates.states[i].name;
            WeaponData data = weaponDataList.Find(item => item.name.Equals(weaponName));
            myWeaponStates.states[i].weaponData = data;
            weaponNameDictionary.Add(weaponName, myWeaponStates.states[i]);
        }

        Debug.Log("Weapon정보 불러오기 완료");
    }

    #endregion


    #region EquippedWeapon 장착된 무기 저장 & 불러오기

    public List<string> equippedNamesList { get; private set; }
    public void SaveEquippedWeapons(List<string> names)
    {
        equippedNamesList = names;

        EquippedWeaponData data = new EquippedWeaponData();
        data.equippedWeaponNames = equippedNamesList;

        string path = Path.Combine(Application.dataPath + "/Data/PlayerData/equippedWeapon.json");
        string str = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, str);
    }

    public void LoadEquippedWeapons()
    {

        string path = Path.Combine(Application.dataPath + "/Data/PlayerData/equippedWeapon.json");
        string loadJson = File.ReadAllText(path);
        EquippedWeaponData data = JsonUtility.FromJson<EquippedWeaponData>(loadJson);

        equippedNamesList = data.equippedWeaponNames;



    }


    #endregion

}

[Serializable]
public class WeaponState
{
    public string name;
    public bool unlocked;
    public bool bought;
    public int price;
    public WeaponData weaponData { get; set; }
}

[Serializable]
public class WeaponStates
{
    public List<WeaponState> states = new List<WeaponState>();
}

[Serializable]
public class EquippedWeaponData
{
    public List<string> equippedWeaponNames = new List<string>();
}