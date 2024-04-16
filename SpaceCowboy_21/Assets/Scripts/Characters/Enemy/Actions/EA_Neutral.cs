using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EA_Neutral : EnemyAction
{
    /// <summary>
    /// enemyState 가 Attack일 때만 위쪽으로 총알을 쏘는 로직.  
    /// Attack일 때, 쿨타임이 아니면, 지속적으로 공격해라. 
    /// </summary>

    protected override void DoAction(EnemyState state)
    {
        if (state == EnemyState.Attack) { onAttack = true; }
        else onAttack = false;
    }

    //protected override void OnAttackAction()
    //{
    //    attackCool = true;

    //    //공격하기
    //    Vector2 gunTipPos = transform.position;
    //    Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90f) * transform.up);

    //    //총알 생성
    //    GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[0].projectile);
    //    projectile.transform.position = gunTipPos;
    //    projectile.transform.rotation = gunTipRot;
    //    projectile.GetComponent<Projectile>().Init(projectileStructs[0].damage, projectileStructs[0].speed, projectileStructs[0].lifeTime);
    //}


}
