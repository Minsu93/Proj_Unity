using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData[] weapons;
    int currentWeaponIndex = -1;

    WeaponData currentWeaponData;
    //�ѱ� ���� Scriptable Object 
    int burstNumber; //�ѹ� ���� �� ���� �߻� ��
    int numberOfProjectiles;    //�ѹ� ���� �� ��Ƽ�� ��
    float shootInterval;    //�߻� ��Ÿ��
    float burstInterval;    //���� �߻� �� ���� �ð�
    float projectileSpread; //�Ѿ� ���� ������ ����
    float randomSpreadAngle;    //�ѱ� ��鸲������ ����� ������
    //float recoil;   //�ѱ� �ݵ�
    GameObject projectilePrefab;    //�Ѿ��� ����
    float damage, lifeTime, speed, knockBackForce;  // Projectile ��ġ��
    public int maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ

    public int currAmmo { get; private set; }     //���� �����ִ� �Ѿ�
    int[] currAmmos;
    int reflectionCount;
    
    public Transform _1H_GunTip;
    public Transform _2H_GunTip;
    Transform gunTip;
    public PlayerBehavior playerBehavior;

    private void Start()
    {
      
        //���� ����Ʈ�� �ִ� �Ѹ��� ���� �Ѿ��� �� �ʱ�ȭ.
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
            //�� ������ �Ѿ� �ѹ߾� ����
            currAmmo -= 1;

            StartCoroutine(ShootRoutine());

            needReload = currAmmo > 0 ? false : true;
            return needReload;

        }
        else
        {
            //�Ѿ��� �߻����� �ʴ´�. 
            needReload = true;
            return needReload;
        }


    }

    IEnumerator ShootRoutine()
    {
        yield return null; 

        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�
        Vector3 dir = gunTip.right; //�߻� ����
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

       //Burst
        for (int index = 0; index < burstNumber; index++)
        {
            //��Ƽ��
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));
                //�Ѿ� ����
                GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
                projectile.transform.position = gunTip.position;
                projectile.transform.rotation = tempRot * randomRotation;
                projectile.GetComponent<Projectile_Player>().init(damage, lifeTime, speed, reflectionCount);


            }
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Shoot);

            if(burstInterval> 0)
                yield return new WaitForSeconds(burstInterval);

            //�ѱ� �ݵ� >> PlayerBehavior��
            //rb.AddForce(transform.right * -1f * recoil, ForceMode2D.Impulse);

            //�ѱ� ����Ʈ
            /*
            if (muzzleFlash)
                muzzleFlash.Emit(1);
            */
        }
        yield return null;
    }


    public void ChangeWeapon(int index)
    {
        //���� ���� �ִ� �Ѿ��� �迭�� �ִ´�. 
        if(currentWeaponIndex >= 0)
        {
            currAmmos[currentWeaponIndex] = currAmmo;
        }

        currentWeaponIndex = index;
        currentWeaponData = weapons[currentWeaponIndex];

        //���⿡ ���� guntip����
        if (currentWeaponData.OneHand)
        {
            gunTip = _1H_GunTip;
        }
        else
        {
            gunTip = _2H_GunTip;
        }

        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
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

        //Player Behavior�� ShootInterval ����.
        playerBehavior.TryChangeWeapon(currentWeaponData);

        //���� �迭�� �ִ� �Ѿ��� �����´�. 
        currAmmo = currAmmos[currentWeaponIndex];
    }

    public void ReloadAmmo()
    {
        //���ε忡�� �ð��� �ʿ��ϴ�.
        currAmmo = maxAmmo;
    }

    public bool CanReload()
    {
        bool canReload;

        canReload = currAmmo < maxAmmo ? true : false;
        return canReload;
    }

}
