using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDictionary : MonoBehaviour
{
    [SerializeField]
    List<WeaponData> weaponDataList = new List<WeaponData>();
    Dictionary<string, WeaponData> weaponNameDictionary = new Dictionary<string, WeaponData>();

    private void Awake()
    {
        for (int i = 0; i < weaponDataList.Count; i++)
        {
            weaponNameDictionary.Add(weaponDataList[i].name, weaponDataList[i]);
        }
    }

    public WeaponData GetWeaponData(string name)
    {
        if (!weaponNameDictionary.TryGetValue(name, out WeaponData data))
        {
            Debug.Log("해당 skill이 Dictionary에 없습니다");
        }

        return data;
    }
}
