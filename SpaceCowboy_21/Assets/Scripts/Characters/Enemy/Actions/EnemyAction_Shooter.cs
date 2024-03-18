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

        //    yield return StartCoroutine(ShootRoutine(AttackDelay));

        //    StopSeeEvent();
        //    AimOffEvent();

        //    yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        //    StopAttack();
        //}

        //#region ShootMethod
        //protected IEnumerator ShootRoutine(float delay)
        //{
        //    //gunTipRot, gunTipPos 업데이트
        //    var guntip = enemyview_s.GetGunTipPos();
        //    Vector2 gunTipPos = guntip.Item1;
        //    Quaternion gunTipRot = guntip.Item2;

        //    Quaternion targetRotation = gunTipRot;

        //    //랜덤 각도 추가
        //    float randomAngle = Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        //    Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //    //총알 생성
        //    GameObject projectile = GameManager.Instance.poolManager.GetEnemyProj(projectilePrefab);
        //    projectile.transform.position = gunTipPos;
        //    projectile.transform.rotation = targetRotation * randomRotation;
        //    projectile.GetComponent<Projectile>().init(damage, projectileSpeed, range, lifeTime);

        //    //View에서 애니메이션 실행
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

