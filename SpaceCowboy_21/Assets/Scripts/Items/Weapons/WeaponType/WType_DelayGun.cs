using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_DelayGun : WeaponType
{
    public event System.Action ShootButtonUpEvent;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        DelayShoot(pos, dir);


        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        if(ShootButtonUpEvent != null)
        {
            ShootButtonUpEvent();
            ShootButtonUpEvent= null;   
        }
    }

    protected void DelayShoot(Vector2 pos, Vector3 dir)
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            if(projectile.TryGetComponent(out Projectile_Delay p_delay))
            {
                float ranSpd = UnityEngine.Random.Range(speed - speedVariation, speed + speedVariation);
                p_delay.Init(damage, ranSpd, lifeTime, range);
                //�߻� �̺�Ʈ�� ���
                ShootButtonUpEvent += p_delay.DelayMovement;
                //�Ѿ˿� Impact�̺�Ʈ ���
                if (weaponImpact != null)
                {
                    p_delay.weaponImpactDel = weaponImpact;
                }

            }


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
