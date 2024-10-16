using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneItem_Projectile : DroneItem
{
    [Header("Enemy Check Range")]
    [SerializeField] protected float enemyCheckRange = 10f;
    [SerializeField] protected float checkInterval = 0.2f;

    [Header("Projectile Property")]
    [SerializeField] protected ProjectileAttackProperty attackProperty;
    protected float lastShootTime;
    protected Transform targetTr;
    float shootTimer;


    protected override void Update()
    {
        base.Update();

        if (!activate) return;
        if (stopFollow) return;
        if (useDrone) return;
        //기본 사격
        ShootMethod();
    }


    protected void ShootMethod()
    {
        //시간 체크
        shootTimer += Time.deltaTime;
        if (shootTimer >= checkInterval)
        {
            //적 체크
            shootTimer = 0;
            targetTr = CheckEnemyIsNear();
        }


        //발사 
        if (targetTr != null)
        {
            if (Time.time - lastShootTime > attackProperty.shootCoolTime)
            {
                //쏜 시간 체크
                lastShootTime = Time.time;

                //사격
                StartCoroutine(burstShootRoutine(targetTr));
            }
        }
    }

    protected Transform CheckEnemyIsNear()
    {
        Transform targetTr = null;
        float minDist = float.MaxValue;
        Collider2D[] enemyColls = new Collider2D[10];
        int num = Physics2D.OverlapCircleNonAlloc(transform.position, enemyCheckRange, enemyColls, LayerMask.GetMask("Enemy"));
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                Vector2 dir = (enemyColls[i].transform.position - transform.position).normalized;
                float dist = Vector2.Distance(transform.position, enemyColls[i].transform.position);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, LayerMask.GetMask("Planet"));
                if (hit.collider != null) continue;

                if (dist < minDist)
                {
                    minDist = dist;
                    targetTr = enemyColls[i].transform;
                }
            }
        }

        return targetTr;
    }


    protected IEnumerator burstShootRoutine(Transform targetTr)
    {
        for (int i = 0; i < attackProperty.numberOfBurst; i++)
        {
            Vector2 pos = transform.position;
            Vector2 dir = ((Vector2)targetTr.position - pos).normalized;
            Shoot(pos, dir);
            yield return new WaitForSeconds(attackProperty.burstInterval);
        }
    }


    protected virtual void Shoot(Vector2 pos, Vector3 dir)
    {
        float totalSpread = attackProperty.projectileSpread * (attackProperty.numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-attackProperty.randomSpreadAngle * 0.5f, attackProperty.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < attackProperty.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, attackProperty.projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(attackProperty.projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(attackProperty.damage, attackProperty.speed, attackProperty.lifeTime, attackProperty.range);
        }
    }

}

[Serializable]
public struct ProjectileAttackProperty
{
    public int numberOfBurst;
    public float burstInterval;
    public int numberOfProjectile;
    public float projectileSpread;
    public float randomSpreadAngle;
    public GameObject projectilePrefab;
    public float damage;
    public float speed;
    public float lifeTime;
    public float range;
    public float shootCoolTime;
}

