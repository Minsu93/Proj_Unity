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
    //    //Ÿ���� �ٶ󺻴�
    //    //SeePlayerOnceEvent();
    //    //Ÿ�� ��ǥ(Point)�� �̵��Ѵ�.
    //    MoveToVisiblePoint(brain.playerTr.position, 10f);
    //}

    //public override void AttackAction()
    //{
    //    //Ÿ���� �����Ѵ�
    //    StartSeeEvent();

    //    attackRoutine = StartCoroutine(AttackCoroutine());
    //}

    //public override void DieAction()
    //{
    //    //������ �ߴ��Ѵ�.
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
    //    //View���� �ִϸ��̼� ����
    //    AttackEvent();

    //    yield return new WaitForSeconds(delay);
    //}
    public override void DoAction(EnemyState state)
    {
        throw new System.NotImplementedException();
    }
}
