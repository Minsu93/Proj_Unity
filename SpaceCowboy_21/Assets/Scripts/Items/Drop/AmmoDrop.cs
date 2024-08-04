using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : AutoCollectable
{
    [SerializeField] WeaponData weaponData;

    protected override void ConsumeEvent()
    {
        //GameManager.Instance.playerManager.CollectWeaponEnergy(weaponData, amount);
    }

}
