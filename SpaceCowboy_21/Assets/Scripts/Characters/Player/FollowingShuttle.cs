using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingShuttle : MonoBehaviour
{
    [Header("ShuttleMovement")]
    [SerializeField] Vector2 positionVector = new Vector2(-1, 1);
    [SerializeField] float maxDistance = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // 최소 감속 시간
    [SerializeField] float maxSmoothTime = 0.5f; // 최대 감속 시간
    float smoothTime;
    private Vector2 velocity = Vector2.zero; // 현재 속도 (SmoothDamp에서 사용)
    Rigidbody2D rb;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Transform targetTr = GameManager.Instance.player;
        if (targetTr == null) return;

        // 목표 위치: 플레이어의 위치
        bool right = GameManager.Instance.playerManager.playerBehavior.aimRight;
        int minus = right ? 1 : -1;
        Vector2 targetPosition = targetTr.position + (targetTr.up * positionVector.y) + (targetTr.right * positionVector.x * minus);

        // 현재 위치와 목표 위치 간의 거리
        float distance = Vector2.Distance(transform.position, targetPosition) / maxDistance;

        // 거리 기반으로 smoothTime 조절
        smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance);

        // 부드럽게 이동 (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
    }

    [Header("Enemy Check Range")]
    [SerializeField] float enemyCheckRange = 10f;
    [SerializeField] float enemyCheckInterval = 0.2f;
    Collider2D[] enemyColls = new Collider2D[10];
    float checkTimer;

    [Header("AttackProperty")]
    [SerializeField] int numberOfBurst = 1;
    [SerializeField] float burstInterval = 0.2f;
    [SerializeField] int numberOfProjectile = 1;
    [SerializeField] float projectileSpread = 1;
    [SerializeField] float randomSpreadAngle = 1;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float damage = 1;
    [SerializeField] float speed = 1;
    [SerializeField] float lifeTime = 1;
    [SerializeField] float range = 1;
    [SerializeField] float shootCoolTime = 3.0f;
    float lastShootTime;


    private void Update()
    {
        //시간 체크
        checkTimer += Time.deltaTime;
        if (checkTimer < enemyCheckInterval) return;
        
        //적 체크
        checkTimer = 0;
        Transform targetTr = CheckEnemyIsNear();
        if (targetTr == null) return;
        
        //발사 체크
        BasicAttack(targetTr);
    }

    Transform CheckEnemyIsNear()
    {
        Transform targetTr = null;
        float minDist = float.MaxValue;
        int num = Physics2D.OverlapCircleNonAlloc(transform.position, enemyCheckRange, enemyColls, LayerMask.GetMask("Enemy"));
        if(num > 0)
        {
            for(int i = 0; i < num; i++)
            {
                float dist = Vector2.Distance(transform.position, enemyColls[i].transform.position);
                if(dist < minDist)
                {
                    minDist = dist;
                    targetTr = enemyColls[i].transform;
                }
            }
        }

        return targetTr;
    }

    void BasicAttack(Transform targetTr)
    {
        if (Time.time - lastShootTime > shootCoolTime)
        {
            //쏜 시간 체크
            lastShootTime = Time.time;

            //사격
            StartCoroutine(burstShootRoutine(targetTr));
        }
    }

    protected IEnumerator burstShootRoutine(Transform targetTr)
    {
        for (int i = 0; i < numberOfBurst; i++)
        {
            Vector2 pos = transform.position;
            Vector2 dir = ((Vector2)targetTr.position - pos).normalized;
            Shoot(pos, dir);
            yield return new WaitForSeconds(burstInterval);
        }
    }


    //실제 쏘는 행동
    protected virtual void Shoot(Vector2 pos, Vector3 dir)
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-randomSpreadAngle * 0.5f, randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(damage, speed, lifeTime, range);
        }
    }

}
