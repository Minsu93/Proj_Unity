using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_BoomMushroom : MonoBehaviour
{
    public float projectileSpread = 30f;
    public int numberOfProjectiles = 5;
    public GameObject projectilePrefab;
    public float damage, speed, range, lifeTime;

    //public override void BrainStateChange()
    //{
    //    return;
    //}

    //public override void WhenDieEvent()
    //{
    //    GenerateBullets();
    //}

    //void GenerateBullets()
    //{
    //    float totalSpread = projectileSpread * (numberOfProjectiles - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

    //    Vector3 dir = transform.up;

    //    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
    //    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

    //    //��Ƽ��
    //    for (int i = 0; i < numberOfProjectiles; i++)
    //    {
    //        Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

    //        //�Ѿ� ����
    //        GameObject projectile = GameManager.Instance.poolManager.GetEnemyProj(projectilePrefab);
    //        projectile.transform.position = transform.position;
    //        projectile.transform.rotation = tempRot;
    //        projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);
    //    }

    //    //AudioManager.instance.PlaySfx(shootSFX);
    //}
}
