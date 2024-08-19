using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class FollowingShuttle : MonoBehaviour
{
    Rigidbody2D rb;
    public ShuttleSkill skill;
    public bool shuttleActivate = false; //셔틀이 스폰되고 나서, 셔틀이 활성화되어 입력이 감지될 때 True. 플레이어가 죽으면 False
    bool startSkill; //스킬 사용 시작. 스킬 버튼을 누른 순간 True, 스킬이 사용이 완료/취소 되어 변신이 풀리면 False
    bool useSkill = false;      //완전히 스킬 사용으로 넘어감. 이동이 끝난 상태. 
    bool stopAttack = false;    //공격을 잠시 중단하고 제 위치로 이동하는것에 집중

    [SerializeField] Collider2D physicsColl;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        //Test
        InitializeShuttle();
        skill.ShuttleSkillInitialize();
        
    }

    // 셔틀 처음 스테이지에 생성 시 
    public void InitializeShuttle()
    {
        shuttleActivate = true;
        ResetShuttle();
    }

    private void FixedUpdate()
    {
        if (!shuttleActivate) return;

        //스킬 버튼 클릭시 
        if (startSkill)
        {
            if(!useSkill)
            {
                //거리 계산
                float dist = Vector2.Distance(transform.position, ShuttleMoveTargetPos);
                if(dist < 0.1f)
                {
                    //스킬 진짜 시작
                    useSkill = true;
                    physicsColl.enabled = false;
                    skill.gameObject.SetActive(true);

                    ResetDel resetDel = new ResetDel(ResetShuttle);
                    skill.ActivateShuttleSkill(resetDel);
                }
                else
                {
                    //스킬 위치까지 이동
                    RigidBodyMoveToPosition(ShuttleMoveTargetPos, out _ );
                }
            }
            
        }
        else
        {
            //평소
            RigidBodyFollowPlayer();
        } 
        
    }

    #region BaseMovement

    [Header("ShuttleMovement")]
    [SerializeField] Vector2 positionVector = new Vector2(-1, 1);
    [SerializeField] float maxDistance = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // 최소 감속 시간
    [SerializeField] float maxSmoothTime = 0.5f; // 최대 감속 시간
    private Vector2 velocity = Vector2.zero; // 현재 속도 (SmoothDamp에서 사용)
    Vector2 ShuttleMoveTargetPos = Vector2.zero;


    void RigidBodyFollowPlayer()
    {
        Transform targetTr = GameManager.Instance.player;
        if (targetTr == null) return;

        // 목표 위치: 플레이어의 위치
        bool right = GameManager.Instance.playerManager.playerBehavior.aimRight;
        int minus = right ? 1 : -1;
        Vector2 targetPosition = targetTr.position + (targetTr.up * positionVector.y) + (targetTr.right * positionVector.x * minus);

        RigidBodyMoveToPosition(targetPosition, out float distanceToPlayer);
        if(distanceToPlayer < 1f)
        {
            physicsColl.enabled = true;
            stopAttack = false;
        }
        else if (distanceToPlayer > 5.0f)
        {
            physicsColl.enabled = false;
            stopAttack = true;
        }
    }

    void RigidBodyMoveToPosition(Vector2 targetPos, out float distance)
    {
        Vector2 targetPosition = targetPos;

        // 현재 위치와 목표 위치 간의 거리
        distance = Vector2.Distance(transform.position, targetPosition) ;

        // 거리 기반으로 smoothTime 조절
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance / maxDistance);

        // 부드럽게 이동 (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
    }

    #endregion

    private void Update()
    {
        //스킬 사용 입력 감지 >> 스킬 사용을 위한 이동 시작.
        if (!shuttleActivate) return;

        if (!startSkill && Input.GetKeyDown(KeyCode.Alpha1))
        {
            startSkill = true;
            physicsColl.enabled = false;
            ShuttleMoveTargetPos = GameManager.Instance.playerManager.playerBehavior.mousePos;
        }

        //기본 공격 
        if (startSkill || stopAttack) return;
        
        BaseAttackMethod();
    }

    #region BaseAttack

    [Header("Enemy Check Range")]
    [SerializeField] float enemyCheckRange = 10f;
    [SerializeField] float enemyCheckInterval = 0.2f;
    float checkTimer;

    [Header("AttackProperty")]
    [SerializeField] ProjectileAttackProperty attackProperty;
    float lastShootTime;
    Transform targetTr;

    void BaseAttackMethod()
    {
        //시간 체크
        checkTimer += Time.deltaTime;
        if (checkTimer >= enemyCheckInterval)
        {
            //적 체크
            checkTimer = 0;
            targetTr = CheckEnemyIsNear();
        }
        
        if (targetTr != null)
        {
            //발사 
            GunAttack(targetTr);
        }
    }

    Transform CheckEnemyIsNear()
    {
        Transform targetTr = null;
        float minDist = float.MaxValue;
        Collider2D[] enemyColls = new Collider2D[10];
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

    void GunAttack(Transform targetTr)
    {
        if (Time.time - lastShootTime > attackProperty.shootCoolTime)
        {
            //쏜 시간 체크
            lastShootTime = Time.time;

            //사격
            StartCoroutine(burstShootRoutine(targetTr));
        }
    }

    protected IEnumerator burstShootRoutine(Transform targetTr)
    {
        for (int i = 0; i < attackProperty.numberOfBurst; i++)
        {
            Vector2 pos = transform.position;
            Vector2 dir = ((Vector2)targetTr.position - pos).normalized;
            Shoot(pos, dir);
            yield return new WaitForSeconds(attackProperty.burstInterval);
        }
    }

    protected virtual void Shoot(Vector2 pos, Vector3 dir)
    {
        float totalSpread = attackProperty.projectileSpread * (attackProperty.numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //랜덤 각도 추가
        float randomAngle = UnityEngine.Random.Range(-attackProperty.randomSpreadAngle * 0.5f, attackProperty.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //멀티샷
        for (int i = 0; i < attackProperty.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, attackProperty.projectileSpread * (i));

            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(attackProperty.projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(attackProperty.damage, attackProperty.speed, attackProperty.lifeTime, attackProperty.range);
        }
    }
    #endregion

    void ResetShuttle()
    {
        startSkill = false;
        useSkill = false;
        stopAttack = false;
        physicsColl.enabled = true;
    }


}

public delegate void ResetDel();


[Serializable] 
public struct ProjectileAttackProperty
{
    public int numberOfBurst;
    public float burstInterval;
    public int numberOfProjectile;
    public float projectileSpread;
    public float randomSpreadAngle;
    public GameObject projectilePrefab;
    public float damage;
    public float speed;
    public float lifeTime;
    public float range;
    public float shootCoolTime ;
}
