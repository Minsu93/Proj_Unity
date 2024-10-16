using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_Charge : WeaponType
{
    [Header("ChargeGun")]   
    [SerializeField] private float maxCharge;   //x배 데미지. 
    private float curCharge = 1;
    [SerializeField] private float chargeSpeed;
    [SerializeField] ParticleSystem chargeParticle;



    //Charging 시작
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        if (Time.time - lastShootTime < shootInterval) return;

        if (curCharge <= maxCharge)
        {
            curCharge += chargeSpeed * Time.deltaTime;
        }
        else
        {
            curCharge = maxCharge;
        }

        if (!chargeParticle.isPlaying)
            chargeParticle.Play();


    }

    //Charge Shoot
    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval) return;

        //챠징 발사
        ChargeShoot(pos, dir, curCharge);

        //챠징 초기화
        curCharge = 1;

        //PlayerWeapon에서 후처리
        AfterShootProcess();

        if(chargeParticle.isPlaying)
            chargeParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
    }


    protected virtual void ChargeShoot(Vector2 pos, Vector3 dir, float power)
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
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab,0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Proj_Charged projCharged = projectile.GetComponent<Proj_Charged>();
            projCharged.Init(damage, speed, lifeTime, range);
            projCharged.InitCharge(power);

        }

        lastShootTime = Time.time;
        //사운드 생성
        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }

}
