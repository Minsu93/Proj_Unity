using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Image healthBar;

    private Health playerHealth;
    private bool isActive = false;

    private void Awake()
    {
        Transform playerTr = GameManager.Instance.player;
        playerHealth = playerTr.GetComponent<Health>();
        isActive = true;

    }


    private void LateUpdate()
    {
        if (!isActive)
            return;


        healthBar.fillAmount = (float)playerHealth.currHealth / playerHealth.maxHealth;

        //jetPackBar.fillAmount = movement.jetpackFuel / movement.jetpackFuelMax;

        /*
        float currentAmmo = shoot.initialWeapon.currentBulletCount;
        float magazineSize = shoot.initialWeapon.magazineSize;
        ammoBar.fillAmount = currentAmmo / magazineSize;
        */


    }

}
