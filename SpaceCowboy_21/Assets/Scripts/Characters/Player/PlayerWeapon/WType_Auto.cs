using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_Auto : WeaponType
{
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        Shoot(pos, dir);


        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }
}