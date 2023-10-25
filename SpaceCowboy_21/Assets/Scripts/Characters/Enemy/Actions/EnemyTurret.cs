using SpaceEnemy;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : EnemyAction
{
    public SkeletonAnimation skeletonAnimation;
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string eventName;

    [Space]
    public bool logDebugMessage = false;

    Spine.EventData eventData;


    private void Start()
    {
        if (skeletonAnimation == null) return;
        skeletonAnimation.Initialize(false);
        if (!skeletonAnimation.valid) return;

        eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);
        skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
    }

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (logDebugMessage) Debug.Log("Event fired! " + e.Data.Name);
        //bool eventMatch = string.Equals(e.Data.Name, eventName, System.StringComparison.Ordinal); // Testing recommendation: String compare.
        bool eventMatch = (eventData == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch)
        {
            Shoot();
        }
    }


    public override void ChaseAction()
    {
        return;
    }

    public override void AttackAction()
    {
        StartCoroutine(AttackCoroutine());
    }

    public override void DieAction()
    {
        StopAllCoroutines();
    }


    IEnumerator AttackCoroutine()
    {
        //충전 딜레이
        yield return StartCoroutine(ChargeRoutine(chargeTime));
        yield return StartCoroutine(ShootRoutine(shootDelay));
        yield return StartCoroutine(ShootRoutine(shootDelay));
        yield return StartCoroutine(ShootRoutine(shootDelay));

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
        ShootEvent();
        yield return new WaitForSeconds(delay);
    }

    void Shoot()
    {
        Vector3 dir = gunTip.transform.up; //발사 각도
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
    }
}
