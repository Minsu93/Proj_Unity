using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble_Weapon : SelfCollectable
{
    [SerializeField] private WeaponData weaponData;

    protected override bool ConsumeEvent()
    {
        //무기 교체
        return GameManager.Instance.playerManager.ChangeWeapon(weaponData);
    }
}
