using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_DelayGun : WeaponType
{
    public event System.Action ShootButtonUpEvent;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval) return;

        DelayShoot(pos, dir);


        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        if(ShootButtonUpEvent != null)
        {
            ShootButtonUpEvent();
            ShootButtonUpEvent= null;   
        }
    }

    protected void DelayShoot(Vector2 pos, Vector3 dir)
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
            if(projectile.TryGetComponent(out Projectile_Delay p_delay))
            {
                float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
                p_delay.Init(damage, ranSpd, lifeTime, range);
                //발사 이벤트에 등록
                ShootButtonUpEvent += p_delay.DelayMovement;
                //총알에 Impact이벤트 등록
                if (weaponImpact != null)
                {
                    p_delay.weaponImpactDel = weaponImpact;
                }

            }


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
