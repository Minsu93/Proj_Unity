using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoSplit : WType_Auto
{
    protected override void Shoot(Vector2 pos, Vector3 dir, GameObject projectilePrefab)
    {
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //�Ѿ� ����
        GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
        projectile.transform.position = pos;
        projectile.transform.rotation = targetRotation * randomRotation;
        Projectile_LifeOverSplit proj = projectile.GetComponent<Projectile_LifeOverSplit>();
        float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
        proj.Init(damage, ranSpd, lifeTime, range, numberOfProjectile, secondProjectilePrefab);

        //�Ѿ˿� Impact�̺�Ʈ ���
        if (weaponImpact != null)
        {
            proj.weaponImpactDel = weaponImpact;
        }

        //�߻� �� �̺�Ʈ ����
        WeaponShootEvent();
        //MuzzleFlash �߻�
        MuzzleFlashEvent(pos, targetRotation);
        //�� �ð� üũ
        lastShootTime = Time.time;
        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);

    }

}
