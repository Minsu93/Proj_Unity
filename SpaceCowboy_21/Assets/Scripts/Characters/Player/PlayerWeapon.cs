using SpaceCowboy;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData[] weapons;
    int currentWeaponIndex = -1;
    int temporAmmo;    //기본 총알 남아있는것
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
    public int maxAmmo { get; private set; }    //총알 탄창의 max수치

    public int currAmmo { get; private set; }     //현재 총의 총알
    int reflectionCount;    //반사 회수

    bool isSingleShot;      //단발식인가요?
    float lastShootTime;    //지난 발사 시간


    //리로드 관련
    bool isReloading;

    //유물 무기를 사용중인가요?
    bool inArtifact;
    public float artifactLifeTime;

    //무기를 바꾸는 중인가요?
    bool isChanging; 

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

    #region Shoot Function


    public void TryStartShoot()
    {
        //총 쏘기 이벤트를 시작한다. 단발총인 경우는 한 발만 발사. 연발 총인 경우는 발사 루틴을 지속
        //달리기 중인 경우에는 총을 쏘지 못한다. 
        if (playerBehavior.runON)
            return;

        //단발총인 경우에는 shootevent를 한번 실행한다. 쿨이 안되서 다시 눌러도 소용없음. 
        if (isSingleShot)
        {
            TryShoot();
        }

        //연발총인 경우에는 계속 쏘기 이벤트를 실행한다. 
        else
        {
            shootRoutine = StartCoroutine(ShootRepeater());
        }

    }

    public void TryStopShoot()
    {
        //총 쏘기 이벤트를 중단한다. 단발총인 경우는 총 쏘기 초기화. 연발 총의 경우에는 발사 중지. 
        //연발 총인 경우에는 계속 쏘기 이벤트를 중단한다. 
        if (shootRoutine != null)
        {
            StopCoroutine(shootRoutine);
            shootRoutine = null;
        }
    }

    //유물 총 자동 발사 기능
    public void AutoShoot()
    {
        shootRoutine = StartCoroutine(ShootRepeater());
    }

    IEnumerator ShootRepeater()
    {
        while (true)
        {
            //총알 쿨타임마다 계속 발사한다. 
            TryShoot();
            yield return null;
        }
    }


    public void TryShoot()  //이벤트 발생을 위해서 > PlayerWeapon, PlayerView 의 ShootEvent를 발생
    {
        PlayerState currState = playerBehavior.state;
        if (currState == PlayerState.Die) return;
        if (currState == PlayerState.Stun)
            return;

        //리로드 중이면 발사하지 않는다.
        if (isReloading)
            return;

        //무기를 바꾸는 중이면 발사하지 않는다. 
        if(isChanging)
            return;
        

        //총알 발사구가 행성 내부에 있다면 발사하지 않는다. 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 aimDirection = (mousePos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 1f, LayerMask.GetMask("Planet"));
        if (hit.collider != null)
            return;



        float currentTime = Time.time;

        if (currentTime - lastShootTime > shootInterval)
        {
            //총알 발사
            if (currAmmo > 0)
            {
                //쏠 때마다 총알 한발씩 제거
                currAmmo -= 1;

                lastShootTime = currentTime;

                StartCoroutine(ShootRoutine());
                playerBehavior.TryShootEvent();
            }
            else
            {
                //기본 총인 경우 총알이 없으면 리로드 시작
                if (!inArtifact)
                {
                    TryReload();
                    return;
                }
            }
        }
    }





    IEnumerator ShootRoutine()
    {
        yield return null; 

        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        //Vector3 dir = gunTip.right; //발사 각도
        Vector3 dir = playerBehavior.aimDirection;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

       //Burst
        for (int index = 0; index < burstNumber; index++)
        {
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

            if(burstInterval> 0)
                yield return new WaitForSeconds(burstInterval);


        }
        yield return null;

        //총알을 다 쓴 경우
        if (currAmmo <= 0)
        {
            //기본 총의 경우에는 
            if (currentWeaponIndex == 0)
            {
                //자동으로 리로드를 한다
                TryReload();
            }
            //그 외의 총인 경우에는
            else
            {
                //총 바꾸기 타임 
                //기본 총으로 돌아간다
                inArtifact = false;
                ChangeWeapon(0);
            }
        }
    }

    #endregion

    #region Reload
    public void TryReload()
    {
        //재장전 중이 아닐 때 
        if (isReloading)
            return;

        //재장전이 가능할떄 (currAmmo 가 maxAmmo보다 작을때)
        if (currAmmo < maxAmmo)
        {
            reloadRoutine = StartCoroutine(ReloadRoutine());
        }

    }

    void StopReload()
    {
        if (!isReloading)
            return;

        StopCoroutine(reloadRoutine);
        reloadRoutine = null;

        isReloading = false;

        //리로드 애니메이션 정지
        playerBehavior.ReloadEnd();


    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        playerBehavior.ReloadStart();

        yield return new WaitForSeconds(defaultReloadTime);

        currAmmo = maxAmmo;

        StopReload();


    }
    #endregion

    #region ChangeWeapon

    public void ChangeWeapon(int index)
    {
        float cost;
        TryStopShoot();

        // index 가 0이라면(리볼버) 상관없이 교체한다.
        if (index != 0) 
        {
            //사용 가능한 유물 총인 경우에만 무기를 교체한다.
            cost = weapons[index].PowerCost;
            if (curGunPower < cost)
            {
                return;
            }
            else
            {
                //power소모 
                curGunPower -= cost;
            }
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

        //현재 발사중이면 총 쏘기를 정지
        TryStopShoot();

        //리로드 중이면 리로드를 취소하고 진행
        StopReload();

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

        //유물무기 타이머 초기화
        artifactLifeTime = currentWeaponData.ArtifaceLifetime;

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

            AutoShoot();
        }


        

    }

    #endregion

    private void Update()
    {
        //유물무기 타이머
        if(artifactLifeTime > 0)
        {
            artifactLifeTime -= Time.deltaTime;

            if(artifactLifeTime <= 0)
            {
                ChangeWeapon(0);
            }
        }


    }

    public void ChargeGunPower()
    {
        curGunPower += chargeAmount;
        if(curGunPower > gunPowerMax)
        {
            curGunPower = gunPowerMax;
        }
    }


}
