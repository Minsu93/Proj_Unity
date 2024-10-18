using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_Auto : WeaponType
{
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval) return;

        Shoot(pos, dir, projectilePrefab);


        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }
}
