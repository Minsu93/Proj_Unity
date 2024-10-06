using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoBurst : WeaponType
{
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //총 발사 주기
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        //Shoot(pos, dir);
        StartCoroutine(burstShootRoutine(pos, dir, numberOfBurst, burstInterval));
        //ArrowShapeShootRoutine(pos, dir);


        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }



    //void ArrowShapeShootRoutine(Vector2 pos, Vector2 dir)
    //{
    //    PositionShoot(pos, dir, 4, 0);
    //    PositionShoot(pos, dir, 3, 0.5f);
    //    PositionShoot(pos, dir, 3, -0.5f);
    //    PositionShoot(pos, dir, 2, 1f);
    //    PositionShoot(pos, dir, 2, 0);
    //    PositionShoot(pos, dir, 2, -1f);
    //    PositionShoot(pos, dir, 1, 0);
    //    PositionShoot(pos, dir, 0, 0);
    //}

    //void PositionShoot(Vector2 pos, Vector2 dir, float xPos, float yPos)
    //{
    //    Vector2 upVec = new Vector2(dir.y, -dir.x);
    //    float xDist = 0.8f;
    //    float yDist = 0.5f;

    //    Shoot(pos + xPos * xDist * dir + yPos * yDist * upVec, dir);
    //}
}
