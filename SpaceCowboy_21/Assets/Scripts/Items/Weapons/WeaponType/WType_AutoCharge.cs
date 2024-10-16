using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoCharge : WeaponType
{
    [Header("ChargeGun"), Range(2f,5f)]
    [SerializeField] private float maxCharge = 3f;   //x�� ������. 
    private float curCharge = 1;
    [SerializeField] private float chargeSpeed;
    [SerializeField] ParticleSystem chargeParticle;
    bool chargeOn;


    public bool shootOnce { get; set; }  //�� ��� ����


    //Charging ����
    private void Update()
    {
        if(curCharge < maxCharge)
        {
            curCharge += chargeSpeed * Time.deltaTime;

            if (!chargeOn)
            {
                chargeOn = true;
                chargeParticle.Play();
            }
                
        }
        else
        {
            curCharge = maxCharge;

            if (chargeOn)
                chargeOn = false;
                chargeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        if (shootOnce) return;
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        else
        {
            shootOnce = true;

            //í¡ �߻�
            ChargeShoot(pos, dir, curCharge);

            //í¡ �ʱ�ȭ
            curCharge = 1;

            //PlayerWeapon���� ��ó��
            AfterShootProcess();
        }
    }

    //Charge Shoot
    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        shootOnce = false;
    }


    protected virtual void ChargeShoot(Vector2 pos, Vector3 dir, float power)
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

            //(����) ������ �Ѿ� �߻�
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;

            Proj_Charged projCharged = projectile.GetComponent<Proj_Charged>();
            projCharged.Init(damage, speed, lifeTime, range);
            projCharged.InitCharge(power);

        }

        lastShootTime = Time.time;
        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }
}
