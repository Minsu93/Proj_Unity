using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoSplit : WType_Auto
{
    protected override void Shoot(Vector2 pos, Vector3 dir, GameObject projectilePrefab)
    {
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //총알 생성
        GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
        projectile.transform.position = pos;
        projectile.transform.rotation = targetRotation * randomRotation;
        Projectile_LifeOverSplit proj = projectile.GetComponent<Projectile_LifeOverSplit>();
        float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
        proj.Init(damage, ranSpd, lifeTime, range, numberOfProjectile, secondProjectilePrefab);

        //총알에 Impact이벤트 등록
        if (weaponImpact != null)
        {
            proj.weaponImpactDel = weaponImpact;
        }

        //발사 시 이벤트 실행
        WeaponShootEvent();
        //MuzzleFlash 발사
        MuzzleFlashEvent(pos, targetRotation);
        //쏜 시간 체크
        lastShootTime = Time.time;
        //사운드 생성
        GameManager.Instance.audioManager.PlaySfx(shootSFX);

    }

}
