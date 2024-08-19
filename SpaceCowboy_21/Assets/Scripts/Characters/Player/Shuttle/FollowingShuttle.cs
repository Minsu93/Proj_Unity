using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class FollowingShuttle : MonoBehaviour
{
    Rigidbody2D rb;
    public ShuttleSkill skill;
    public bool shuttleActivate = false; //��Ʋ�� �����ǰ� ����, ��Ʋ�� Ȱ��ȭ�Ǿ� �Է��� ������ �� True. �÷��̾ ������ False
    bool startSkill; //��ų ��� ����. ��ų ��ư�� ���� ���� True, ��ų�� ����� �Ϸ�/��� �Ǿ� ������ Ǯ���� False
    bool useSkill = false;      //������ ��ų ������� �Ѿ. �̵��� ���� ����. 
    bool stopAttack = false;    //������ ��� �ߴ��ϰ� �� ��ġ�� �̵��ϴ°Ϳ� ����

    [SerializeField] Collider2D physicsColl;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        //Test
        InitializeShuttle();
        skill.ShuttleSkillInitialize();
        
    }

    // ��Ʋ ó�� ���������� ���� �� 
    public void InitializeShuttle()
    {
        shuttleActivate = true;
        ResetShuttle();
    }

    private void FixedUpdate()
    {
        if (!shuttleActivate) return;

        //��ų ��ư Ŭ���� 
        if (startSkill)
        {
            if(!useSkill)
            {
                //�Ÿ� ���
                float dist = Vector2.Distance(transform.position, ShuttleMoveTargetPos);
                if(dist < 0.1f)
                {
                    //��ų ��¥ ����
                    useSkill = true;
                    physicsColl.enabled = false;
                    skill.gameObject.SetActive(true);

                    ResetDel resetDel = new ResetDel(ResetShuttle);
                    skill.ActivateShuttleSkill(resetDel);
                }
                else
                {
                    //��ų ��ġ���� �̵�
                    RigidBodyMoveToPosition(ShuttleMoveTargetPos, out _ );
                }
            }
            
        }
        else
        {
            //���
            RigidBodyFollowPlayer();
        } 
        
    }

    #region BaseMovement

    [Header("ShuttleMovement")]
    [SerializeField] Vector2 positionVector = new Vector2(-1, 1);
    [SerializeField] float maxDistance = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // �ּ� ���� �ð�
    [SerializeField] float maxSmoothTime = 0.5f; // �ִ� ���� �ð�
    private Vector2 velocity = Vector2.zero; // ���� �ӵ� (SmoothDamp���� ���)
    Vector2 ShuttleMoveTargetPos = Vector2.zero;


    void RigidBodyFollowPlayer()
    {
        Transform targetTr = GameManager.Instance.player;
        if (targetTr == null) return;

        // ��ǥ ��ġ: �÷��̾��� ��ġ
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

        // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ�
        distance = Vector2.Distance(transform.position, targetPosition) ;

        // �Ÿ� ������� smoothTime ����
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance / maxDistance);

        // �ε巴�� �̵� (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
    }

    #endregion

    private void Update()
    {
        //��ų ��� �Է� ���� >> ��ų ����� ���� �̵� ����.
        if (!shuttleActivate) return;

        if (!startSkill && Input.GetKeyDown(KeyCode.Alpha1))
        {
            startSkill = true;
            physicsColl.enabled = false;
            ShuttleMoveTargetPos = GameManager.Instance.playerManager.playerBehavior.mousePos;
        }

        //�⺻ ���� 
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
        //�ð� üũ
        checkTimer += Time.deltaTime;
        if (checkTimer >= enemyCheckInterval)
        {
            //�� üũ
            checkTimer = 0;
            targetTr = CheckEnemyIsNear();
        }
        
        if (targetTr != null)
        {
            //�߻� 
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
