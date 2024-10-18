using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_ImpactGun : WeaponType
{
    [SerializeField] int spawnCount = 2;
    [SerializeField] float gap = 180f;
    [SerializeField] float startDelay = 0.2f;
    [SerializeField] float secondSpeed = 10f;
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval) return;

        Shoot(pos, dir, projectilePrefab);


        //PlayerWeapon에서 후처리
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
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile_Player proj = projectile.GetComponent<Projectile_Player>();
            //float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
            proj.Init(damage, secondSpeed, lifeTime, range, startDelay);
        }

    }
}
