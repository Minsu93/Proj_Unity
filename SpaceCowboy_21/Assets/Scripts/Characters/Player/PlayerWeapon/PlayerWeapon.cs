using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerWeapon : MonoBehaviour
{
    //�ѱ� ����
    public WeaponData baseWeapon { get; set; }   //�⺻ �ѱ�
    WeaponData curWeapon;
    bool infiniteBullets;    //�Ѿ��� ����
    float range;    //�Ÿ� ǥ�� �� 
    public float maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ
    public float currAmmo { get; private set; }     //���� ���� �Ѿ�


    [SerializeField] float weaponChangeTime = 0.5f; //���� ��ü�ϴµ� �ɸ��� �ð�
    bool isChanging;    //���⸦ �ٲٴ� ���ΰ���?
    public bool shootOn { get; set; }   //�ѽ�� ���ΰ���? 

    //WeaponSight����
    bool showWeaponRange { get; set; }  //�ѱ� �����Ÿ� ���̰� ���� ����.
    GameObject weaponRangeSight;     //�����Ÿ� ������
    GameObject weaponRangePoint;    //������ �� spritePoint
    bool pointOn;
    int targetLayer;
    [SerializeField] LineRenderer sightLineRenderer;

    WeaponType[] weaponPrefabs = new WeaponType[4];
 
    
    //��ũ��Ʈ ����
    PlayerBehavior playerBehavior;
    GameObject weaponSlot;  //WeaponType ���� ���
    WeaponType currWeaponType;  //���� ���� Ÿ��


    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        CreateWeaponRange();

        //weaponSight�� Ÿ�� 
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy");

        //���� ���Ի���.
        weaponSlot = new GameObject();
        weaponSlot.name = "WeaponSlot";
        weaponSlot.transform.parent = transform;

    }
    private void Update()
    {
        UpdateWeaponSight();
    }

    #region WeaponSight
    //�ѱ� �����Ÿ� ǥ�ñ� ����
    void CreateWeaponRange()
    {
        weaponRangeSight = sightLineRenderer.gameObject;

        weaponRangePoint = new GameObject("WeaponRangeSight");
        weaponRangePoint.transform.parent = weaponRangeSight.transform;
        SpriteRenderer spr = weaponRangePoint.AddComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>("UI/Circle16");
        spr.sortingLayerName = "Effect";

        weaponRangePoint.SetActive(false);
        weaponRangeSight.SetActive(false);
        showWeaponRange = false;
        pointOn = false;

    }
    public void ShowWeaponSight(bool visible)
    {
        weaponRangeSight.SetActive(visible);
        showWeaponRange = visible;
     
    }

    void UpdateWeaponSight()
    {
        if (showWeaponRange)
        {
            float targetDist;
            RaycastHit2D hit = Physics2D.Raycast(playerBehavior.gunTipPos, playerBehavior.aimDirection, range, targetLayer);
            if (hit.collider != null)
            {
                targetDist = hit.distance;
                pointOn = true;
            }
            else
            {
                targetDist = range;
                pointOn = false;
            }

            Vector2 pos0 = playerBehavior.gunTipPos;
            Vector2 pos1 = pos0 + (playerBehavior.aimDirection * targetDist * 0.2f);
            Vector2 pos2 = pos0 + (playerBehavior.aimDirection * targetDist * 0.8f);
            Vector2 pos3 = pos0 + (playerBehavior.aimDirection * targetDist);

            sightLineRenderer.SetPosition(0, pos0);
            sightLineRenderer.SetPosition(1, pos1);
            sightLineRenderer.SetPosition(2, pos2);
            sightLineRenderer.SetPosition(3, pos3);

            if (pointOn)
            {
                if (!weaponRangePoint.activeSelf)
                {
                    weaponRangePoint.SetActive(true);
                }
                weaponRangePoint.transform.position = pos3;
            }
            else
            {
                if (weaponRangePoint.activeSelf)
                {
                    weaponRangePoint.SetActive(false);
                }
            }

        }
    }

    #endregion

    #region Shoot Function

    public void ShootProcess() 
    {
        //���⸦ �ٲٴ� ���̸� �߻����� �ʴ´�. 
        if (isChanging) return;

        //���� źâ�� �ƴϸ�, �Ѿ��� ���� ��� �߻����� �ʴ´�. 
        if (!infiniteBullets && currAmmo < 1) return;

        if (!shootOn) shootOn = true;

        //���
        currWeaponType.ShootButtonDown(playerBehavior.gunTipPos, playerBehavior.aimDirection);

    }

    //�ѽ�� �ʱ�ȭ
    public void ShootOffProcess()
    {
        if(shootOn) shootOn = false;
        currWeaponType.ShootButtonUp(playerBehavior.gunTipPos, playerBehavior.aimDirection);
    }

    void AfterShootProcess()
    {
        //�� ������ �Ѿ� �ѹ߾� ����
        if (!infiniteBullets) currAmmo -= 1;
        //lastShootTime = Time.time;
        GameManager.Instance.playerManager.UpdateGaugeUIShootTime(currAmmo);  //UI �� ����


        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

        // ���� �⺻ ���� �ƴϰ�, �Ѿ��� �� �� ��� 
        if (curWeapon != baseWeapon && currAmmo <= 0)
        {
            GameManager.Instance.playerManager.ResetPlayerWeaponUI();     //����ϴ� ���� ���
            //GameManager.Instance.playerManager.ChangeWeapon(0);             //���� �⺻���� ����
            ChangeWeapon(0);
            //playerBehavior.TryChangeWeapon(baseWeapon);
        }
    }

    //void Shoot()
    //{
    //    float totalSpread = projectileSpread * (numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

    //    Vector3 gunTipPos = playerBehavior.gunTipPos;
    //    Vector3 dir = playerBehavior.aimDirection;

    //    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
    //    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

    //    //���� ���� �߰�
    //    float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
    //    Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

    //    //��Ƽ��
    //    for (int i = 0; i < numberOfProjectile; i++)
    //    {
    //        Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

    //        //�Ѿ� ����
    //        GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
    //        projectile.transform.position = gunTipPos;
    //        projectile.transform.rotation = tempRot * randomRotation;
    //        Projectile proj = projectile.GetComponent<Projectile>();
    //        proj.Init(damage, speed, lifeTime, range, projPenetration, projReflection, projGuide);
    //    }

    //    //���� ����
    //    GameManager.Instance.audioManager.PlaySfx(shootSFX);
    //}

    #endregion
   
    #region ChangeWeapon
    //���� ���� �� �̸� ���⸦ �����صд�. ������ ������ �����߿� �ٲ��� �����ϱ�. 
    public void PreInitializeWeapons(WeaponInventory[] weapons)
    {
        if(weapons.Length > 4)
        {
            throw new Exception("������ ���� 4���� �����ϴ�");
        }
        for(int i = 0; i < weapons.Length; i++)
        {
            GameObject weaponObj = Instantiate(weapons[i].weaponData.WeaponPrefab, weaponSlot.transform);
            WeaponType weaponType = weaponObj.GetComponent<WeaponType>();
            weaponType.Initialize(weapons[i].weaponData);
            weaponType.afterShootEvent -= AfterShootProcess;
            weaponType.afterShootEvent += AfterShootProcess;
            weaponPrefabs[i] = weaponType;
            weaponObj.SetActive(false);
        }
    }

    public void ChangeWeapon(int index)
    {
        isChanging = true;
        shootOn = false;
        //���� ������� ����� �����
        if(currWeaponType != null) 
            currWeaponType.gameObject.SetActive(false);

        //�ٲ� ������ �� ������ �ҷ��´�(�̶�, �ҷ����� �������� �̹� ������ �Ϸ�� �����̴�)
        currWeaponType = weaponPrefabs[index];
        currWeaponType.gameObject.SetActive(true);

        //��Ų
        GunType finalSkin = currWeaponType.finalGunType;
        playerBehavior.TryChangeWeaponSkin(finalSkin);

        //�� ��ũ��Ʈ(Player Weapon) ���� �ʿ��� �͵��� ����.
        WeaponStats weaponStats = currWeaponType.weaponStats;
        InitializePlayerWeapon(currWeaponType.weaponData, weaponStats.range, weaponStats.maxAmmo);

        StartCoroutine(ChangePauseRoutine());
    }

    IEnumerator ChangePauseRoutine()
    {
        //�� �ٲٴ� �� �ɸ��� �ð� 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }

    //public void InitializeWeapon(WeaponData weaponData)
    //{
    //    //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
    //    curWeapon = weaponData;
    //    range = weaponData.Range;

    //    currAmmo = weaponData.MaxAmmo;
    //    maxAmmo = weaponData.MaxAmmo;

    //    ShowWeaponSight(weaponData.ShowRange);

    //    if (maxAmmo == 0) infiniteBullets = true;
    //    else infiniteBullets = false;

    //    //weaponSlot �� BP ����. 
    //    if(currWeaponType != null) Destroy(currWeaponType.gameObject);  //���� ������ ����
    //    GameObject weaponObj = Instantiate(weaponData.WeaponPrefab, weaponSlot.transform);
    //    currWeaponType = weaponObj.GetComponent<WeaponType>();
    //    currWeaponType.Initialize(weaponData);
    //    currWeaponType.afterShootEvent += AfterShootProcess;


    //}
    void InitializePlayerWeapon(WeaponData weaponData, float range,int maxAmmo)
    {
        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
        curWeapon = weaponData;

        currAmmo = maxAmmo;
        this.maxAmmo = maxAmmo;

        this.range = range;
        ShowWeaponSight(true);

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

    }
    //���� �ѱ⽺�� ����.
    //void GetFinalStat()
    //{
    //    damage = damage * ((100 + damagePlus)/100);
    //    shootInterval = shootInterval * (100 / (100 + fireRatePlus));
    //    speed = speed * (100 + projSpeedPlus) / 100;
    //    range = range * (100 + projRangePlus) / 100;
    //    numberOfProjectile = projNumberPlus + 1;
    //}

    #endregion



}


