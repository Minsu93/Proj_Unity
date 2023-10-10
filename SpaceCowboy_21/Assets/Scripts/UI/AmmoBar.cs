using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoBar : MonoBehaviour
{
    public TextMeshProUGUI currAmmo;
    public TextMeshProUGUI maxAmmo;

    public PlayerWeapon weapon;

    private void Awake()
    {
        weapon = GameManager.Instance.player.GetComponent<PlayerWeapon>();
    }

    private void LateUpdate()
    {

        currAmmo.text = weapon.currAmmo.ToString();
        maxAmmo.text = weapon.maxAmmo.ToString();   
    }
}
