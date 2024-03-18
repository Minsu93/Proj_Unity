using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge_Ammo : MonoBehaviour
{
    public Image ammoGauge;

    public Color ammoColor;
    public Color emptyColor;

    [SerializeField] float currAmmo;
    [SerializeField] float maxAmmo;
    bool activate;
    PlayerWeapon weapon;


    private void Awake()
    {
        weapon = GameManager.Instance.player.GetComponent<PlayerWeapon>();
    }

    private void LateUpdate()
    {
        if (weapon == null) return;

        currAmmo = weapon.currAmmo;
        maxAmmo = weapon.maxAmmo;
        
        DecideShowing();

        if (!activate) return;

        if (currAmmo / maxAmmo > 0.2f)
        {
            ammoGauge.color = ammoColor;
        }
        else
        {
            ammoGauge.color = emptyColor;
        }

        ammoGauge.fillAmount = currAmmo / maxAmmo;
    }

    //보여줄 지 말지 고민.
    void DecideShowing()
    {
        if(maxAmmo - currAmmo < 0.1f)
        {
            if (activate)
            {
                HideGauge();
            }
        }
        else
        {
            if (!activate)
            {
                ShowGauge();
            }
        }
    }
    
    void ShowGauge()
    {
        activate = true;
        ammoGauge.gameObject.SetActive(true);
    }

    void HideGauge()
    {
        activate = false;
        ammoGauge.gameObject.SetActive(false);
    }
}
