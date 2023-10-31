using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_P_Bubble : Projectile_Player
{
    [Header("Bubble")]
    public float awakeTimer = 0.5f;
    public float checkRadius = 5f;
    public int numberOfProjectiles = 6;
    public GameObject projectilePrefab;
    int targetLayer;
    RaycastHit2D[] targetHits;
    protected override void Awake()
    {
        base.Awake();
        targetLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyHitableProj") | 1 << LayerMask.NameToLayer("Trap");

    }
    public override void init(float damage, float lifeTime, float speed, int reflect)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.speed = speed;
        time = 0;

        activate = true;
        coll.enabled = false;
        ViewObj.SetActive(true);

        if (trail != null)
        {
            trail.enabled = true;
            trail.Clear();
        }

        ProjectileViewReset();

        if (gravityOn)
        {
            projectileGravity.activate = true;
            //행성 리스트 초기화
            projectileGravity.gravityPlanets.Clear();
            projectileGravity.ResetGravityMultiplier();
        }

        projectileMovement.StartMovement(speed * rb.mass);

        StartCoroutine(LaunchRoutine(awakeTimer));
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile"))
        {
            AfterHitEvent();
        }
    }

    protected override void AfterHitEvent()
    {
        if(!activate)
        {
            return;
        }

        base.AfterHitEvent();

        targetHits = CheckTarget();
        AimShoot(targetHits.Length);
        RandomShoot(numberOfProjectiles - targetHits.Length);
    }

    IEnumerator LaunchRoutine(float sec)
    {
        yield return new WaitForSeconds(sec);

        coll.enabled = true;
    }

    RaycastHit2D[] CheckTarget()
    {
        //주변에 적이 있는지 체크

        RaycastHit2D[] hits;
        hits = Physics2D.CircleCastAll(transform.position, checkRadius, Vector2.right, 0f, targetLayer);

        return hits;
    }

    void AimShoot(int num)
    {
        //적을 겨냥한 총알 발사
        for (int i = 0; i < num; i++)
        {
            Vector2 targetVec = (targetHits[i].transform.position - transform.position).normalized;
            Vector2 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = targetRot;
            projectile.GetComponent<Projectile_Player>().init(1, 1, 10, 0);
        }
    }

    void RandomShoot(int num)
    {

        //랜덤 방향으로 총알 발사
        for (int i = 0; i < num; i++)
        {

            Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(-180, 180f));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = randomRot;
            projectile.GetComponent<Projectile_Player>().init(1, 1, 10, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

}
