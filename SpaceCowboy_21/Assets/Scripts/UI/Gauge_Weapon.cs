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

    public void WeaponChange(PlayerWeapon playerWeapon, float shootInterval)
    {
        weapon = playerWeapon;
        maxTime = shootInterval;
        curTime = shootInterval;
    }

    public void ListenShoot(float lastShootTime)
    {
        this.lastShootTime = lastShootTime;
        curTime = 0;
    }



    private void LateUpdate()
    {
        if (weapon == null) return;

        if (curTime <= maxTime)
        {
            curTime = Time.time - lastShootTime;
        }

        DecideShowing();

        if (!activate) return;
        ammoGauge.fillAmount = curTime / maxTime;


        


        //if (currAmmo / maxAmmo > 0.2f)
        //{
        //    ammoGauge.color = ammoColor;
        //}
        //else
        //{
        //    ammoGauge.color = emptyColor;
        //}

    }

    //게이지는 curTime 이 MaxTime 보다 작을 때 보여진다. 
    void DecideShowing()
    {
        if(curTime < maxTime)
        {
            if (!activate)
            {
                activate = true;
                ShowGauge();
            }
        }
        else
        {
            if (activate)
            {
                activate = false;
                HideGauge();
            }
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
