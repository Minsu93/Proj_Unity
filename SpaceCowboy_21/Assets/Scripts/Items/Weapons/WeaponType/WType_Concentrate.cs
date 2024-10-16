using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_Concentrate : WeaponType
{
    [SerializeField] float concentrateDist = 5.0f;
    [SerializeField] float concentrateAngle = 15.0f;


    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        Vector2 upVec = Quaternion.Euler(0, 0, 90) * dir;
        //Vector2 upDist = (upVec * GetHeight(concentrateDist, concentrateAngle));
        Vector2 upDist = (upVec * concentrateDist);

        //Shoot(pos + upDist, Quaternion.Euler(0,0, concentrateAngle) * dir);
        //Shoot(pos - upDist, Quaternion.Euler(0, 0, -concentrateAngle) * dir);
        Shoot(pos , dir, projectilePrefab);
        Shoot(pos , dir, secondProjectilePrefab);

        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }

    //public float GetHeight(float dist, float angle)
    //{
    //    // angle�� �� �������, �������� ��ȯ�ؾ� �մϴ�.
    //    float radians = Mathf.Deg2Rad * angle;
    //    return dist * Mathf.Sin(radians);
    //}

}
