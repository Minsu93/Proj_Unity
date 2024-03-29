using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_BoomMushroom : EnemyBrain
{
    public float projectileSpread = 30f;
    public int numberOfProjectiles = 5;
    public GameObject projectilePrefab;
    public float damage, speed, range, lifeTime;

    public override void DetectSiutation()
    {
        return;
    }

    public override void DamageEvent(float dmg)
    {
        if (enemyState == EnemyState.Die)
            return;

        //데미지를 적용
        if (health.AnyDamage(dmg))
        {
            //맞는 효과 
            //action.HitView();

            if (health.IsDead())
            {
                //죽은 경우 
                enemyState = EnemyState.Die;

                GenerateBullets();
                gameObject.SetActive(false);
            }
        }
    }

    void GenerateBullets()
    {
        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 dir = transform.up;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //멀티샷
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //총알 생성
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectilePrefab);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = tempRot;
            projectile.GetComponent<Projectile>().init(damage, speed, range, lifeTime);
        }

        //AudioManager.instance.PlaySfx(shootSFX);
    }
}
