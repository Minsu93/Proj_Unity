using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceEnemy
{
    public class EnemyAction_Shooter : EnemyAction
    {

        //[Header("Projectile Property")]
        //public GameObject projectilePrefab;
        //public float randomSpreadAngle;
        //public float damage;
        //public float projectileSpeed;
        //public float lifeTime;
        //public float range;

        //EnemyView_Shooter enemyview_s;
        //protected override void Awake()
        //{
        //    base.Awake();

        //    enemyview_s = GetComponentInChildren<EnemyView_Shooter>();


        //}
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

        //    yield return StartCoroutine(ShootRoutine(AttackDelay));

        //    StopSeeEvent();
        //    AimOffEvent();

        //    yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        //    StopAttack();
        //}

        //#region ShootMethod
        //protected IEnumerator ShootRoutine(float delay)
        //{
        //    //gunTipRot, gunTipPos ������Ʈ
        //    var guntip = enemyview_s.GetGunTipPos();
        //    Vector2 gunTipPos = guntip.Item1;
        //    Quaternion gunTipRot = guntip.Item2;

        //    Quaternion targetRotation = gunTipRot;

        //    //���� ���� �߰�
        //    float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        //    Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //    //�Ѿ� ����
        //    GameObject projectile = GameManager.Instance.poolManager.GetEnemyProj(projectilePrefab);
        //    projectile.transform.position = gunTipPos;
        //    projectile.transform.rotation = targetRotation * randomRotation;
        //    projectile.GetComponent<Projectile>().init(damage, projectileSpeed, range, lifeTime);

        //    //View���� �ִϸ��̼� ����
        //    AttackEvent();

        //    yield return new WaitForSeconds(delay);
        //}


        //#endregion
        public override void DoAction(EnemyState state)
        {
            throw new System.NotImplementedException();
        }
    }
}

