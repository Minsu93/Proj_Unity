using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Image Bar;

    private Oxygen pOxygen;
    private bool isActive = false;

    private void Awake()
    {
        Transform playerTr = GameManager.Instance.player;
        pOxygen = playerTr.GetComponent<Oxygen>();
        isActive = true;

    }


    private void LateUpdate()
    {
        if (!isActive)
            return;


        Bar.fillAmount = (float)pOxygen.currOxygen / pOxygen.oxygenMax;

        //jetPackBar.fillAmount = movement.jetpackFuel / movement.jetpackFuelMax;

        /*
        float currentAmmo = shoot.initialWeapon.currentBulletCount;
        float magazineSize = shoot.initialWeapon.magazineSize;
        ammoBar.fillAmount = currentAmmo / magazineSize;
        */


    }

}
