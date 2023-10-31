using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySniper : EnemyAction
{


    public override void ChaseAction()
    {
        //타겟 목표(Point)로 이동한다.
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
        //충전 딜레이
        //사격 딜레이 * 3번
        //이후 쿨타임 
        StartAimEvent();
        yield return StartCoroutine(ChargeRoutine(chargeTime));
        yield return StartCoroutine(ShootRoutine(shootDelay));
        StopAimEvent();
        yield return StartCoroutine(DelayRoutine(afterShootDelay));

        //공격 상태라면 다시 감지 상태로 돌아간다
        if (brain.enemyState == EnemyState.Attack)
            brain.enemyState = EnemyState.Idle;
    }

    protected IEnumerator ChargeRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    protected IEnumerator ShootRoutine(float delay)
    {
        Vector3 dir = (brain.playerTr.position - gunTip.transform.position).normalized; //발사 각도
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //총알 생성
        GameObject projectile = GameManager.Instance.poolManager.GetEnemyProj(projectilePrefab);
        projectile.transform.position = gunTip.position;
        projectile.transform.rotation = targetRotation * randomRotation;
        projectile.GetComponent<Projectile>().init(damage, lifeTime, projectileSpeed);

        ShootEvent();

        yield return new WaitForSeconds(delay);
    }



}
