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

        //�������� ����
        if (health.AnyDamage(dmg))
        {
            //�´� ȿ�� 
            //action.HitView();

            if (health.IsDead())
            {
                //���� ��� 
                enemyState = EnemyState.Die;

                GenerateBullets();
                gameObject.SetActive(false);
            }
        }
    }

    void GenerateBullets()
    {
        float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 dir = transform.up;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //��Ƽ��
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectilePrefab);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = tempRot;
            projectile.GetComponent<Projectile>().init(damage, speed, range, lifeTime);
        }

        //AudioManager.instance.PlaySfx(shootSFX);
    }
}
