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

        //playerWeapon���� ���� �� ������ ���� ���. 
        GameManager.Instance.playerManager.playerWeapon.weaponShootAction += GunAttack;
    }

    protected override void EndUseDrone()
    {
        GameManager.Instance.playerManager.playerWeapon.weaponShootAction -= GunAttack;
        //���� ��� �̺�Ʈ �����ʸ� �����Ѵ�. 
        base.EndUseDrone();
    }


    protected void GunAttack()
    {
        Vector2 mousePos = GameManager.Instance.playerManager.playerBehavior.mousePos;
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;
        Shoot(transform.position, dir);
    }
}
