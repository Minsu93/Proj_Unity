using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData baseWeapon;   //�⺻ �ѱ�


    bool infiniteBullets;    //�Ѿ��� ����
    //float baseAmmo; //�⺻ ������ �Ѿ� �� 
    //float subAmmo; //Ư�� ������ �Ѿ� �� 
    //bool isSubWeapon; //Ư�� �����ΰ���?

    //���ε� ����
    //bool reloadOn = true;  //���ε带 �����ϳ���?
    //[Tooltip("�߻� �� ���� ���۱��� �ɸ��� �ð�")]
    //public float reloadTimer = 1.0f;    //�߻� �� ���� ���۱��� �ɸ��� �ð� 
    //[Tooltip("���� �ӵ�")]
    //public float reloadSpeed = 3.0f;    //���� �ӵ�
    //float rTimer;

    //�ѱ� ����
    int burstNumber; //�ѹ� ���� �� ���� �߻� ��
    int numberOfProjectiles;    //�ѹ� ���� �� ��Ƽ�� ��
    float shootInterval;    //�߻� ��Ÿ��
    float burstInterval;    //���� �߻� �� ���� �ð�
    float projectileSpread; //�Ѿ� ���� ������ ����
    float randomSpreadAngle;    //�ѱ� ��鸲������ ����� ������
    AudioClip shootSFX;     //�߻�� ȿ����

    GameObject projectilePrefab;    //�Ѿ��� ����
    float damage, speed,lifeTime;  // Projectile ��ġ��
    public float maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ
    public float currAmmo { get; private set; }     //���� ���� �Ѿ�
    int reflectionCount;    //�ݻ� Ƚ��

    public float weaponChangeTime = 0.5f; //���� ��ü�ϴµ� �ɸ��� �ð�

    float lastShootTime;    //���� �߻� �ð�
    bool isChanging;    //���⸦ �ٲٴ� ���ΰ���?
    public bool shootON { get; set; }  //�� ��� ����


    PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
    }


    private void Update()   
    {
        //baseAmmo�� ��� ȸ���ȴ�
        if (!playerBehavior.activate) return;

        ////�Ѿ� ����(���ε�)
        //if (!shootON)
        //{
        //    if(!isSubWeapon)
        //    {
        //        if (currAmmo < maxAmmo)
        //        {
        //            if (rTimer > 0)
        //            {
        //                rTimer -= Time.deltaTime;
        //            }
        //            else
        //            {
        //                currAmmo += Time.deltaTime * reloadSpeed;
        //            }
        //        }
        //    }

        //    return;
        //}
        if(!shootON) return;

        //���⸦ �ٲٴ� ���̸� �߻����� �ʴ´�. 
        if (isChanging)
            return;

        //�Ѿ��� ���� ��� �߻����� �ʴ´�. 
        if(currAmmo < 1)
        {
            return;
        }

        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        TryShoot();

        //�߻� ����
        //shootON = false;
    }

    #region Shoot Function


    public void TryShoot()  //�̺�Ʈ �߻��� ���ؼ� > PlayerWeapon, PlayerView �� ShootEvent�� �߻�
    {
        //�� ������ �Ѿ� �ѹ߾� ����
        if(!infiniteBullets) currAmmo -= 1;

        lastShootTime = Time.time;
        //rTimer = reloadTimer;

        //�߻�
        Shoot();

        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

        // �Ѿ��� �� �� ��� 
        if (currAmmo <= 0)
        {
            //ChangeWeapon(baseWeapon);
            playerBehavior.TryChangeWeapon(0);
        }


    }

    void Shoot()
    {
        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        //Vector3 dir = gunTip.right; //�߻� ����
        Vector3 gunTipPos = playerBehavior.gunTipPos;
        Vector3 dir = playerBehavior.aimDirection;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = PoolManager.instance.Get(projectilePrefab);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = tempRot * randomRotation;
            projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);
        }

        AudioManager.instance.PlaySfx(shootSFX);
    }

    #endregion

   
    #region ChangeWeapon

    public void ChangeWeapon(WeaponData weaponData)
    {
        //isSubWeapon = weaponData != baseWeapon;
        lastShootTime = 0f;

        //���ϴ� ������ �ٲ��ش�.
        InitializeWeapon(weaponData);

        StartCoroutine(ChangeWeaponRoutine());

    }

    IEnumerator ChangeWeaponRoutine()
    {
        isChanging = true;
        //�� �ٲٴ� �� �ɸ��� �ð� 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }

    public void InitializeWeapon(WeaponData weaponData)
    {
        //��Ų�� ����
        //playerBehavior.TryChangeWeapon(weaponData.GunType);

        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
        burstNumber = weaponData.BurstNumer; 
        burstInterval = weaponData.BurstInterval;    
        numberOfProjectiles = weaponData.NumberOfProjectiles;  
        shootInterval = weaponData.ShootInterval;   
        projectileSpread = weaponData.ProjectileSpread; 
        randomSpreadAngle = weaponData.RandomSpreadAngle;
        shootSFX = weaponData.ShootSFX;

        projectilePrefab = weaponData.ProjectilePrefab; 
        damage = weaponData.Damage;
        speed = weaponData.Speed;
        lifeTime = weaponData.LifeTime;
        reflectionCount = weaponData.ReflectionCount;

        currAmmo = weaponData.MaxAmmo;
        maxAmmo = weaponData.MaxAmmo;

        infiniteBullets = weaponData.InfiniteAmmo;

    }

    #endregion


}
