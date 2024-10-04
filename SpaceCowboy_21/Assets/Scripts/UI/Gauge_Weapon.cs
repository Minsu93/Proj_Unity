using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge_Weapon : MonoBehaviour
{
    //총알 전체량이 아니라 발사까지 남은 시간을 알려주는 용도로 사용하자. 
    //Interval 과 LastShootTime을 알아온다. 

    [SerializeField] Image ammoGauge;
    [SerializeField] Color ammoColor;
    [SerializeField] Color emptyColor;

    float maxAmmo;
    float curAmmo;

    FollowOBJ followObj;


    private void Awake()
    {
        followObj = GetComponentInChildren<FollowOBJ>();
    }

    public void setFollowTarget(GameObject obj)
    {
        followObj.followingObj = obj;
    }

    //무기 변경 시 호출되는 함수
    public void WeaponChange(PlayerWeapon playerWeapon, float maxAmmo)
    {
        this.maxAmmo = maxAmmo;
        this.curAmmo = maxAmmo;
        UpdateGauge();
    }

    //총을 쏠 때마다 호출되는 함수
    public void ListenShoot(float curAmmo)
    {
        this.curAmmo = curAmmo;
        UpdateGauge();
    }

    void UpdateGauge()
    {
        //DecideShowing();
        if (maxAmmo == 0)
        {
            ammoGauge.gameObject.SetActive(false);
        }
        else
        {
            ammoGauge.gameObject.SetActive(true);
        }


        if (maxAmmo > 0)
        {
            ammoGauge.fillAmount = curAmmo / maxAmmo;
        }
    }


}
