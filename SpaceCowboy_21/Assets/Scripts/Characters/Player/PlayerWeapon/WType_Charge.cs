using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_Charge : WeaponType
{
    
    [SerializeField] private float maxCharge;   //x�� ������. 
    private float curCharge = 1;
    [SerializeField] private float chargeSpeed;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        if (curCharge <= maxCharge)
        {
            curCharge += chargeSpeed * Time.deltaTime;
        }
        else
        {
            curCharge = maxCharge;
        }
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        //í¡ �߻�
        ChargeShoot(pos, dir, curCharge);

        //í¡ �ʱ�ȭ
        curCharge = 1;

        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }


    protected virtual void ChargeShoot(Vector2 pos, Vector3 dir, float power)
    {
        float totalSpread = weaponStats.projectileSpread * (weaponStats.numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-weaponStats.randomSpreadAngle * 0.5f, weaponStats.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < weaponStats.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, weaponStats.projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(weaponStats.damage * power, weaponStats.speed, weaponStats.lifeTime, weaponStats.range, weaponStats.projPenetration, weaponStats.projReflection, weaponStats.projGuide);

        }

        lastShootTime = Time.time;
        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }

}
