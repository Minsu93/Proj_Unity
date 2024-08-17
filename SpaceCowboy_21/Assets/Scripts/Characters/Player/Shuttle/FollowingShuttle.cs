using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingShuttle : MonoBehaviour
{
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        RigidBodyFollowPlayer();
    }

    #region BaseMovement

    [Header("ShuttleMovement")]
    [SerializeField] Vector2 positionVector = new Vector2(-1, 1);
    [SerializeField] float maxDistance = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // �ּ� ���� �ð�
    [SerializeField] float maxSmoothTime = 0.5f; // �ִ� ���� �ð�
    float smoothTime;
    private Vector2 velocity = Vector2.zero; // ���� �ӵ� (SmoothDamp���� ���)


    void RigidBodyFollowPlayer()
    {
        Transform targetTr = GameManager.Instance.player;
        if (targetTr == null) return;

        // ��ǥ ��ġ: �÷��̾��� ��ġ
        bool right = GameManager.Instance.playerManager.playerBehavior.aimRight;
        int minus = right ? 1 : -1;
        Vector2 targetPosition = targetTr.position + (targetTr.up * positionVector.y) + (targetTr.right * positionVector.x * minus);

        RigidBodyMoveToPosition(targetPosition);
    }

    void RigidBodyMoveToPosition(Vector2 targetPos)
    {
        Vector2 targetPosition = targetPos;

        // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ�
        float distance = Vector2.Distance(transform.position, targetPosition) / maxDistance;

        // �Ÿ� ������� smoothTime ����
        smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance);

        // �ε巴�� �̵� (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
    }

    #endregion

    private void Update()
    {
        //�⺻ ���� 
        BaseAttackMethod();
    }

    #region BaseAttack

    [Header("Enemy Check Range")]
    [SerializeField] float enemyCheckRange = 10f;
    [SerializeField] float enemyCheckInterval = 0.2f;
    Collider2D[] enemyColls = new Collider2D[10];
    float checkTimer;

    [Header("AttackProperty")]
    [SerializeField] ProjectileAttackProperty attackProperty;
    float lastShootTime;

    void BaseAttackMethod()
    {
        //�ð� üũ
        checkTimer += Time.deltaTime;
        if (checkTimer < enemyCheckInterval) return;

        //�� üũ
        checkTimer = 0;
        Transform targetTr = CheckEnemyIsNear();
        if (targetTr == null) return;

        //�߻� üũ
        GunAttack(targetTr);
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

    void GunAttack(Transform targetTr)
    {
        if (Time.time - lastShootTime > attackProperty.shootCoolTime)
        {
            //�� �ð� üũ
            lastShootTime = Time.time;

            //���
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
        float totalSpread = attackProperty.projectileSpread * (attackProperty.numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90 - (totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //���� ���� �߰�
        float randomAngle = UnityEngine.Random.Range(-attackProperty.randomSpreadAngle * 0.5f, attackProperty.randomSpreadAngle * 0.5f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //��Ƽ��
        for (int i = 0; i < attackProperty.numberOfProjectile; i++)
        {
            Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, attackProperty.projectileSpread * (i));

            //�Ѿ� ����
            GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(attackProperty.projectilePrefab, 0);
            projectile.transform.position = pos;
            projectile.transform.rotation = tempRot * randomRotation;
            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Init(attackProperty.damage, attackProperty.speed, attackProperty.lifeTime, attackProperty.range);
        }
    }
    #endregion
}

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