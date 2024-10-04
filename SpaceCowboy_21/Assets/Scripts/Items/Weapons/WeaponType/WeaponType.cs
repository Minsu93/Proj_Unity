using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;



public delegate void WeaponShootDelegate();
public delegate void WeaponImpactDelegate();


public abstract class WeaponType : MonoBehaviour
{
    /// <summary>
    /// �θ�. �� ��� ����� ����ִ�. 
    /// </summary>

    [Header("Weapon Status")]
    //public int itemID;
    protected WeaponStats baseStats = new WeaponStats();
    protected WeaponStats weaponStats = new WeaponStats();
    protected float lifeTime;
    protected float projectileSpread;
    protected float randomSpreadAngle;
    protected float speedVariation;
    protected float burstInterval;
    protected int numberOfProjectile;
    protected int numberOfBurst;

    protected GameObject projectilePrefab;
    protected AudioClip shootSFX;  

    //PlayerWeapon���� �ʿ��� ����
    public WeaponData weaponData { get; set; }
    public bool showRange { get; set; }
    public float range { get; set; }
    public int maxAmmo { get; set; }
    public float weaponDuration { get; private set; }


    protected Transform gunTipTr;
    [SerializeField] ParticleSystem muzzleFlashVFX;


    protected float lastShootTime;   //���� �߻� �ð�
    
    public event System.Action afterShootEvent;     //�ѽ�� ��ó��. ammo����.
    

    //�� ���� �� ����
    public virtual void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        this.weaponData = weaponData;
        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
        baseStats.damage = weaponData.Damage;
        baseStats.speed = weaponData.Speed;
        baseStats.shootInterval = weaponData.ShootInterval;
        
        burstInterval = weaponData.BurstInterval;
        projectileSpread = weaponData.ProjectileSpread;
        randomSpreadAngle = weaponData.RandomSpreadAngle;
        speedVariation = weaponData.SpeedVariation;
        numberOfProjectile = weaponData.NumberOfProjectile;
        numberOfBurst = weaponData.NumberOfBurst;
        lifeTime = weaponData.LifeTime;
        range = weaponData.Range;
        maxAmmo = weaponData.MaxAmmo;
        weaponDuration = weaponData.Duration;
        
        //��Ÿ ����
        projectilePrefab = weaponData.ProjectilePrefab;
        showRange = weaponData.ShowRange;
        shootSFX = weaponData.ShootSFX;

        gunTipTr = transform.GetChild(0);
        gunTipTr.localPosition = gunTipLocalPos;
        
    }

    //�̹� ������ ���⸦ �ٽ� ���� �� , ���� ������ �Ծ��� ��, ���� ������ �������� ��.
    public virtual void ResetWeapon(WeaponStats bonusStats)
    {
        //���̽� Stat + ���ʽ� Stat
        WeaponStats totalStats = baseStats;

        if(bonusStats != null)
        {
            totalStats.damage *= (100 + bonusStats.damage) / 100;
            totalStats.speed *= (100 + bonusStats.speed) / 100;
            totalStats.shootInterval *= 100 / (100 + bonusStats.shootInterval);

            totalStats.weaponShoot = bonusStats.weaponShoot;
            totalStats.weaponImpact = bonusStats.weaponImpact;
        }

        //���� �����Ѵ�.
        weaponStats = totalStats;
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
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab,0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            float ranSpd = UnityEngine.Random.Range(weaponStats.speed - speedVariation, weaponStats.speed + speedVariation);
            proj.Init(weaponStats.damage, ranSpd, lifeTime, range);
            //�Ѿ˿� Impact�̺�Ʈ ���
            if(weaponStats.weaponImpact != null)
            {
                proj.weaponImpactDel = weaponStats.weaponImpact;
            }

        }

        //�߻� �� �̺�Ʈ ����
        WeaponShootEvent();
        //MuzzleFlash �߻�
        MuzzleFlashEvent(pos, targetRotation);
        //�� �ð� üũ
        lastShootTime = Time.time;
        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);

    }

    protected IEnumerator burstShootRoutine(Vector2 pos, Vector3 dir, int repeatNumber, float interval)
    {
        for (int i = 0; i < repeatNumber; i++)
        {
            Shoot(pos, dir);
            yield return new WaitForSeconds(interval);
        }
    }

    protected void WeaponShootEvent()
    {
        if(weaponStats.weaponShoot != null)
        {
            weaponStats.weaponShoot();
        }
    }

    protected void ShowHitEffect(ParticleSystem particle, Vector2 pos)
    {
        if (particle != null)
            GameManager.Instance.particleManager.GetParticle(particle, pos, transform.rotation);
    }

    //�ѱ� �Һ� VFX 
    protected void MuzzleFlashEvent(Vector3 pos, Quaternion quat)
    {
        if(muzzleFlashVFX == null) { return; }
        GameManager.Instance.particleManager.GetParticle(muzzleFlashVFX, pos, quat);
    }

}

//�ڼտ� �����Ͽ� ���ť��� ��ȭ��ų ������ ���ݵ�
[System.Serializable]
public class WeaponStats
{
    public float damage;
    public float speed;
    public float shootInterval;

    public WeaponShootDelegate weaponShoot;
    public WeaponImpactDelegate weaponImpact;
}

public enum BarrelType { PingPong, InOrder }
