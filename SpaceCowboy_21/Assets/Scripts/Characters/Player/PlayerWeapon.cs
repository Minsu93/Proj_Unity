using SpaceCowboy;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public WeaponData[] weapons;
    int currentWeaponIndex = -1;
    int temporAmmo;    //�⺻ �Ѿ� �����ִ°�
    public float defaultReloadTime = 0.5f;

    public float chargeAmount = 0.1f;
    public float gunPowerMax = 15f;   //POWER �ִ� ��ġ
    public float curGunPower;   //���� �����ִ� gunPower��ġ


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

    public int currAmmo { get; private set; }     //���� ���� �Ѿ�
    int reflectionCount;    //�ݻ� ȸ��

    bool isSingleShot;      //�ܹ߽��ΰ���?
    float lastShootTime;    //���� �߻� �ð�


    //���ε� ����
    bool isReloading;

    //���� ���⸦ ������ΰ���?
    bool inArtifact;
    public float artifactLifeTime;

    //���⸦ �ٲٴ� ���ΰ���?
    bool isChanging; 

    public Transform[] gunTips; 
    Transform gunTip;
    public PlayerBehavior playerBehavior;
    Coroutine shootRoutine;
    Coroutine reloadRoutine;
    Coroutine changeRoutine;

    private void Start()
    {
        //�⺻ �� �ʱ�ȭ.

        if (currentWeaponIndex < 0)
        {
            temporAmmo = weapons[0].MaxAmmo;
        } 

        ChangeWeapon(0);
    }

    #region Shoot Function


    public void TryStartShoot()
    {
        //�� ��� �̺�Ʈ�� �����Ѵ�. �ܹ����� ���� �� �߸� �߻�. ���� ���� ���� �߻� ��ƾ�� ����
        //�޸��� ���� ��쿡�� ���� ���� ���Ѵ�. 
        if (playerBehavior.runON)
            return;

        //�ܹ����� ��쿡�� shootevent�� �ѹ� �����Ѵ�. ���� �ȵǼ� �ٽ� ������ �ҿ����. 
        if (isSingleShot)
        {
            TryShoot();
        }

        //�������� ��쿡�� ��� ��� �̺�Ʈ�� �����Ѵ�. 
        else
        {
            shootRoutine = StartCoroutine(ShootRepeater());
        }

    }

    public void TryStopShoot()
    {
        //�� ��� �̺�Ʈ�� �ߴ��Ѵ�. �ܹ����� ���� �� ��� �ʱ�ȭ. ���� ���� ��쿡�� �߻� ����. 
        //���� ���� ��쿡�� ��� ��� �̺�Ʈ�� �ߴ��Ѵ�. 
        if (shootRoutine != null)
        {
            StopCoroutine(shootRoutine);
            shootRoutine = null;
        }
    }

    //���� �� �ڵ� �߻� ���
    public void AutoShoot()
    {
        shootRoutine = StartCoroutine(ShootRepeater());
    }

    IEnumerator ShootRepeater()
    {
        while (true)
        {
            //�Ѿ� ��Ÿ�Ӹ��� ��� �߻��Ѵ�. 
            TryShoot();
            yield return null;
        }
    }


    public void TryShoot()  //�̺�Ʈ �߻��� ���ؼ� > PlayerWeapon, PlayerView �� ShootEvent�� �߻�
    {
        PlayerState currState = playerBehavior.state;
        if (currState == PlayerState.Die) return;
        if (currState == PlayerState.Stun)
            return;

        //���ε� ���̸� �߻����� �ʴ´�.
        if (isReloading)
            return;

        //���⸦ �ٲٴ� ���̸� �߻����� �ʴ´�. 
        if(isChanging)
            return;
        

        //�Ѿ� �߻籸�� �༺ ���ο� �ִٸ� �߻����� �ʴ´�. 
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector2 aimDirection = (mousePos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 1f, LayerMask.GetMask("Planet"));
        if (hit.collider != null)
            return;



        float currentTime = Time.time;

        if (currentTime - lastShootTime > shootInterval)
        {
            //�Ѿ� �߻�
            if (currAmmo > 0)
            {
                //�� ������ �Ѿ� �ѹ߾� ����
                currAmmo -= 1;

                lastShootTime = currentTime;

                StartCoroutine(ShootRoutine());
                playerBehavior.TryShootEvent();
            }
            else
            {
                //�⺻ ���� ��� �Ѿ��� ������ ���ε� ����
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

        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        //Vector3 dir = gunTip.right; //�߻� ����
        Vector3 dir = playerBehavior.aimDirection;
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
                projectile.transform.position = transform.position + dir * 1f;
                projectile.transform.rotation = tempRot * randomRotation;
                projectile.GetComponent<Projectile_Player>().init(damage, lifeTime, speed, reflectionCount);


            }

            AudioManager.instance.PlaySfx(currentWeaponData.ShootSFX);

            if(burstInterval> 0)
                yield return new WaitForSeconds(burstInterval);


        }
        yield return null;

        //�Ѿ��� �� �� ���
        if (currAmmo <= 0)
        {
            //�⺻ ���� ��쿡�� 
            if (currentWeaponIndex == 0)
            {
                //�ڵ����� ���ε带 �Ѵ�
                TryReload();
            }
            //�� ���� ���� ��쿡��
            else
            {
                //�� �ٲٱ� Ÿ�� 
                //�⺻ ������ ���ư���
                inArtifact = false;
                ChangeWeapon(0);
            }
        }
    }

    #endregion

    #region Reload
    public void TryReload()
    {
        //������ ���� �ƴ� �� 
        if (isReloading)
            return;

        //�������� �����ҋ� (currAmmo �� maxAmmo���� ������)
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

        //���ε� �ִϸ��̼� ����
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

        // index �� 0�̶��(������) ������� ��ü�Ѵ�.
        if (index != 0) 
        {
            //��� ������ ���� ���� ��쿡�� ���⸦ ��ü�Ѵ�.
            cost = weapons[index].PowerCost;
            if (curGunPower < cost)
            {
                return;
            }
            else
            {
                //power�Ҹ� 
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

        //���ϴ� ������ �ٲ��ش�.
        InitializeWeapon(index);

        //�� �ٲٴ� �ִϸ��̼��� ���� 

        isChanging = true;
        //�� �ٲٴ� �� �ɸ��� �ð� 
        yield return null;

        isChanging = false;
        changeRoutine = null;


    }

    public void InitializeWeapon(int index)
    {
        //���� �������� ������̸� �������� �� ���� ������ ���� �ٲ��� ���Ѵ�. <<����
        if (inArtifact)
            return;

        //���� �߻����̸� �� ��⸦ ����
        TryStopShoot();

        //���ε� ���̸� ���ε带 ����ϰ� ����
        StopReload();

        //���� ���� �⺻ ���� ���, ���� �Ѿ��� temporAmmo�� �ִ´�
        if (currentWeaponIndex == 0)
        {
            temporAmmo = currAmmo;
        }


        currentWeaponIndex = index;
        currentWeaponData = weapons[currentWeaponIndex];

        //�� ���� �ѱ� ��ġ ����
        gunTip = gunTips[currentWeaponData.SkinIndex];

        //�� ��� ����
        playerBehavior.TryChangeWeapon(currentWeaponData);


        //�ʱ� ����(������ ��ũ���ͺ� ������Ʈ ��)
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

        //�������� Ÿ�̸� �ʱ�ȭ
        artifactLifeTime = currentWeaponData.ArtifaceLifetime;

        //�Ѿ��� �ʱ�ȭ
        maxAmmo = currentWeaponData.MaxAmmo;
        
        //�⺻ ���� ���
        if(index == 0)
        {
            inArtifact = false;
            currAmmo = temporAmmo;
        }
        //���� ���� ���
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
        //�������� Ÿ�̸�
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
