using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_GatlingGun : WeaponType
{
    /// �� ������ �Ѿ��� ��� �ӵ��� ��������. 

    [SerializeField] float speedUpDuration = 3f;    //�ְ� ���ݼӵ����� x�� �ɸ���. 
    [SerializeField] float speedMultiplierMax = 2f; //x+1�� ���� ��������. 
    float curSpeed = 0;
    float baseSpeed;
    float interval;

    public override void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        base.Initialize(weaponData, gunTipLocalPos);
        
        baseSpeed = shootInterval;
    }
    

    
    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        curSpeed += speedMultiplierMax / speedUpDuration * Time.deltaTime;

        if(curSpeed > speedMultiplierMax )
        {
            curSpeed = speedMultiplierMax;
        }
        interval = baseSpeed * 1 / (1 + curSpeed);

        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < interval) return;

        Shoot(pos, dir, projectilePrefab);

        //PlayerWeapon���� ��ó��
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        curSpeed = 0;
    }
}
