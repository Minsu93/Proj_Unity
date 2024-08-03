using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_SingleShoot : WeaponType
{
    public bool shootOnce { get; set; }  //√— ΩÓ±‚ Ω√¿€

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        if (shootOnce) return;
        //√— πﬂªÁ ¡÷±‚
        if (Time.time - lastShootTime < weaponStats.shootInterval) return;

        else
        {
            shootOnce = true;

            StartCoroutine(burstRoutine(pos, dir, numberOfBurst, burstInterval));
            
            //PlayerWeaponø°º≠ »ƒ√≥∏Æ
            AfterShootProcess();
        }

    }

    IEnumerator burstRoutine(Vector2 pos, Vector3 dir, int repeatNumber, float interval)
    {
        for(int i = 0; i < repeatNumber; i++)
        {
            Shoot(pos, dir);
            yield return new WaitForSeconds(interval);
        }
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        shootOnce = false;
    }
}
