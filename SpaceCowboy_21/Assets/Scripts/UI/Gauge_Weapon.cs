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

    //[SerializeField] float currAmmo;
    //[SerializeField] float maxAmmo;

    float maxTime;
    float curTime;
    float lastShootTime;

    float maxAmmo;
    float curAmmo;

    bool activate;

    PlayerWeapon weapon;
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
        DecideShowing();
        if(maxAmmo > 0)
        {
            ammoGauge.fillAmount = curAmmo / maxAmmo;
        }
    }



    //게이지는 curTime 이 MaxTime 보다 작을 때 보여진다. 
    //(수정) maxAmmo가 0이 아니면 보여진다. 그렇지 않으면 보이지 않는다. 
    void DecideShowing()
    {
        if(maxAmmo == 0)
        {
            HideGauge();
        }
        else
        {
            ShowGauge();
        }
    }
    
    void ShowGauge()
    {
        ammoGauge.gameObject.SetActive(true);
    }

    void HideGauge()
    {
        ammoGauge.gameObject.SetActive(false);
    }
}
