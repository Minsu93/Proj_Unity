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
    public float maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ
    public float currAmmo { get; private set; }     //���� ���� �Ѿ�
    [SerializeField] float weaponChangeTime = 0.5f; //���� ��ü�ϴµ� �ɸ��� �ð�
    public bool shootOn { get; set; }   //�ѽ�� ���ΰ���? 
    bool infiniteBullets;    //�Ѿ��� ����
    bool isChanging;    //���⸦ �ٲٴ� ���ΰ���?
    float range;    //�Ÿ� ǥ�� �� 

    //WeaponSight����
    GameObject weaponRangeSight;     //�����Ÿ� ������
    GameObject weaponRangePoint;    //������ �� spritePoint
    bool showWeaponRange { get; set; }  //�ѱ� �����Ÿ� ���̰� ���� ����.
    bool pointOn;
    int targetLayer;
    [SerializeField] LineRenderer sightLineRenderer;

    //���� ����
    Dictionary<WeaponData, WeaponType> weaponDictionary = new Dictionary<WeaponData, WeaponType>();
    
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
        GameManager.Instance.playerManager.UpdateGaugeUIShootTime(currAmmo);  //UI �� ����

        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

        // �Ѿ��� �� �� ��� 
        if (!infiniteBullets && currAmmo <= 0)
        {
            GameManager.Instance.playerManager.ChangeWeapon(0);             //���� �⺻���� ����
        }
    }

    #endregion
   
    #region ChangeWeapon

    public void InitializeWeapon(WeaponData data)
    {
        WeaponType wtype;

        //���� ������� ���Ⱑ ������ �����´�. �׷��� ������ ���� �����Ѵ�. 
        if (weaponDictionary.ContainsKey(data))
        {
            weaponDictionary.TryGetValue(data, out wtype);
        }
        else
        {
            //����
            GameObject weaponObj = Instantiate(data.WeaponPrefab, weaponSlot.transform);
            wtype = weaponObj.GetComponent<WeaponType>();
            wtype.Initialize(data);
            wtype.afterShootEvent -= AfterShootProcess;
            wtype.afterShootEvent += AfterShootProcess;
            weaponObj.SetActive(false);

            //Dict�� �߰�
            weaponDictionary.Add(data, wtype);
        }

        ChangeWeapon(wtype);
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        isChanging = true;
        shootOn = false;

        //���� ������� ����� �����
        if(currWeaponType != null) 
            currWeaponType.gameObject.SetActive(false);

        //�ٲ� ������ �� ������ �ҷ��´�(�̶�, �ҷ����� �������� �̹� ������ �Ϸ�� �����̴�)
        currWeaponType = weaponType;
        currWeaponType.gameObject.SetActive(true);

        //��Ų
        GunType finalSkin = currWeaponType.finalGunType;
        playerBehavior.TryChangeWeaponSkin(finalSkin);

        //�� ��ũ��Ʈ(Player Weapon) ���� �ʿ��� Ammo, Range ����
        WeaponStats weaponStats = currWeaponType.weaponStats;

        currAmmo = weaponStats.maxAmmo;
        maxAmmo = weaponStats.maxAmmo;
        range = weaponStats.range;
        ShowWeaponSight(true);

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

        //���� ��ü ��Ÿ��
        StartCoroutine(ChangePauseRoutine());
    }

    IEnumerator ChangePauseRoutine()
    {
        //�� �ٲٴ� �� �ɸ��� �ð� 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }


    #endregion



}


