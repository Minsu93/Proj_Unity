using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData[] weapons;
    int currentWeaponIndex = -1;

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

    public int currAmmo { get; private set; }     //현재 남아있는 총알
    int[] currAmmos;
    int reflectionCount;
    
    public Transform _1H_GunTip;
    public Transform _2H_GunTip;
    Transform gunTip;
    public PlayerBehavior playerBehavior;

    private void Start()
    {
      
        //현재 리스트에 있는 총마다 각각 총알의 수 초기화.
        currAmmos = new int[weapons.Length];

        for(int i = 0; i< weapons.Length; i++)
        {
            currAmmos[i] = weapons[i].MaxAmmo;
        }

        ChangeWeapon(0);


    }



    public bool PlayShootEvent()
    {
        bool needReload = false;

        if(currAmmo > 0)
        {
            //쏠 때마다 총알 한발씩 제거
            currAmmo -= 1;

            StartCoroutine(ShootRoutine());

            needReload = currAmmo > 0 ? false : true;
            return needReload;

        }
        else
        {
            //총알을 발사하지 않는다. 
            needReload = true;
            return needReload;
        }


    }

    IEnumerator ShootRoutine()
    {
        yield return null; 

        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //우선 전체 총알이 퍼질 각도를 구한다
        Vector3 dir = gunTip.right; //발사 각도
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
                projectile.transform.position = gunTip.position;
                projectile.transform.rotation = tempRot * randomRotation;
                projectile.GetComponent<Projectile_Player>().init(damage, lifeTime, speed, reflectionCount);


            }
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Shoot);

            if(burstInterval> 0)
                yield return new WaitForSeconds(burstInterval);

            //총기 반동 >> PlayerBehavior로
            //rb.AddForce(transform.right * -1f * recoil, ForceMode2D.Impulse);

            //총구 이펙트
            /*
            if (muzzleFlash)
                muzzleFlash.Emit(1);
            */
        }
        yield return null;
    }


    public void ChangeWeapon(int index)
    {
        //현재 갖고 있던 총알을 배열에 넣는다. 
        if(currentWeaponIndex >= 0)
        {
            currAmmos[currentWeaponIndex] = currAmmo;
        }

        currentWeaponIndex = index;
        currentWeaponData = weapons[currentWeaponIndex];

        //무기에 따라 guntip변경
        if (currentWeaponData.OneHand)
        {
            gunTip = _1H_GunTip;
        }
        else
        {
            gunTip = _2H_GunTip;
        }

        //초기 설정(변수에 스크립터블 오브젝트 들어감)
        burstNumber = currentWeaponData.BurstNumer; 
        burstInterval = currentWeaponData.BurstInterval;    
        numberOfProjectiles = currentWeaponData.NumberOfProjectiles;  
        shootInterval = currentWeaponData.ShootInterval;   
        projectileSpread = currentWeaponData.ProjectileSpread; 
        randomSpreadAngle = currentWeaponData.RandomSpreadAngle;    
        //recoil = currentWeaponData.Recoil;  

        projectilePrefab = currentWeaponData.ProjectilePrefab; 
        damage = currentWeaponData.Damage;
        maxAmmo = currentWeaponData.MaxAmmo;
        lifeTime = currentWeaponData.LifeTime;
        speed = currentWeaponData.Speed;
        //knockBackForce = currentWeaponData.KnockBackForce;
        reflectionCount = currentWeaponData.ReflectionCount;

        //Player Behavior에 ShootInterval 전달.
        playerBehavior.TryChangeWeapon(currentWeaponData);

        //현재 배열에 있는 총알을 가져온다. 
        currAmmo = currAmmos[currentWeaponIndex];
    }

    public void ReloadAmmo()
    {
        //리로드에는 시간이 필요하다.
        currAmmo = maxAmmo;
    }

    public bool CanReload()
    {
        bool canReload;

        canReload = currAmmo < maxAmmo ? true : false;
        return canReload;
    }

}
