using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Projectile_Upward : EnemyAttack_Projectile
{
    public GameObject guntipManual;
    protected override IEnumerator AttackRoutine()
    {
        enemyAction.StartAimStart();
        //�ٸ� �ൿ ����.
        enemyAction.EnemyPause(attackCoolTime);

        yield return new WaitForSeconds(preAttackDelay);

        int burst = burstNumber;
        while (burst > 0)
        {
            burst--;
            //var guntip = tipPoser.GetGunTipPos();         //�� ��� ��ġ, ȸ������ �����;� �Ѵ�. 
            ShootAction(guntipManual.transform.position, guntipManual.transform.rotation);
            enemyAction.StartAttackView();   //�ִϸ��̼� ���� 
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(afterAttackDelay);

        enemyAction.StartAimStop();
    }
}
