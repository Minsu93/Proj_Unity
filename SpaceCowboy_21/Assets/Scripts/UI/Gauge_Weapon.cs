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

    //���� ���� �� ȣ��Ǵ� �Լ�
    public void WeaponChange(PlayerWeapon playerWeapon, float maxAmmo)
    {
        this.maxAmmo = maxAmmo;
        this.curAmmo = maxAmmo;
        UpdateGauge();
    }

    //���� �� ������ ȣ��Ǵ� �Լ�
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



    //�������� curTime �� MaxTime ���� ���� �� ��������. 
    //(����) maxAmmo�� 0�� �ƴϸ� ��������. �׷��� ������ ������ �ʴ´�. 
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
