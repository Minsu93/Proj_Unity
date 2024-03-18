using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EA_Neutral : EnemyAction
{
    /// <summary>
    /// enemyState �� Attack�� ���� �������� �Ѿ��� ��� ����.  
    /// </summary>

    public override void DoAction(EnemyState state)
    {
        return;
    }

    protected override void Update()
    {
        if (!activate) return;

        EnemyState currState = brain.enemyState;

        if (currState == EnemyState.Die) activate = false;

        if (attackCool)
        {
            timer += Time.deltaTime;
            if (timer > attackCoolTime)
            {
                attackCool = false;
                timer = 0;
            }
            return;
        }

        if(currState == EnemyState.Attack)
        {
            attackCool = true;

            //�����ϱ�
            Vector2 gunTipPos = transform.position;
            Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90f) * transform.up) ;

            //�Ѿ� ����
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[0].projectile);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = gunTipRot;
            projectile.GetComponent<Projectile>().init(projectileStructs[0].damage, projectileStructs[0].speed, projectileStructs[0].range, projectileStructs[0].lifeTime);

        }
    }

    


}
