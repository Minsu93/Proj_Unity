using SpaceCowboy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�ڼտ� �����Ͽ� ���ť��� ��ȭ��ų ������ ���ݵ�
[System.Serializable]
public class WeaponStats
{
    //public GameObject projectilePrefab;
    public float damage;
    public float speed;
    public float lifeTime;
    public float range;
    public float shootInterval;
    public float projectileSpread;
    public float randomSpreadAngle;
    public int numberOfProjectile;
    public int projPenetration;
    public int projReflection;
    public int projGuide;
    public int maxAmmo;

    public WeaponShootDelegate weaponShoot;
    public WeaponImpactDelegate weaponImpact;
}

public delegate void WeaponShootDelegate();
public delegate void WeaponImpactDelegate();

public abstract class WeaponType : MonoBehaviour
{
    /// <summary>
    /// �θ�. �� ��� ����� ����ִ�. 
    /// </summary>


    [Header("Weapon Status")] 
    public WeaponStats weaponStats = new WeaponStats();

    //protected int numberOfProjectile;    
    //protected float shootInterval;    
    //protected float projectileSpread; 
    //protected float randomSpreadAngle;   
    //protected float damage;
    //protected float speed;
    //protected float lifeTime;
    //protected int projPenetration;
    //protected int projReflection;
    //protected int projGuide;
    protected GameObject projectilePrefab;
    protected AudioClip shootSFX;     //�߻�� ȿ����

    //PlayerWeapon���� �ʿ��� ����
    public WeaponData weaponData { get; set; }
    //public float range { get; set; }
    //public int maxammo { get; set; }
    public bool showRange { get; set; }
    public GunType finalGunType { get; set; }

    protected float lastShootTime;   //���� �߻� �ð�
    public event System.Action afterShootEvent;     //�ѽ�� ��ó��. ammo����.
    

    //protected WeaponCubeSlots cubeSlots;

    private void Awake()
    {
        //cubeSlots = GetComponent<WeaponCubeSlots>();    
    }


    //�� ���� ��, ť�� ���� ���� �� ����
    public virtual void Initialize(WeaponData weaponData)
    {
        this.weaponData = weaponData;
        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
        WeaponStats baseStats = new WeaponStats();
        baseStats.damage = weaponData.Damage;
        baseStats.speed = weaponData.Speed;
        baseStats.lifeTime = weaponData.LifeTime;
        baseStats.range = weaponData.Range;
        baseStats.shootInterval = weaponData.ShootInterval;
        baseStats.projectileSpread = weaponData.ProjectileSpread;
        baseStats.randomSpreadAngle = weaponData.RandomSpreadAngle;
        baseStats.numberOfProjectile = weaponData.NumberOfProjectile;
        baseStats.projPenetration = weaponData.ProjPenetration;
        baseStats.projReflection = weaponData.ProjReflection;
        baseStats.projGuide = weaponData.ProjGuide;
        baseStats.maxAmmo = weaponData.MaxAmmo;

        projectilePrefab = weaponData.ProjectilePrefab;
        showRange = weaponData.ShowRange;
        finalGunType = weaponData.GunType;

        shootSFX = weaponData.ShootSFX;

        //���� �����ִ� ���Cube���� ������ �����Ѵ�. 
        //weaponStats = cubeSlots.UpdateStats(baseStats);

        //Combination �� Ȯ���Ѵ�
        //CheckCombination();
    }

    public abstract void ShootButtonDown(Vector2 pos, Vector3 dir);
    public abstract void ShootButtonUp(Vector2 pos, Vector3 dir);

    
    protected virtual void AfterShootProcess()
    {
        if (afterShootEvent != null) afterShootEvent();
    }


    //���� ��� �ൿ
    protected virtual void Shoot(Vector2 pos, Vector3 dir)
    {
        float totalSpread = weaponStats.projectileSpread * (weaponStats.numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-weaponStats.randomSpreadAngle * 0.5f, weaponStats.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < weaponStats.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, weaponStats.projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(weaponStats.damage, weaponStats.speed, weaponStats.lifeTime, weaponStats.range, weaponStats.projPenetration, weaponStats.projReflection, weaponStats.projGuide);
            
            if(weaponStats.weaponImpact != null)
            {
                proj.weaponImpactDel = weaponStats.weaponImpact;
            }

        }

        //�߻� �� �̺�Ʈ ����
        WeaponShootEvent();
        //�� �ð� üũ
        lastShootTime = Time.time;
        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);

    }

    
    void WeaponShootEvent()
    {
        if(weaponStats.weaponShoot != null)
        {
            weaponStats.weaponShoot();
        }
    }

    //void CheckCombination()
    //{
    //    int index = cubeSlots.CheckSpecialCombinations();
    //    switch (index)
    //    {
    //        case 0:
    //            //��ȭ�� ����
    //            break;
    //        case 1:
    //            //��ȭ�� A
    //            Varitation_A();
    //            break;
    //        case 2:
    //            //��ȭ�� A
    //            Varitation_B();
    //            break;
    //        case 3:
    //            //��ȭ�� A
    //            Varitation_C();
    //            break;
    //    }
    //}

    ////��ȭ��
    //protected virtual void Varitation_A()
    //{
    //    Debug.Log("AB ���� ����");
    //}
    //protected virtual void Varitation_B()
    //{
    //    Debug.Log("AC ���� ����");
    //}
    //protected virtual void Varitation_C()
    //{
    //    Debug.Log("BC ���� ����");
    //}
}
