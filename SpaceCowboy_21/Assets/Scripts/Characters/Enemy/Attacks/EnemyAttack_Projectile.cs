using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Projectile : EnemyAttack
{
    //���� �Ӽ� 
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

    //��ũ��Ʈ
    GunTipPoser tipPoser;
    protected override void Awake()
    {
        base.Awake();
        tipPoser = GetComponentInChildren<GunTipPoser>();
    }

    //���� �ൿ
    public override void OnAttackAction()
    {
        if (_attackCool) return;

        _attackCool = true;
        StartCoroutine(AttackCoolRoutine());
        StartCoroutine(AttackRoutine());     
    }
    


    protected virtual IEnumerator AttackRoutine()
    {
        enemyAction.StartAimStart();
        //�ٸ� �ൿ ����.
        enemyAction.EnemyPause(attackCoolTime);

        yield return new WaitForSeconds(preAttackDelay);

        int burst = burstNumber;
        while (burst > 0)
        {
            burst--;
            var guntip = tipPoser.GetGunTipPos();         //�� ��� ��ġ, ȸ������ �����;� �Ѵ�. 
            ShootAction(guntip.Item1, guntip.Item2);
            enemyAction.StartAttackView();   //�ִϸ��̼� ���� 
            yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(afterAttackDelay);

        enemyAction.StartAimStop();
    }

    public void ShootAction(Vector2 pos, Quaternion Rot)
    {
        float totalSpread = this.anglePerProjectile * (numberOfProjectiles - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        //���� ���� �߰�
        float randomAngle = Random.Range(-this.randomAngle * 0.5f, this.randomAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //���� ����
        Quaternion targetRotation = Rot * Quaternion.Euler(0, 0, -(totalSpread / 2));    

        //��Ƽ��
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, totalSpread * (i));

            //�Ѿ� ����
            GameObject proj = GameManager.Instance.poolManager.GetEnemyProj(projectile);
            proj.transform.position = pos;
            proj.transform.rotation = tempRot * randomRotation;
            proj.GetComponent<Projectile_Enemy>().Init(damage, speed, lifeTime, range);
        }
    }

    //protected void ShootOrbitAction(Vector2 pos, Quaternion Rot, int projIndex, Planet planet, bool isRight)
    //{
    //    Vector2 gunTipPos = pos;
    //    Quaternion gunTipRot = Rot;

    //    Quaternion targetRotation = gunTipRot;

    //    //���� ���� �߰�
    //    float randomAngle = Random.Range(-projectileStructs[projIndex].spreadAngle * 0.5f, projectileStructs[projIndex].spreadAngle * 0.5f);
    //    Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

    //    //�Ѿ� ����
    //    GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[projIndex].projectile);
    //    projectile.transform.position = gunTipPos;
    //    projectile.transform.rotation = targetRotation * randomRotation;

    //    Proj_Orbit orbit = projectile.GetComponent<Proj_Orbit>();
    //    orbit.Init(projectileStructs[projIndex].damage, projectileStructs[projIndex].speed, projectileStructs[projIndex].lifeTime);
    //    orbit.SetOrbit(planet, isRight);

    //    AttackView();

    //}
}
