using SpaceCowboy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData baseWeapon;   //�⺻ �ѱ�
    WeaponData curWeapon;
    //public GameObject artifactWeaponPrefab; //�������� ���� ���� ������. 

    FireMode fireMode;
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
    float gunRecoil;

    public float maxAmmo { get; private set; }    //�Ѿ� źâ�� max��ġ
    public float currAmmo { get; private set; }     //���� ���� �Ѿ�

    //���� �߰� ����
    float damagePlus;
    float fireRatePlus;
    float projSpeedPlus;
    float projRangePlus;
    int projNumberPlus;


    [SerializeField] float weaponChangeTime = 0.5f; //���� ��ü�ϴµ� �ɸ��� �ð�

    public float lastShootTime { get; set; }    //���� �߻� �ð�
    bool isChanging;    //���⸦ �ٲٴ� ���ΰ���?
    public bool shootOn { get; set; }   //�ѽ�� ���ΰ���? 
    public bool shootOnce { get; set; }  //�� ��� ����
    bool showWeaponRange { get; set; }  //�ѱ� �����Ÿ� ���̰� ���� ����.
    GameObject weaponRangeSight;     //�����Ÿ� ������
    GameObject weaponRangePoint;    //������ �� spritePoint
    bool pointOn;
    int targetLayer;
    [SerializeField] LineRenderer sightLineRenderer;
    

    [SerializeField] private float maxCharge;
    private float curCharge;
    [SerializeField] private float chargeSpeed;

    PlayerBehavior playerBehavior;
    
    //ArtifactWeapon currArtifactWeapon; //���� �������� ���� ����

    //public event System.Action weaponShoot;
    //public event System.Action<Vector2> weaponImpact;

    //Test
    //[SerializeField] GameObject awp;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        CreateWeaponRange();
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy");
    }

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

    private void FixedUpdate()
    {
        if (showWeaponRange)
        {
            //weaponRangeSight.transform.position = ((Vector3)playerBehavior.aimDirection * range) + playerBehavior.gunTipPos;
            //weaponRangeSight.transform.rotation = Quaternion.LookRotation(Vector3.forward,  Quaternion.Euler(0, 0, 90) * playerBehavior.aimDirection);

            float targetDist;
            RaycastHit2D hit = Physics2D.Raycast(playerBehavior.gunTipPos, playerBehavior.aimDirection, range, targetLayer);
            if(hit.collider != null)
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
                }
                weaponRangePoint.transform.position = pos3;
            }
            else
            {
                if (weaponRangePoint.activeSelf)
                {
                    weaponRangePoint.SetActive(false);
                }
            }
            
        }
    }



    #region Shoot Function

    public void ShootProcess() 
    {
        //���⸦ �ٲٴ� ���̸� �߻����� �ʴ´�. 
        if (isChanging) return;

        //���� źâ�� �ƴϸ�, �Ѿ��� ���� ��� �߻����� �ʴ´�. 
        if (!infiniteBullets && currAmmo < 1) return;

        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        if (!shootOn) shootOn = true;

        switch (fireMode)
        {
            case FireMode.Auto:
                Shoot();
                break;

            case FireMode.Charge:
                //���� �� ����
                if(curCharge <= maxCharge)
                {
                    curCharge += chargeSpeed * Time.deltaTime;
                }
                else
                {
                    curCharge = maxCharge;
                }
                

                return;

            case FireMode.SingleShot:
                if (shootOnce) return;
                else
                {
                    shootOnce = true;
                    Shoot();
                }
                break;


        }

        //�� ������ �Ѿ� �ѹ߾� ����
        if (!infiniteBullets) currAmmo -= 1;
        lastShootTime = Time.time;
        GameManager.Instance.playerManager.PlayerShootTime(lastShootTime);  //UI �� ����

        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

        // ���� �⺻ ���� �ƴϰ�, �Ѿ��� �� �� ��� 
        if (curWeapon != baseWeapon && currAmmo <= 0)
        {
            GameManager.Instance.playerManager.ResetPlayerWeapon();     //����ϴ� ���� ���
            playerBehavior.TryChangeWeapon(baseWeapon);
        }
    }

    //�ѽ�� �ʱ�ȭ
    public void ShootOffProcess()
    {
        //if (!shootOn) return;
        //else
        //{
        //    shootOn = false;
        //}

        switch (fireMode)
        {
            case FireMode.Charge:
                //í¡ �߻�
                ChargeShoot(curCharge);

                //í¡ �ʱ�ȭ
                curCharge = 0;

                //�� ������ �Ѿ� �ѹ߾� ����
                if (!infiniteBullets) currAmmo -= 1;
                lastShootTime = Time.time;
                GameManager.Instance.playerManager.PlayerShootTime(lastShootTime);  //UI �� ����


                //PlayerView �� �ִϸ��̼� ���� 
                playerBehavior.TryShootEvent();

                // ���� �⺻ ���� �ƴϰ�, �Ѿ��� �� �� ��� 
                if (curWeapon != baseWeapon && currAmmo <= 0)
                {
                    GameManager.Instance.playerManager.ResetPlayerWeapon();     //����ϴ� ���� ���
                    playerBehavior.TryChangeWeapon(baseWeapon);
                }
                break;

            case FireMode.SingleShot:
                shootOnce = false;
                break;

        }
    }

    void Shoot()
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

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
        }

        //�ѱ�ݵ�
        //playerBehavior.GunRecoil(3f, -dir);

        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }


    void ChargeShoot(float curCharge)
    {
        Debug.Log("���� �� �߻�!");
    }
    #endregion
   

    #region ChangeWeapon

    public void ChangeWeapon(WeaponData weaponData)
    {
        isChanging = true;
        lastShootTime = 0f;
        shootOn = false;
        shootOnce = false;
        curCharge = 0f;

        //���ϴ� ������ �ٲ��ش�.
        InitializeWeapon(weaponData);

        //�÷��̾� �Ŵ����� ���� UI�� �ٲ� ��� ���� ������ �����Ѵ�. 
        GameManager.Instance.playerManager.PlayerChangeWeapon(weaponData);

        StartCoroutine(ChangeWeaponRoutine());

    }

    IEnumerator ChangeWeaponRoutine()
    {
        //�� �ٲٴ� �� �ɸ��� �ð� 
        yield return new WaitForSeconds(weaponChangeTime);

        isChanging = false;
    }

    public void InitializeWeapon(WeaponData weaponData)
    {
        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
        curWeapon = weaponData;
        fireMode = weaponData.FireMode;
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
        gunRecoil = weaponData.Recoil;

        currAmmo = weaponData.MaxAmmo;
        maxAmmo = weaponData.MaxAmmo;

        ShowWeaponSight(weaponData.ShowRange);

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
