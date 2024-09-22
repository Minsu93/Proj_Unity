using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_AssistShoot : DroneItem_Projectile
{
    public override void UseDrone(Vector2 mousePos, Quaternion quat)
    {
        if (stopFollow) return;
        if (useDrone) return;

        Debug.Log("Start Use Drone");

        //playerWeapon에서 총을 쏠 때마다 같이 쏜다. 
        GameManager.Instance.playerManager.playerWeapon.weaponShootAction += GunAttack;
    }

    protected override void EndUseDrone()
    {
        GameManager.Instance.playerManager.playerWeapon.weaponShootAction -= GunAttack;
        //같이 쏘는 이벤트 리스너를 제거한다. 
        base.EndUseDrone();
    }


    protected void GunAttack()
    {
        Vector2 mousePos = GameManager.Instance.playerManager.playerBehavior.mousePos;
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        Shoot(transform.position, dir);
    }
}
