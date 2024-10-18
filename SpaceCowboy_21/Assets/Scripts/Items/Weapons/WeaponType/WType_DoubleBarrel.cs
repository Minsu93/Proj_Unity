using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_DoubleBarrel : WeaponType
{
    [SerializeField] BarrelType type = BarrelType.InOrder;
    [SerializeField, Range(2,5)] int barrelCount = 2;
    [SerializeField] float PointDist = 0.5f;
    int index = 0;
    int pingpongIndex = 0;

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval) return;

        //�ѱ� ����
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * dir;
        switch(type)
        {
            case BarrelType.InOrder:
                index = (index + 1) % barrelCount;   // �ѱ� ������ ���� 0,1,2,...0,1,2... ������ �ݺ�
                break;
            case BarrelType.PingPong:
                pingpongIndex++;
                index = GetPingPongIndex(pingpongIndex, barrelCount);
                break;
        }
        Vector2 bottomPoint = pos - (((barrelCount - 1) * PointDist / 2) * upVec);
        Vector2 point = bottomPoint + (PointDist * index * upVec );
        Shoot(point, dir, projectilePrefab);

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
