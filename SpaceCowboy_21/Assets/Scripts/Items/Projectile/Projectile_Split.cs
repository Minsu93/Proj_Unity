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
        float totalSpread = projectileSpread * (numberOfSplit - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Quaternion rot = transform.rotation;
        //Vector3 dir = rot * transform.right;
        //Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        //Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        
        Quaternion targetRotation = rot * Quaternion.Euler(0, 0, - totalSpread / 2);

        ////���� ���� �߰�
        //float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        //Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < numberOfSplit; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(splitPrefab, 0);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = tempRot;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(damage, speed, lifeTime, splitProjDistance);
            
            //�Ѿ˿� Impact�̺�Ʈ ���
            if (weaponImpactDel != null)
            {
                proj.weaponImpactDel = weaponImpactDel;
            }
        }

        AfterHitEvent();

    }

}
