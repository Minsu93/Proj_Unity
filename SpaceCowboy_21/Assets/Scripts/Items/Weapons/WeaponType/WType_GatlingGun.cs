using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_GatlingGun : WeaponType
{
    /// 쏠 때마다 총알을 쏘는 속도가 빨라진다. 

    [SerializeField] float speedUpDuration = 3f;    //최고 공격속도까지 x초 걸린다. 
    [SerializeField] float speedMultiplierMax = 2f; //x+1배 까지 빨라진다. 
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

        //총 발사 주기
        if (Time.time - lastShootTime < interval) return;

        Shoot(pos, dir, projectilePrefab);

        //PlayerWeapon에서 후처리
        AfterShootProcess();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        curSpeed = 0;
    }
}
