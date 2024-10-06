using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Split : Projectile_Player
{
    [SerializeField] GameObject splitPrefab;
    [SerializeField] int numberOfSplit = 3;
    [SerializeField] float projectileSpread = 15.0f;
    [SerializeField] float splitDamage, splitSpeed, splitProjDistance;
    protected override void LifeOver()
    {
        float totalSpread = projectileSpread * (numberOfSplit - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Quaternion rot = transform.rotation;
        //Vector3 dir = rot * transform.right;
        //Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        //Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        
        Quaternion targetRotation = rot * Quaternion.Euler(0, 0, - totalSpread / 2);

        ////랜덤 각도 추가
        //float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        //Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < numberOfSplit; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(splitPrefab, 0);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = tempRot;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(damage, speed, lifeTime, splitProjDistance);
            
            //총알에 Impact이벤트 등록
            if (weaponImpactDel != null)
            {
                proj.weaponImpactDel = weaponImpactDel;
            }
        }

        AfterHitEvent();

    }

}
