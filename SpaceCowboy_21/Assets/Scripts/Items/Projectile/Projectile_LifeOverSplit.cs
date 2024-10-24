using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_LifeOverSplit : Projectile_Player
{
    GameObject splitPrefab;
    int numberOfSplit;
    [SerializeField] float projectileSpread = 15.0f;
    [SerializeField] float splitDamage, splitSpeed, splitProjDistance;

    public void Init(float damage, float speed, float lifeTime, float distance, int splitCount, GameObject splitPrefab)
    {
        base.Init(damage, speed, lifeTime, distance);
        numberOfSplit = splitCount;
        this.splitPrefab = splitPrefab;
    }

    protected override void LifeOver()
    {
        float totalSpread = projectileSpread * (numberOfSplit - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Quaternion rot = transform.rotation;     
        Quaternion targetRotation = rot * Quaternion.Euler(0, 0, - totalSpread / 2);

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
