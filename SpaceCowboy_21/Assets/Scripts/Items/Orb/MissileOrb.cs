using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileOrb : Orb
{
    /// <summary>
    /// 미사일 오브 : 주변에 있는 적에게 미사일을 3발 발사한다. 
    /// 적의 수가 적으면 같은 타겟을 여러 번 타격한다. 
    /// 적이 아예 없으면 랜덤한 장소를 타격한다. 
    /// </summary>
    /// 

    //감지 범위
    public float detectRadius = 5f;
    //발사하는 projectile
    public GameObject projectilePrefab;
    //발사 반복 횟수
    public int missileCount = 3;
    [SerializeField] private float missileInterval = 0.3f;

    //데미지 및 속도
    public float damage, speed, lifeTime;


    protected override void WhenDieEvent()
    {
        if (CheckTarget(out RaycastHit2D[] hits))
        {
            StartCoroutine(AimShoot(hits));
        }
        else
            StartCoroutine(RandomShoot(missileCount));
    }


    bool CheckTarget(out RaycastHit2D[] hits)
    {
        //주변에 적이 있는지 체크        
        hits = Physics2D.CircleCastAll(transform.position, detectRadius, Vector2.right, 0f, LayerMask.GetMask("Enemy"));

        if (hits.Length > 0)
        {
            return true;
        }
        else return false;
        
    }

    IEnumerator AimShoot(RaycastHit2D[] targets)
    {
        for (int i = 0; i < missileCount; i++)
        {
            int targetNum = i % targets.Length;
            Vector2 targetVec = (targets[targetNum].transform.position - transform.position).normalized;
            Vector2 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);

            //총알 생성
            Shoot(transform.position, targetRot);
            yield return new WaitForSeconds(missileInterval);
        }
    }

    IEnumerator RandomShoot(int num)
    {
        //랜덤 방향으로 총알 발사
        for (int i = 0; i < num; i++)
        {
            Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(-180, 180f));

            //총알 생성
            Shoot(transform.position, randomRot);
            yield return new WaitForSeconds(missileInterval);
        }
    }
    void Shoot(Vector3 pos, Quaternion rot)
    {
        //총알 생성
        GameObject projectile = PoolManager.instance.Get(projectilePrefab);
        projectile.transform.position = pos;
        projectile.transform.rotation = rot;
        projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);
    }
}
