using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class FollowingShuttle : MonoBehaviour
{
    Rigidbody2D rb;
    //public ShuttleSkill skill;
    //public List<ShuttleSkill> skillList = new List<ShuttleSkill>();
    ShuttleSkill curSkill;
    public bool shuttleActivate = false; //셔틀이 스폰되고 나서, 셔틀이 활성화되어 입력이 감지될 때 True. 플레이어가 죽으면 False
    bool startSkill; //스킬 사용 시작. 스킬 버튼을 누른 순간 True, 스킬이 사용이 완료/취소 되어 변신이 풀리면 False
    bool useSkill = false;      //완전히 스킬 사용으로 넘어감. 이동이 끝난 상태. 
    bool stopAttack = false;    //공격을 잠시 중단하고 제 위치로 이동하는것에 집중

    [SerializeField] Collider2D physicsColl;
    [SerializeField] Transform viewTr;
    Vector3 viewScale;

    //스킬 및 현재 쿨타임 
    public List<SkillCoolTime> skillCoolTimeList = new List<SkillCoolTime>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        viewScale = viewTr.localScale;
    }

    // 셔틀 처음 스테이지에 생성 시 
    public void InitializeShuttle()
    {
        shuttleActivate = true;
        ResetShuttle();
        LoadSkillData();

    }

    #region Load SkillData
    void LoadSkillData()
    {
        List<string> skillDatas = GameManager.Instance.skillDictionary.LoadEquippedSkill();
        skillCoolTimeList.Clear();
        //쿨타임 리스트 제작
        foreach(string dataName in skillDatas)
        {
            GameObject skillPrefab = GameManager.Instance.skillDictionary.GetSkillPrefab(dataName);
            GameObject obj = Instantiate(skillPrefab,this.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            if( obj.TryGetComponent<ShuttleSkill>(out ShuttleSkill skill))
            {
                //skillList.Add(skill);
                SkillCoolTime cool = new SkillCoolTime(skill, skill.skillCoolTime);
                skillCoolTimeList.Add(cool);
                
                skill.ShuttleSkillInitialize();
            }
            obj.SetActive(false);
        }

        //스킬 아이콘 등록
        for(int i = 0; i < skillCoolTimeList.Count; i++)
        {
            GameManager.Instance.playerManager.UpdateSkillUIImage(i, skillCoolTimeList[i].skill_Instance.backicon, skillCoolTimeList[i].skill_Instance.fillicon);
        }
    }

    //void SetSkillCoolTime()
    //{
    //    for(int i = 0; i < skillList.Count; i++)
    //    {
    //        SkillCoolTime cool = new SkillCoolTime(false, skillList[i].skillCoolTime);
    //        skillCoolTimeList.Add(cool);
    //    }
    //}
    #endregion

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
                    curSkill.gameObject.SetActive(true);

                    ResetDel resetDel = new ResetDel(ResetShuttle);
                    curSkill.ActivateShuttleSkill(resetDel);
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

    bool isRight = true;
    void RigidBodyMoveToPosition(Vector2 targetPos, out float distance)
    {
        Vector2 targetPosition = targetPos;

        // 현재 위치와 목표 위치 간의 거리
        distance = Vector2.Distance(transform.position, targetPosition) ;

        // 거리 기반으로 smoothTime 조절
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance / maxDistance);

        // 부드럽게 이동 (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));

        //이동 속도에 따라 view회전
        if(velocity.x > 0 && !isRight)
        {
            isRight = true;
            viewTr.localScale = new Vector3(viewScale.x, viewScale.y, viewScale.z);
        }
        else if(velocity.x < 0 && isRight)
        {
            isRight = false;
            viewTr.localScale = new Vector3(-viewScale.x, viewScale.y, viewScale.z); 
        }
    }

    #endregion

    private void Update()
    {
        //스킬별로 쿨타임을 카운트.
        if(skillCoolTimeList.Count > 0 )
        {
            for(int i = 0; i < skillCoolTimeList.Count; i++)
            {
                if (!skillCoolTimeList[i].isReady)
                {
                    skillCoolTimeList[i].CoolTimeWait();
                    GameManager.Instance.playerManager.UpdateSkillUICoolTime(i, skillCoolTimeList[i].GetCoolTimeFillAmount());
                }
            }
        }

        //스킬 사용 입력 감지 >> 스킬 사용을 위한 이동 시작.
        if (!shuttleActivate) return;

        if (!startSkill)
        {
            //1번
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (skillCoolTimeList.Count > 0 && skillCoolTimeList[0].isReady)
                {
                    curSkill = skillCoolTimeList[0].skill_Instance;
                    skillCoolTimeList[0].ResetReady();
                    GameManager.Instance.playerManager.UpdateSkillUICoolTime(0, skillCoolTimeList[0].GetCoolTimeFillAmount());
                    skillCoolTimeList[0].isUsing = true;
                }
            }
            //2번
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if(skillCoolTimeList.Count > 1 && skillCoolTimeList[1].isReady)
                {
                    curSkill = skillCoolTimeList[1].skill_Instance;
                    skillCoolTimeList[1].ResetReady();
                    GameManager.Instance.playerManager.UpdateSkillUICoolTime(1, skillCoolTimeList[1].GetCoolTimeFillAmount());

                }
            }
            //3번
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if(skillCoolTimeList.Count > 2 && skillCoolTimeList[2].isReady)
                {
                    curSkill = skillCoolTimeList[2].skill_Instance;
                    skillCoolTimeList[2].ResetReady();
                    GameManager.Instance.playerManager.UpdateSkillUICoolTime(2, skillCoolTimeList[2].GetCoolTimeFillAmount());

                }
            }

            //뭔가 선택된 스킬이 있으면 사용 시작.
            if (curSkill != null)
            {
                startSkill = true;
                physicsColl.enabled = false;
                ShuttleMoveTargetPos = GameManager.Instance.playerManager.playerBehavior.mousePos;
            }
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

    void ResetShuttle(int index)
    {
        curSkill = null;
        startSkill = false;
        useSkill = false;
        stopAttack = false;
        physicsColl.enabled = true;
    }


}

public delegate void ResetDel(int index);


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


[Serializable]
public class SkillCoolTime
{
    public ShuttleSkill skill_Instance;
    public bool isUsing;
    public bool isReady;
    float coolTime;
    float curTime;

    public SkillCoolTime(ShuttleSkill instance, float coolTime)
    {
        this .skill_Instance = instance;
        this.isUsing = false;
        this.isReady = false;
        this.coolTime = coolTime;
        this.curTime = 0;
    }

    public void CoolTimeWait()
    {
        if (!isReady)
        {
            curTime += Time.deltaTime;
            if (curTime >= coolTime) 
            {
                isReady = true;
                curTime = coolTime;
            }
        }
    }

    public void ResetReady()
    {
        isReady = false;
        curTime = 0;
    }

    public float GetCoolTimeFillAmount()
    {
        if(coolTime == 0)
        {
            return 0;
        }
        else return curTime / coolTime;
    }

}