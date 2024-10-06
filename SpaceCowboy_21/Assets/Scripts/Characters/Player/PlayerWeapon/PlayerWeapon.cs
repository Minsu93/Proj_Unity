using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    //�ѱ� ����
    [Header("Weapon Property")]
    [SerializeField] float weaponChangeTime = 0.5f; //���� ��ü�ϴµ� �ɸ��� �ð�

    public int weaponSlots { get; set; }
    public float maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ
    public float currAmmo { get; private set; }     //���� ���� �Ѿ�
    public bool shootOn { get; set; }   //�ѽ�� ���ΰ���? 
    bool infiniteBullets;    //�Ѿ��� ����
    bool isChanging;    //���⸦ �ٲٴ� ���ΰ���?
    float range;    //�Ÿ� ǥ�� �� 


    [Header("Weapon Sight")]
    //WeaponSight����
    GameObject weaponRangeSight;     //�����Ÿ� ������
    GameObject weaponRangePoint;    //������ �� spritePoint
    bool showWeaponRange { get; set; }  //�ѱ� �����Ÿ� ���̰� ���� ����.
    bool pointOn;
    int targetLayer;
    [SerializeField] LineRenderer sightLineRenderer;


    [SerializeField] GameObject bone1H;
    [SerializeField] GameObject bone2H;
    public GameObject point1H;
    public GameObject point2H;

    public WeaponData baseWeaponData;
    Dictionary<WeaponData, WeaponType> weaponTypeDictionary = new Dictionary<WeaponData, WeaponType>();
    public Queue<WeaponData> wDataQueue= new Queue<WeaponData>();

    //��ũ��Ʈ ����
    PlayerBehavior playerBehavior;
    public WeaponType currWeaponType;  //���� ���� Ÿ��

    public event System.Action weaponShootAction;


    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        CreateWeaponRange();

        //weaponSight�� Ÿ�� 
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy");
    }
    private void Update()
    {
        UpdateWeaponSight();
        CheckWeaponDurtation();
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

    void ChangeWeaponRangeColor(Color color)
    {
        weaponRangePoint.GetComponent<SpriteRenderer>().color = color;
        //sightLineRenderer.startColor = color;
        //sightLineRenderer.endColor = color;

        var gradient = new Gradient();

        gradient.mode = GradientMode.Blend;

        var gradientColorKeys = new GradientColorKey[2]
        {
            new GradientColorKey(color, 0f),
            new GradientColorKey(color, 1f)
        };

        var alphaKeys = new GradientAlphaKey[4]
        {
            new GradientAlphaKey(0f, 0f),
            new GradientAlphaKey(1f, 0.1f),
            new GradientAlphaKey(1f, 0.9f),
            new GradientAlphaKey(0f, 1f)
        };

        gradient.SetKeys(gradientColorKeys, alphaKeys);

        sightLineRenderer.colorGradient = gradient;

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
                    ChangeWeaponRangeColor(Color.yellow);
                }
                weaponRangePoint.transform.position = pos3;
            }
            else
            {
                if (weaponRangePoint.activeSelf)
                {
                    weaponRangePoint.SetActive(false);
                    ChangeWeaponRangeColor(Color.white);

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
        //if (!infiniteBullets) currAmmo -= 1;
        GameManager.Instance.playerManager.UpdateGaugeUIShootTime(currAmmo);  //UI �� ����

        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

        // �Ѿ��� �� �� ��� 
        if (!infiniteBullets && currAmmo <= 0)
        {
            DequeueData();    //������ ���� ������
        }

        // �Ѿ� �߻� �̺�Ʈ ����
        if (weaponShootAction != null) weaponShootAction();
    }

    #endregion

    #region ChangeWeapon

    public bool EnqueueData(WeaponData data)
    {
        //if(currWeaponType.weaponData == baseWeaponData)
        //{
        //    WeaponType wtype = GetWeaponType(data);
        //    ChangeWeapon(wtype, wtype.maxAmmo, false);
        //    GameManager.Instance.playerManager.UpdateWeaponQueue();
        //    return true;
        //}

        //if (wDataQueue.Count < weaponSlots)
        //{
        //    wDataQueue.Enqueue(data);
        //    Debug.Log("Queue Count is : " + wDataQueue.Count);

        //    //UI�� ������Ʈ �Ѵ� 
        //    GameManager.Instance.playerManager.UpdateWeaponQueue();
        //    return true;
        //}

        //�ٷ� �����ع�����.
        WeaponType wtype = GetWeaponType(data);
        ChangeWeapon(wtype, data.MaxAmmo, false);
        return true;
        //return false;
    }

    public void DequeueData()
    {
        //���ÿ� ������ ��������
        if (wDataQueue.TryDequeue(out WeaponData wData))
        {
            WeaponType wtype = GetWeaponType(wData);
            ChangeWeapon(wtype, wData.MaxAmmo, false);

        }
        //���ÿ� ������ �⺻ �ѱ�� 
        else
        {
            BackToBaseWeapon(false);
        }
        //UI�� ������Ʈ �Ѵ� 
        GameManager.Instance.playerManager.UpdateWeaponQueue();

    }

    //�ѱ� ���ӽð� üũ
    void CheckWeaponDurtation()
    {
        if(weaponDuration > 0)
        {
            weaponDuration -= Time.deltaTime;
            if(weaponDuration <= 0)
            {
                DequeueData();
            }
        }
    }

    //�⺻ �ѱ� ��ȯ
    public void BackToBaseWeapon(bool instant)
    {
        WeaponType wtype = GetWeaponType(baseWeaponData);
        ChangeWeapon(wtype, wtype.maxAmmo, instant);

    }

    public WeaponType GetWeaponType(WeaponData data)
    {
        WeaponType wtype;

        //���� ������� ���Ⱑ ������ �����´�. �׷��� ������ ���� �����Ѵ�. 
        if (weaponTypeDictionary.ContainsKey(data))
        {
            weaponTypeDictionary.TryGetValue(data, out wtype);
        }
        else
        {
            GameObject weaponObj;
            //����
            if (data.GunStyle == GunStyle.OneHand)
            {
                //1H
                weaponObj = Instantiate(data.WeaponPrefab, bone1H.transform);
                wtype = weaponObj.GetComponent<WeaponType>();
                wtype.Initialize(data, point1H.transform.localPosition);
            }
            else
            {
                //2H
                weaponObj = Instantiate(data.WeaponPrefab, bone2H.transform);
                wtype = weaponObj.GetComponent<WeaponType>();
                wtype.Initialize(data, point2H.transform.localPosition);
            }

            wtype.afterShootEvent -= AfterShootProcess;
            wtype.afterShootEvent += AfterShootProcess;
            weaponObj.SetActive(false);

            //Dict�� �߰�
            weaponTypeDictionary.Add(data, wtype);
        }

        return wtype;
    }
    float weaponDuration;
    public void ChangeWeapon(WeaponType weaponType, float curAmmo, bool instant)
    {
        isChanging = !instant;
        shootOn = false;

        //���� ������� ����� �����
        if(currWeaponType != null) 
            currWeaponType.gameObject.SetActive(false);

        //�ٲ� ������ �� ������ �ҷ��´�(�̶�, �ҷ����� �������� �̹� ������ �Ϸ�� �����̴�)
        currWeaponType = weaponType;
        currWeaponType.gameObject.SetActive(true);

        //��Ų
        playerBehavior.TryChangeWeaponSkin(currWeaponType.weaponData);

        //�� ��ũ��Ʈ(Player Weapon) ���� �ʿ��� Ammo, Range ����
        currAmmo = curAmmo;
        maxAmmo = currWeaponType.maxAmmo;
        range = currWeaponType.range;
        weaponDuration = currWeaponType.weaponDuration;
        ShowWeaponSight(currWeaponType.showRange);

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

        ResetBuff();

        GameManager.Instance.playerManager.UpdatePlayerGaugeUI(currWeaponType.weaponData);

        //���� ��ü ��Ÿ��
        if (!instant)
            StartCoroutine(ChangePauseRoutine());
    }

    //���� �ʱ�ȭ �� ���� ���� ������ �����Ѵ�. Player Stats Buff���� ��, �ѱ� ���� �ø��� ��.
    public void ResetBuff()
    {
        //������ �����´�
        WeaponStats buffStats = GameManager.Instance.playerManager.playerBuffs.weaponBuffStats;
        //�����Ѵ�. 
        currWeaponType.ResetWeapon(buffStats);
    }

    IEnumerator ChangePauseRoutine()
    {
        //�� �ٲٴ� �� �ɸ��� �ð� 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }

    #endregion

}


public class AmmoInventory
{
    public WeaponData weaponData;
    public float currAmmo;
}
