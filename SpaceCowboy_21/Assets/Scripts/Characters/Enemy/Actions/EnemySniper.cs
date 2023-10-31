using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySniper : EnemyAction
{


    public override void ChaseAction()
    {
        //Ÿ�� ��ǥ(Point)�� �̵��Ѵ�.
        MoveToVisiblePoint(brain.playerTr.position, 10f);
    }

    public override void AttackAction()
    {
        StartCoroutine(AttackCoroutine());
    }

    public override void DieAction()
    {
        DieEvent();
    }


    IEnumerator AttackCoroutine()
    {
        //���� ������
        //��� ������ * 3��
        //���� ��Ÿ�� 
        StartAimEvent();
        yield return StartCoroutine(ChargeRoutine(chargeTime));
        yield return StartCoroutine(ShootRoutine(shootDelay));
        StopAimEvent();
        yield return StartCoroutine(DelayRoutine(afterShootDelay));

        //���� ���¶�� �ٽ� ���� ���·� ���ư���
        if (brain.enemyState == EnemyState.Attack)
            brain.enemyState = EnemyState.Idle;
    }

    protected IEnumerator ChargeRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    protected IEnumerator ShootRoutine(float delay)
    {
        Vector3 dir = (brain.playerTr.position - gunTip.transform.position).normalized; //�߻� ����
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //�Ѿ� ����
        GameObject projectile = GameManager.Instance.poolManager.GetEnemyProj(projectilePrefab);
        projectile.transform.position = gunTip.position;
        projectile.transform.rotation = targetRotation * randomRotation;
        projectile.GetComponent<Projectile>().init(damage, lifeTime, projectileSpeed);

        ShootEvent();

        yield return new WaitForSeconds(delay);
    }



}
