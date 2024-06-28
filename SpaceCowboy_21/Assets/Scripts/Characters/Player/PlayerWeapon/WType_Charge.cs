using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_Charge : WeaponType
{
    
    [SerializeField] private float maxCharge;   //x배 데미지. 
    private float curCharge = 1;
    [SerializeField] private float chargeSpeed;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        if (curCharge <= maxCharge)
        {
            curCharge += chargeSpeed * Time.deltaTime;
        }
        else
        {
            curCharge = maxCharge;
        }
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        //챠징 발사
        ChargeShoot(pos, dir, curCharge);

        //챠징 초기화
        curCharge = 1;

        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }


    protected virtual void ChargeShoot(Vector2 pos, Vector3 dir, float power)
    {
        float totalSpread = weaponStats.projectileSpread * (weaponStats.numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-weaponStats.randomSpreadAngle * 0.5f, weaponStats.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < weaponStats.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, weaponStats.projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(weaponStats.damage * power, weaponStats.speed, weaponStats.lifeTime, weaponStats.range, weaponStats.projPenetration, weaponStats.projReflection, weaponStats.projGuide);

        }

        lastShootTime = Time.time;
        //사운드 생성
        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }

}
