using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_SpringCooler : WeaponType
{
    [SerializeField] BarrelType type = BarrelType.InOrder;
    [SerializeField] float angleDist = 30f;
    [SerializeField] float pointDist = 0.3f;
    int index = 0;
    int pingpongIndex = 0;


    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        int barrelCount = numberOfProjectile;
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        switch (type)
        {
            case BarrelType.InOrder:
                index = (index + 1) % barrelCount;   // �ѱ� ������ ���� 0,1,2,...0,1,2... ������ �ݺ�
                break;
            case BarrelType.PingPong:
                pingpongIndex++;
                index = GetPingPongIndex(pingpongIndex, barrelCount);
                break;
        }

        Vector2 upVec = Quaternion.Euler(0, 0, 90) * dir;
        Vector2 bottomPoint = pos - (((barrelCount - 1) * pointDist / 2) * upVec);
        Vector2 point = bottomPoint + (pointDist * index * upVec);

        float anglePerShot = angleDist / (barrelCount - 1);
        float baseAngle = -1 * angleDist * 0.5f;
        Vector2 rotatedDir = Quaternion.Euler(0, 0, baseAngle + (anglePerShot * index)) * dir;

        Shoot(point, rotatedDir, projectilePrefab);


        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        return;
    }

    int GetPingPongIndex(int currentFrame, int maxIndex)
    {
        int range = maxIndex - 1;
        int pingPongValue = currentFrame % (2 * range);
        return Mathf.Abs(pingPongValue - range);
    }
}
