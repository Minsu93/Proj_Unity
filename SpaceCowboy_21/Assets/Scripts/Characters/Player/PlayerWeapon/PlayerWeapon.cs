using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    //총기 관련
    [Header("Weapon Property")]
    [SerializeField] float weaponChangeTime = 0.5f; //무기 교체하는데 걸리는 시간

    public int weaponSlots { get; set; }
    public float maxAmmo { get; private set; }    //총알 탄창의 max수치
    public float currAmmo { get; private set; }     //현재 총의 총알
    public bool shootOn { get; set; }   //총쏘는 중인가요? 
    bool infiniteBullets;    //총알이 무한
    bool isChanging;    //무기를 바꾸는 중인가요?
    float range;    //거리 표시 용 


    [Header("Weapon Sight")]
    //WeaponSight관련
    GameObject weaponRangeSight;     //사정거리 가늠자
    GameObject weaponRangePoint;    //가늠자 끝 spritePoint
    bool showWeaponRange { get; set; }  //총기 사정거리 보이게 할지 말지.
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

    //스크립트 관련
    PlayerBehavior playerBehavior;
    public WeaponType currWeaponType;  //현재 웨폰 타입

    public event System.Action weaponShootAction;


    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        CreateWeaponRange();

        //weaponSight용 타겟 
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy");
    }
    private void Update()
    {
        UpdateWeaponSight();
        CheckWeaponDurtation();
    }

    #region WeaponSight
    //총기 사정거리 표시기 생성
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
        //무기를 바꾸는 중이면 발사하지 않는다. 
        if (isChanging) return;

        //무한 탄창이 아니며, 총알이 없는 경우 발사하지 않는다. 
        if (!infiniteBullets && currAmmo < 1) return;

        if (!shootOn) shootOn = true;

        //사격
        currWeaponType.ShootButtonDown(playerBehavior.gunTipPos, playerBehavior.aimDirection);

    }

    //총쏘기 초기화
    public void ShootOffProcess()
    {
        if(shootOn) shootOn = false;
        currWeaponType.ShootButtonUp(playerBehavior.gunTipPos, playerBehavior.aimDirection);
    }

    void AfterShootProcess()
    {
        //쏠 때마다 총알 한발씩 제거
        //if (!infiniteBullets) currAmmo -= 1;
        GameManager.Instance.playerManager.UpdateGaugeUIShootTime(currAmmo);  //UI 에 전달

        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

        // 총알을 다 쓴 경우 
        if (!infiniteBullets && currAmmo <= 0)
        {
            DequeueData();    //마지막 무기 꺼내기
        }

        // 총알 발사 이벤트 실행
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

        //    //UI를 업데이트 한다 
        //    GameManager.Instance.playerManager.UpdateWeaponQueue();
        //    return true;
        //}

        //바로 장착해버리기.
        WeaponType wtype = GetWeaponType(data);
        ChangeWeapon(wtype, data.MaxAmmo, false);
        return true;
        //return false;
    }

    public void DequeueData()
    {
        //스택에 있으면 가져오기
        if (wDataQueue.TryDequeue(out WeaponData wData))
        {
            WeaponType wtype = GetWeaponType(wData);
            ChangeWeapon(wtype, wData.MaxAmmo, false);

        }
        //스택에 없으면 기본 총기로 
        else
        {
            BackToBaseWeapon(false);
        }
        //UI를 업데이트 한다 
        GameManager.Instance.playerManager.UpdateWeaponQueue();

    }

    //총기 지속시간 체크
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

    //기본 총기 소환
    public void BackToBaseWeapon(bool instant)
    {
        WeaponType wtype = GetWeaponType(baseWeaponData);
        ChangeWeapon(wtype, wtype.maxAmmo, instant);

    }

    public WeaponType GetWeaponType(WeaponData data)
    {
        WeaponType wtype;

        //현재 만들어진 무기가 있으면 가져온다. 그렇지 않으면 새로 생성한다. 
        if (weaponTypeDictionary.ContainsKey(data))
        {
            weaponTypeDictionary.TryGetValue(data, out wtype);
        }
        else
        {
            GameObject weaponObj;
            //생성
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

            //Dict에 추가
            weaponTypeDictionary.Add(data, wtype);
        }

        return wtype;
    }
    float weaponDuration;
    public void ChangeWeapon(WeaponType weaponType, float curAmmo, bool instant)
    {
        isChanging = !instant;
        shootOn = false;

        //현재 사용중인 무기는 숨긴다
        if(currWeaponType != null) 
            currWeaponType.gameObject.SetActive(false);

        //바꿀 무기의 현 정보를 불러온다(이때, 불러오는 정보들은 이미 갱신이 완료된 정보이다)
        currWeaponType = weaponType;
        currWeaponType.gameObject.SetActive(true);

        //스킨
        playerBehavior.TryChangeWeaponSkin(currWeaponType.weaponData);

        //이 스크립트(Player Weapon) 에서 필요한 Ammo, Range 갱신
        currAmmo = curAmmo;
        maxAmmo = currWeaponType.maxAmmo;
        range = currWeaponType.range;
        weaponDuration = currWeaponType.weaponDuration;
        ShowWeaponSight(currWeaponType.showRange);

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

        ResetBuff();

        GameManager.Instance.playerManager.UpdatePlayerGaugeUI(currWeaponType.weaponData);

        //무기 교체 쿨타임
        if (!instant)
            StartCoroutine(ChangePauseRoutine());
    }

    //버프 초기화 시 현재 무기 스텟을 리셋한다. Player Stats Buff에서 콜, 총기 변경 시마다 콜.
    public void ResetBuff()
    {
        //버프를 가져온다
        WeaponStats buffStats = GameManager.Instance.playerManager.playerBuffs.weaponBuffStats;
        //적용한다. 
        currWeaponType.ResetWeapon(buffStats);
    }

    IEnumerator ChangePauseRoutine()
    {
        //총 바꾸는 데 걸리는 시간 
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
