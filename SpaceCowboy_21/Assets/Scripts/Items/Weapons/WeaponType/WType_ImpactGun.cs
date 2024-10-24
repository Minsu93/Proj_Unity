using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_ImpactGun : WeaponType
{
    int spawnCount;
    float gap = 180f;
    [SerializeField] float startDelay = 0.2f;
    [SerializeField] float secondSpeed = 10f;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        SingleShoot(pos, dir, projectilePrefab);


        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }

    public override void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        base.Initialize(weaponData, gunTipLocalPos);

        weaponImpact = ImpactGenerate;
    }

    public override void InitializeByTier()
    {
        base.InitializeByTier();
        gap = projectileSpread * numberOfProjectile;
        spawnCount = numberOfProjectile;

    }

    void ImpactGenerate(Transform targetTr)
    {
        Vector2 pos = targetTr.position;
        float baseAngle;
        if (spawnCount > 1)
            baseAngle = gap / (spawnCount - 1);
        else
            baseAngle = gap * 0.5f;

        for(int i = 0; i < spawnCount; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, -180 - (gap * 0.5f) + (baseAngle * i)) * targetTr.right;
            SecondShoot(pos, dir, secondProjectilePrefab, startDelay);
        }
    }

    

    protected void SecondShoot(Vector2 pos, Vector3 dir, GameObject projectilePrefab, float startDelay)
    {
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //�Ѿ� ����
        GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
        projectile.transform.position = pos;
        projectile.transform.rotation = targetRotation * randomRotation;
        Projectile_Player proj = projectile.GetComponent<Projectile_Player>();
        proj.Init(damage, secondSpeed, lifeTime, range, startDelay);

    }
}