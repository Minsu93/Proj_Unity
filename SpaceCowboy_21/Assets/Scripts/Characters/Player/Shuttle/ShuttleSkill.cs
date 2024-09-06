using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 셔틀 스킬의 부모 
/// </summary>
public abstract class ShuttleSkill : MonoBehaviour, ITarget, IHitable, IKickable
{
    [Header("Icon")]
    public Sprite backicon;
    public Sprite fillicon;

    [Header("Shuttle Base")]
    public float energyUse = 5.0f;

    public float skillCoolTime = 5.0f; // 셔틀 스킬 재사용 대기 시간. 완전히 사용이 끝난 이후부터 재사용 대기 시작이 차기 시작한다. 
    [SerializeField] float maxWaitTime = 10.0f;    // 상호작용 하지 않을 시 원래대로 돌아가는 시간
    float _waitTimer;
    [SerializeField] float maxUseTime = 5.0f; // 상호작용 시 작동 시간 
    float _useTimer;

    bool activate;  //가동 시작 
    protected bool useStart;  //발차기를 맞고 작동 시작
    [SerializeField] protected float sizeDefaultMultiplier = 2.0f;


    [SerializeField] CircleCollider2D kickedColl;
    [SerializeField] protected Collider2D projHitColl;
    protected Rigidbody2D rb;

    ResetDel resetShuttleDel;   //초기화용도 
    int curIndex;

    //초기 생성 시 
    public void ShuttleSkillInitialize(int index)
    {
        curIndex = index;
        ResetSkill();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    //변신
    public virtual void ActivateShuttleSkill(ResetDel del)
    {
        resetShuttleDel = del;
        kickedColl.enabled = true;
        projHitColl.enabled = true;
        this.transform.localScale = Vector3.one * sizeDefaultMultiplier;
        activate = true;
    }

    //플레이어 총에 맞았을 때
    public abstract void DamageEvent(float damage, Vector2 hitPoint);

    //플레이어 발차기에 맞았을 때 
    public abstract void Kicked(Vector2 hitPos);



    protected virtual void Update()
    {
        if (!activate) return;

        if (!useStart)
        {
            CountWaitTimer();
        }
        else
        {
            CountUseTimer();
        }
    }

    #region Timer
    //대기시간 카운트
    void CountWaitTimer()
    {
        if(_waitTimer < maxWaitTime)
        {
            _waitTimer += Time.deltaTime;
        }
        else
        {
            CompleteMethod();
        }
    }

    //사용시간 카운트
    void CountUseTimer()
    {
        if (_useTimer < maxUseTime)
        {
            _useTimer += Time.deltaTime;
        }
        else
        {
            CompleteMethod();
        }
    }

    #endregion

    //변신 해제, 사용 종료
    void CompleteMethod()
    {
        CompleteEvent();
        ResetSkill();
        resetShuttleDel(curIndex);
        this.gameObject.SetActive(false);
    }

    public abstract void CompleteEvent();

    void ResetSkill()
    {
        this.transform.localScale = Vector3.one;
        kickedColl.enabled = false;
        projHitColl.enabled = false;

        _waitTimer = 0;
        _useTimer = 0;
        activate = false;
        useStart = false;
    }

    public Collider2D GetCollider()
    {
        return kickedColl;
    }

}
