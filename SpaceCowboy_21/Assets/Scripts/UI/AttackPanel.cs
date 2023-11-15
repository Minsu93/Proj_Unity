using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackPanel : MonoBehaviour
{
    public Image gunFillImage;

    PlayerWeapon playerWeapon;
    

    private void Awake()
    {
        playerWeapon = GameManager.Instance.weapon;
    }

    private void LateUpdate()
    {
        gunFillImage.fillAmount = (float)playerWeapon.currAmmo / playerWeapon.maxAmmo;
    }
}
