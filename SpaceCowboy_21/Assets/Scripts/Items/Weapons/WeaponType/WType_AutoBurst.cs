using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoBurst : WeaponType
{
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        //Shoot(pos, dir);
        StartCoroutine(burstShootRoutine(pos, dir, numberOfBurst, burstInterval));


        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }

}
