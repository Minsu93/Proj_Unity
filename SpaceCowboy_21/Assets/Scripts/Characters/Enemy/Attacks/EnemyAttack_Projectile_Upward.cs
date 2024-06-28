using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Projectile_Upward : EnemyAttack_Projectile
{
    public GameObject guntipManual;
    protected override IEnumerator AttackRoutine()
    {
        enemyAction.StartAimStart();
        //다른 행동 정지.
        enemyAction.EnemyPause(attackCoolTime);

        yield return new WaitForSeconds(preAttackDelay);

        int burst = burstNumber;
        while (burst > 0)
        {
            burst--;
            //var guntip = tipPoser.GetGunTipPos();         //총 쏘는 위치, 회전값을 가져와야 한다. 
            ShootAction(guntipManual.transform.position, guntipManual.transform.rotation);
            enemyAction.StartAttackView();   //애니메이션 실행 
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(afterAttackDelay);

        enemyAction.StartAimStop();
    }
}
