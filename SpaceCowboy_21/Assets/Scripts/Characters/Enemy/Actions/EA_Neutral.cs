using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EA_Neutral : EnemyAction
{
    /// <summary>
    /// enemyState �� Attack�� ���� �������� �Ѿ��� ��� ����.  
    /// Attack�� ��, ��Ÿ���� �ƴϸ�, ���������� �����ض�. 
    /// </summary>

    protected override void DoAction(EnemyState state)
    {
        if (state == EnemyState.Attack) { onAttack = true; }
        else onAttack = false;
    }

    //protected override void OnAttackAction()
    //{
    //    attackCool = true;

    //    //�����ϱ�
    //    Vector2 gunTipPos = transform.position;
    //    Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90f) * transform.up);

    //    //�Ѿ� ����
    //    GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[0].projectile);
    //    projectile.transform.position = gunTipPos;
    //    projectile.transform.rotation = gunTipRot;
    //    projectile.GetComponent<Projectile>().Init(projectileStructs[0].damage, projectileStructs[0].speed, projectileStructs[0].lifeTime);
    //}


}
