using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_P_Bubble : Projectile_Player
{
    //[Header("Bubble")]
    //public float awakeTimer = 0.5f;
    //public float checkRadius = 5f;
    //public int numberOfProjectiles = 6;
    //public GameObject projectilePrefab;
    //public LayerMask autoTargetLayer;
    //RaycastHit2D[] targetHits;

    //public override void init(float damage, float speed, float range, float lifeTime)
    //{
    //    this.damage = damage;
    //    this.lifeTime = lifeTime;
    //    this.speed = speed;

    //    activate = true;
    //    coll.enabled = false;
    //    ViewObj.SetActive(true);

    //    if (trail != null)
    //    {
    //        trail.enabled = true;
    //        trail.Clear();
    //    }

    //    InitProjectile();

    //    projectileMovement.StartMovement(speed * rb.mass);

    //    StartCoroutine(LaunchRoutine(awakeTimer));
    //}

    //protected override void LifeTimeOver()
    //{
    //    ShootProcess();
    //    AfterHitEvent();
    //}

    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("PlayerProjectile"))
    //    {
    //        ShootProcess();
    //        AfterHitEvent();
    //    }
    //}


    //IEnumerator LaunchRoutine(float sec)
    //{
    //    yield return new WaitForSeconds(sec);

    //    coll.enabled = true;
    //}

    //void ShootProcess()
    //{
    //    targetHits = CheckTarget();
    //    AimShoot(targetHits.Length);
    //    RandomShoot(numberOfProjectiles - targetHits.Length);
    //}


    //RaycastHit2D[] CheckTarget()
    //{
    //    //�ֺ��� ���� �ִ��� üũ

    //    RaycastHit2D[] hits;
    //    hits = Physics2D.CircleCastAll(transform.position, checkRadius, Vector2.right, 0f, autoTargetLayer);

    //    return hits;
    //}

    //void AimShoot(int num)
    //{
    //    //���� �ܳ��� �Ѿ� �߻�
    //    for (int i = 0; i < num; i++)
    //    {
    //        Vector2 targetVec = (targetHits[i].transform.position - transform.position).normalized;
    //        Vector2 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
    //        Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);

    //        //�Ѿ� ����
    //        GameObject projectile = PoolManager.instance.Get(projectilePrefab);
    //        projectile.transform.position = transform.position;
    //        projectile.transform.rotation = targetRot;
    //        projectile.GetComponent<Projectile_Player>().init(1, 1, 10, 0);
    //    }
    //}

    //void RandomShoot(int num)
    //{

    //    //���� �������� �Ѿ� �߻�
    //    for (int i = 0; i < num; i++)
    //    {

    //        Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(-180, 180f));

    //        //�Ѿ� ����
    //        GameObject projectile = PoolManager.instance.Get(projectilePrefab);
    //        projectile.transform.position = transform.position;
    //        projectile.transform.rotation = randomRot;
    //        projectile.GetComponent<Projectile_Player>().init(1, 1, 10, 0);
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position, checkRadius);
    //}

}
