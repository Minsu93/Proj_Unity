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
