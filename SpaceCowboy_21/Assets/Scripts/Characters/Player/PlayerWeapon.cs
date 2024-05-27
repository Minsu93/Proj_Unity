using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData baseWeapon;   //�⺻ �ѱ�
    WeaponData curWeapon;
    //public GameObject artifactWeaponPrefab; //�������� ���� ���� ������. 

    bool infiniteBullets;    //�Ѿ��� ����
    int numberOfProjectile;    //�ѹ� ���� �� ��Ƽ�� ��
    float shootInterval;    //�߻� ��Ÿ��
    float projectileSpread; //�Ѿ� ���� ������ ����
    float randomSpreadAngle;    //�ѱ� ��鸲������ ����� ������
    GameObject projectilePrefab;    //�Ѿ��� ����
    float damage;
    float speed;
    float lifeTime;
    float range;
    int projPenetration;
    int projReflection;
    int projGuide;
    AudioClip shootSFX;     //�߻�� ȿ����

    public float maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ
    public float currAmmo { get; private set; }     //���� ���� �Ѿ�

    //���� �߰� ����
    float damagePlus;
    float fireRatePlus;
    float projSpeedPlus;
    float projRangePlus;
    int projNumberPlus;


    [SerializeField] float weaponChangeTime = 0.5f; //���� ��ü�ϴµ� �ɸ��� �ð�

    float lastShootTime;    //���� �߻� �ð�
    bool isChanging;    //���⸦ �ٲٴ� ���ΰ���?
    public bool shootON { get; set; }  //�� ��� ����

    PlayerBehavior playerBehavior;
    //ArtifactWeapon currArtifactWeapon; //���� �������� ���� ����

    //public event System.Action weaponShoot;
    //public event System.Action<Vector2> weaponImpact;

    //Test
    //[SerializeField] GameObject awp;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

    }

    private void Start()
    {
        //ChangeArtifact(awp);
        playerBehavior.TryChangeWeapon(baseWeapon);
    }


    private void Update()   
    {
        if (!playerBehavior.activate) return;

        if (!shootON) return;

        //���⸦ �ٲٴ� ���̸� �߻����� �ʴ´�. 
        if (isChanging) return;

        //���� źâ�� �ƴϸ�, �Ѿ��� ���� ��� �߻����� �ʴ´�. 
        if (!infiniteBullets && currAmmo < 1) return;

        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        TryShoot();
    }

    #region Shoot Function


    public void TryShoot() 
    {
        //�� ������ �Ѿ� �ѹ߾� ����
        if(!infiniteBullets) currAmmo -= 1;
        lastShootTime = Time.time;
        Shoot();

        ////���� �߻� ȿ�� �ߵ�
        //ArtifactShoot();

        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

        // ���� �⺻ ���� �ƴϰ�, �Ѿ��� �� �� ��� 
        if (curWeapon != baseWeapon && currAmmo <= 0)
        {
            playerBehavior.TryChangeWeapon(baseWeapon);
        }
    }

    void Shoot()
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        //Vector3 dir = gunTip.right; //�߻� ����
        Vector3 gunTipPos = playerBehavior.gunTipPos;
        Vector3 dir = playerBehavior.aimDirection;

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
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(damage, speed, lifeTime, range, projPenetration, projReflection, projGuide);
            
            ////���� ������ ���� �ѹ� ���� �ִ´�. 
            //proj.ProjImpactEvent -= ArtifactImpact;
            //proj.ProjImpactEvent += ArtifactImpact;
        }

        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }

    #endregion
   

    #region ChangeWeapon

    public void ChangeWeapon(WeaponData weaponData)
    {
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
        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
        curWeapon = weaponData;
        projectilePrefab = weaponData.ProjectilePrefab;
        numberOfProjectile = weaponData.NumberOfProjectile;  
        shootInterval = weaponData.ShootInterval;   
        damage = weaponData.Damage;
        speed = weaponData.Speed;
        lifeTime = weaponData.LifeTime;
        range = weaponData.Range;
        projPenetration = weaponData.ProjPenetration;
        projReflection = weaponData.ProjReflection;
        projGuide = weaponData.ProjGuide;

        projectileSpread = weaponData.ProjectileSpread;
        randomSpreadAngle = weaponData.RandomSpreadAngle;
        shootSFX = weaponData.ShootSFX;



        currAmmo = weaponData.MaxAmmo;
        maxAmmo = weaponData.MaxAmmo;

        if (maxAmmo == 0) infiniteBullets = true;
        else infiniteBullets = false;

    }

    //���� �ѱ⽺�� ����.
    //void GetFinalStat()
    //{
    //    damage = damage * ((100 + damagePlus)/100);
    //    shootInterval = shootInterval * (100 / (100 + fireRatePlus));
    //    speed = speed * (100 + projSpeedPlus) / 100;
    //    range = range * (100 + projRangePlus) / 100;
    //    numberOfProjectile = projNumberPlus + 1;
    //}

    #endregion

    #region ArtifactWeapon

    //public void ChangeArtifact(GameObject artifactWeaponPrefab)
    //{
    //    if (currArtifactWeapon != null)
    //    {
    //        currArtifactWeapon.RemoveArtifactWeapon();
    //        Destroy(currArtifactWeapon.gameObject);
    //    }
    //    //���� �ν��Ͻ�
    //    currArtifactWeapon = Instantiate(artifactWeaponPrefab, this.transform).GetComponent<ArtifactWeapon>();
    //    currArtifactWeapon.CreateArtifactWeapon(this);

    //    //���� �ѱ� ���� ����
    //    this.damagePlus = currArtifactWeapon.damagePlus;
    //    this.fireRatePlus = currArtifactWeapon.fireRatePlus;
    //    this.projSpeedPlus = currArtifactWeapon.projSpeedPlus;
    //    this.projRangePlus = currArtifactWeapon.projRangePlus;
    //    this.projNumberPlus = currArtifactWeapon.projNumberPlus;
    //    this.projPenetrationPlus = currArtifactWeapon.projPenetrationPlus;
    //    this.projReflectionPlus = currArtifactWeapon.projReflectionPlus;
    //    this.projGuidePlus = currArtifactWeapon.projGuidePlus;

    //    //���� �⺻ �ѱ� ����
    //    baseWeapon = currArtifactWeapon.changedWeapon;
    //    //�ѱ� ��ü
    //    playerBehavior.TryChangeWeapon(baseWeapon);


    //}

    

    //void ArtifactShoot()
    //{
    //    if (weaponShoot != null) weaponShoot();
    //}

    //public void ArtifactImpact(Vector2 pos)
    //{
    //    if(weaponImpact!= null) weaponImpact(pos); 
    //}
    #endregion

    //#region SkillShot
    //public void AddSkillAmmo(int amount)
    //{
    //    currSkillAmmo += amount;
    //    if (currSkillAmmo > maxSkillAmmo)
    //    {
    //        currSkillAmmo = maxSkillAmmo;
    //    }
    //}

    ////��ų�� ����Ѵ�
    //public void UseSkillShot(Vector3 pos, Vector2 dir)
    //{
    //    if (currSkillAmmo <= 0) return;
    //    currSkillAmmo -= 1;

    //    skillArtifact.SkillOperation(pos, dir);
    //}

    //#endregion


}
