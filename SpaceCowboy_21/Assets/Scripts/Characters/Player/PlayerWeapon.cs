using SpaceCowboy;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData[] weapons;
    int currentWeaponIndex = -1;
    float temporAmmo;    //기본 총알 남아있는것
    public float defaultReloadTime = 0.5f;

    public float chargeAmount = 0.1f;
    public float gunPowerMax = 15f;   //POWER 최대 수치
    public float curGunPower;   //현재 남아있는 gunPower수치


    WeaponData currentWeaponData;

    //총기 관련 Scriptable Object 
    int burstNumber; //한번 누를 때 연속 발사 수
    int numberOfProjectiles;    //한번 누를 때 멀티샷 수
    float shootInterval;    //발사 쿨타임
    float burstInterval;    //연속 발사 시 사이 시간
    float projectileSpread; //총알 마다 떨어진 각도
    float randomSpreadAngle;    //총구 흔들림때문에 생기는 랜덤값
    //float recoil;   //총기 반동
    GameObject projectilePrefab;    //총알의 종류
    float damage, lifeTime, speed, knockBackForce;  // Projectile 수치들
    public float maxAmmo { get; private set; }    //총알 탄창의 max수치

    public float currAmmo { get; private set; }     //현재 총의 총알
    int reflectionCount;    //반사 회수

    bool isSingleShot;      //단발식인가요?
    float lastShootTime;    //지난 발사 시간


    //리로드 관련
    bool isReloading;
    public float reloadTimer = 1.0f;    //발사 후 충전 시작까지 걸리는 시간 
    public float reloadSpeed = 3.0f;    //충전 속도
    float rTimer;

    //유물 무기를 사용중인가요?
    bool inArtifact;

    //무기를 바꾸는 중인가요?
    bool isChanging;

    //총 쏘기 시작
    bool shootON;

    public Transform[] gunTips; 
    Transform gunTip;
    public PlayerBehavior playerBehavior;
    Coroutine shootRoutine;
    Coroutine reloadRoutine;
    Coroutine changeRoutine;

    private void Start()
    {
        //기본 총 초기화.
        if (currentWeaponIndex < 0)
        {
            temporAmmo = weapons[0].MaxAmmo;
        } 

        ChangeWeapon(0);
    }

    private void Update()
    {
        //currAmmo는 계속 회복된다


        PlayerState currState = playerBehavior.state;
        if (currState == PlayerState.Die) return;
        if (currState == PlayerState.Stun) return;

        if (!shootON & inArtifact)
        {
            //총알 충전(리로드)
            if (currAmmo < maxAmmo)
            {
                if (rTimer > 0)
                {
                    rTimer -= Time.deltaTime;
                }
                else
                {
                    currAmmo += Time.deltaTime * reloadSpeed ;
                }
            }
            return;
        }

        //리로드 중이면 발사하지 않는다.
        if (isReloading)
            return;

        //무기를 바꾸는 중이면 발사하지 않는다. 
        if (isChanging)
            return;

        //총알이 없는 경우 발사하지 않는다. 
        if(currAmmo < 1)
        {
            return;
        }

        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        if (isSingleShot)
        {
            //단발식
            TryShoot();
            //발사 중지
            StopShoot();
        }
        else
        {
            //연사
            TryShoot();
        }
    }

    #region Shoot Function

    public void StartShoot()
    {
        shootON = true;
    }
    public void StopShoot()
    {
        if (shootON)
        {
            shootON = false;
        } 
    }


    public void TryShoot()  //이벤트 발생을 위해서 > PlayerWeapon, PlayerView 의 ShootEvent를 발생
    {
        //총알 발사구가 행성 내부에 있다면 발사하지 않는다. 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 aimDirection = (mousePos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 1f, LayerMask.GetMask("Planet"));
        if (hit.collider != null)
            return;

        //쏠 때마다 총알 한발씩 제거
        currAmmo -= 1;

        lastShootTime = Time.time;
        rTimer = reloadTimer;

        //발사
        Shoot();

        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

        // 총알을 다 쓴 경우 
        if (currAmmo <= 0)
        {
            //기본 총으로 돌아간다
            inArtifact = false;
            ChangeWeapon(0);
        }


    }

    void Shoot()
    {
        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        //Vector3 dir = gunTip.right; //발사 각도
        Vector3 dir = playerBehavior.aimDirection;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));
            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = transform.position + dir * 1f;
            projectile.transform.rotation = tempRot * randomRotation;
            projectile.GetComponent<Projectile_Player>().init(damage, lifeTime, speed, reflectionCount);
        }

        AudioManager.instance.PlaySfx(currentWeaponData.ShootSFX);
    }

    #endregion

   
    #region ChangeWeapon

    public void ChangeWeapon(int index)
    {
        float cost;

        // index 가 0이라면(리볼버) 상관없이 교체한다.
        if (index != 0) 
        {
            ////사용 가능한 유물 총인 경우에만 무기를 교체한다.
            //cost = weapons[index].PowerCost;
            //if (curGunPower < cost)
            //{
            //    return;
            //}
            //else
            //{
            //    //power소모 
            //    curGunPower -= cost;
            //}
        }

        if (changeRoutine != null)
        {
            StopCoroutine(changeRoutine);
            changeRoutine = null;
        }

        changeRoutine = StartCoroutine(ChangeWeaponRoutine(index));



    }

    IEnumerator ChangeWeaponRoutine(int index)
    {

        //원하는 총으로 바꿔준다.
        InitializeWeapon(index);

        //총 바꾸는 애니메이션이 들어간다 

        isChanging = true;
        //총 바꾸는 데 걸리는 시간 
        yield return null;

        isChanging = false;
        changeRoutine = null;


    }

    public void InitializeWeapon(int index)
    {
        //현재 유물총을 사용중이면 유물총을 다 쓰기 전까지 총을 바꾸지 못한다. <<버그
        if (inArtifact)
            return;

        //리로드 중이면 리로드를 취소하고 진행
        //StopReload();

        //현재 총이 기본 총인 경우, 현재 총알을 temporAmmo에 넣는다
        if (currentWeaponIndex == 0)
        {
            temporAmmo = currAmmo;
        }


        currentWeaponIndex = index;
        currentWeaponData = weapons[currentWeaponIndex];

        //총 별로 총구 위치 변경
        gunTip = gunTips[currentWeaponData.SkinIndex];

        //총 모양 변경
        playerBehavior.TryChangeWeapon(currentWeaponData);


        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        burstNumber = currentWeaponData.BurstNumer; 
        burstInterval = currentWeaponData.BurstInterval;    
        numberOfProjectiles = currentWeaponData.NumberOfProjectiles;  
        shootInterval = currentWeaponData.ShootInterval;   
        projectileSpread = currentWeaponData.ProjectileSpread; 
        randomSpreadAngle = currentWeaponData.RandomSpreadAngle;
        isSingleShot = currentWeaponData.SingleShot;

        projectilePrefab = currentWeaponData.ProjectilePrefab; 
        
        damage = currentWeaponData.Damage;
        lifeTime = currentWeaponData.LifeTime;
        speed = currentWeaponData.Speed;
        reflectionCount = currentWeaponData.ReflectionCount;


        //총알을 초기화
        maxAmmo = currentWeaponData.MaxAmmo;
        
        //기본 총의 경우
        if(index == 0)
        {
            inArtifact = false;
            currAmmo = temporAmmo;
        }
        //유물 총의 경우
        else
        {
            inArtifact = true;
            currAmmo = maxAmmo;

        }


        

    }

    #endregion



    //public void ChargeGunPower()
    //{
    //    curGunPower += chargeAmount;
    //    if(curGunPower > gunPowerMax)
    //    {
    //        curGunPower = gunPowerMax;
    //    }
    //}


}
