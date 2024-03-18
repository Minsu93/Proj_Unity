using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction_Warrior : EnemyAction
{
    //public override void IdleAction()
    //{
    //    StopSeeEvent();
    //    StartIdleEvent();
    //}

    //public override void ChaseAction()
    //{
    //    //타겟을 바라본다
    //    //SeePlayerOnceEvent();
    //    //타겟 목표(Point)로 이동한다.
    //    MoveToVisiblePoint(brain.playerTr.position, 10f);
    //}

    //public override void AttackAction()
    //{
    //    //타겟을 추적한다
    //    StartSeeEvent();

    //    attackRoutine = StartCoroutine(AttackCoroutine());
    //}

    //public override void DieAction()
    //{
    //    //추적을 중단한다.
    //    StopSeeEvent();
    //    StopChase();
    //    StopChasePlanet();
    //    StopAttack();

    //    DieEvent();
    //}

    //public override void HitAction()
    //{
    //    HitEvent();
    //}

    //IEnumerator AttackCoroutine()
    //{
    //    AimOnEvent();

    //    yield return StartCoroutine(DelayRoutine(preAttackDelay));

    //    yield return StartCoroutine(MeleeAttackRoutine(AttackDelay));

    //    StopSeeEvent() ;
    //    AimOffEvent();

    //    yield return StartCoroutine(DelayRoutine(afterAttackDelay));

    //    StopAttack();
    //}

    //IEnumerator MeleeAttackRoutine(float delay)
    //{
    //    //View에서 애니메이션 실행
    //    AttackEvent();

    //    yield return new WaitForSeconds(delay);
    //}
    public override void DoAction(EnemyState state)
    {
        throw new System.NotImplementedException();
    }
}
