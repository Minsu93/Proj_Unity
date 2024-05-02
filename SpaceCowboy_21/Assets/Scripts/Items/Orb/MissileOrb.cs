using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileOrb : Orb
{
    /// <summary>
    /// �̻��� ���� : �ֺ��� �ִ� ������ �̻����� 3�� �߻��Ѵ�. 
    /// ���� ���� ������ ���� Ÿ���� ���� �� Ÿ���Ѵ�. 
    /// ���� �ƿ� ������ ������ ��Ҹ� Ÿ���Ѵ�. 
    /// </summary>
    /// 

    //���� ����
    public float detectRadius = 5f;
    //�߻��ϴ� projectile
    public GameObject projectilePrefab;
    //�߻� �ݺ� Ƚ��
    public int missileCount = 3;
    [SerializeField] private float missileInterval = 0.3f;

    //������ �� �ӵ�
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
        //�ֺ��� ���� �ִ��� üũ        
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

            //�Ѿ� ����
            Shoot(transform.position, targetRot);
            yield return new WaitForSeconds(missileInterval);
        }
    }

    IEnumerator RandomShoot(int num)
    {
        //���� �������� �Ѿ� �߻�
        for (int i = 0; i < num; i++)
        {
            Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(-180, 180f));

            //�Ѿ� ����
            Shoot(transform.position, randomRot);
            yield return new WaitForSeconds(missileInterval);
        }
    }
    void Shoot(Vector3 pos, Quaternion rot)
    {
        //�Ѿ� ����
        GameObject projectile = PoolManager.instance.Get(projectilePrefab);
        projectile.transform.position = pos;
        projectile.transform.rotation = rot;
        projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);
    }
}
