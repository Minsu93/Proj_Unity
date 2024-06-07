using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge_Weapon : MonoBehaviour
{
    //�Ѿ� ��ü���� �ƴ϶� �߻���� ���� �ð��� �˷��ִ� �뵵�� �������. 
    //Interval �� LastShootTime�� �˾ƿ´�. 

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

    //�������� curTime �� MaxTime ���� ���� �� ��������. 
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
