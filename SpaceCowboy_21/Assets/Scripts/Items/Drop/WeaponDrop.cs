using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : Collectable
{
    public WeaponData weaponData;
    PlayerBehavior playerBehavior;

    protected override void ConsumeEffect()
    {
        playerBehavior.TryChangeWeapon(weaponData);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerBehavior = GameManager.Instance.PlayerBehavior;
    }

}
