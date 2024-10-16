using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_AutoParallel : WeaponType
{
    [SerializeField] int barrelCount = 3;
    [SerializeField] float concentrateDist = 1.0f;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        //�ѱ� ���� �α� 
        float totalDist = concentrateDist * (barrelCount - 1) / 2.0f;
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * dir;
        Vector2 upDist = pos + (upVec * totalDist);

        for (int i = 0; i < barrelCount;  i++)
        {
            Shoot(upDist - (concentrateDist * i * upVec), dir);
        }
        //Shoot(pos, dir);


        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }
}
