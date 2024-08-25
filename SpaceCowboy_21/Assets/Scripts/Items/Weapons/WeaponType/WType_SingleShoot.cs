using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_SingleShoot : WeaponType
{
    public bool shootOnce { get; set; }  //�� ��� ����

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        if (shootOnce) return;
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        else
        {
            shootOnce = true;

            StartCoroutine(burstShootRoutine(pos, dir, numberOfBurst, burstInterval));
            
            //PlayerWeapon���� ��ó��
            AfterShootProcess();
        }

    }



    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        shootOnce = false;
    }

    public override void ResetWeapon(WeaponStats bonusStats)
    {
        base.ResetWeapon(bonusStats);

        shootOnce = false;
    }
}
