using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoParallel : WeaponType
{
    [SerializeField] float pararrelDist = 1.0f;


    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        int barrelCount = numberOfProjectile;
        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval) return;

        //총구 여럿 두기 
        float totalDist = pararrelDist * (barrelCount - 1) / 2.0f;
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * dir;
        Vector2 upDist = pos + (upVec * totalDist);

        for (int i = 0; i < barrelCount;  i++)
        {
            SingleShoot(upDist - (pararrelDist * i * upVec), dir, projectilePrefab);
        }


        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }
}
