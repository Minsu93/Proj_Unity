using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Projectile : EnemyAttack
{
    //세부 속성 
    [Space]
    public float damage;
    public float speed;
    public float lifeTime;
    public float range;
    [Space]
    public float randomAngle;
    [Space]
    public int numberOfProjectiles;
    public float anglePerProjectile;
    [Space]
    public int burstNumber;
    public float burstDelay;
    [Space]
    public GameObject projectile;

    //스크립트
    GunTipPoser tipPoser;
    protected override void Awake()
    {
        base.Awake();
        tipPoser = GetComponentInChildren<GunTipPoser>();
    }

    //공격 행동
    public override void OnAttackAction()
    {
        if (_attackCool) return;

        _attackCool = true;
        StartCoroutine(AttackCoolRoutine());

        //enemyAction.EnemyPause(attackCoolTime);
        enemyAction.AfterAttack();
        StartCoroutine(AttackRoutine());     
    }
    


    protected virtual IEnumerator AttackRoutine()
    {
        enemyAction.StartAimStart();
        //다른 행동 정지.

        yield return new WaitForSeconds(preAttackDelay);

        int burst = burstNumber;
        while (burst > 0)
        {
            burst--;
            var guntip = tipPoser.GetGunTipPos();         //총 쏘는 위치, 회전값을 가져와야 한다. 
            ShootAction(guntip.Item1, guntip.Item2);
            enemyAction.StartAttackView();   //애니메이션 실행 
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(afterAttackDelay);

        enemyAction.StartAimStop();
    }

    public void ShootAction(Vector2 pos, Quaternion Rot)
    {
        float totalSpread = this.anglePerProjectile * (numberOfProjectiles - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        //랜덤 각도 추가
        float randomAngle = Random.Range(-this.randomAngle * 0.5f, this.randomAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //시작 각도
        Quaternion targetRotation = Rot * Quaternion.Euler(0, 0, -(totalSpread / 2));    

        //멀티샷
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, totalSpread * (i));

            //총알 생성
            GameObject proj = GameManager.Instance.poolManager.GetPoolObj(projectile, 1);
            proj.transform.position = pos;
            proj.transform.rotation = tempRot * randomRotation;
            proj.GetComponent<Projectile>().Init(damage, speed, lifeTime, range);
        }
    }

}
