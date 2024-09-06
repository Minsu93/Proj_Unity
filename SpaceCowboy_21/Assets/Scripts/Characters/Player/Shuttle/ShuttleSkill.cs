using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ��ų�� �θ� 
/// </summary>
public abstract class ShuttleSkill : MonoBehaviour, ITarget, IHitable, IKickable
{
    [Header("Icon")]
    public Sprite backicon;
    public Sprite fillicon;

    [Header("Shuttle Base")]
    public float energyUse = 5.0f;

    public float skillCoolTime = 5.0f; // ��Ʋ ��ų ���� ��� �ð�. ������ ����� ���� ���ĺ��� ���� ��� ������ ���� �����Ѵ�. 
    [SerializeField] float maxWaitTime = 10.0f;    // ��ȣ�ۿ� ���� ���� �� ������� ���ư��� �ð�
    float _waitTimer;
    [SerializeField] float maxUseTime = 5.0f; // ��ȣ�ۿ� �� �۵� �ð� 
    float _useTimer;

    bool activate;  //���� ���� 
    protected bool useStart;  //�����⸦ �°� �۵� ����
    [SerializeField] protected float sizeDefaultMultiplier = 2.0f;


    [SerializeField] CircleCollider2D kickedColl;
    [SerializeField] protected Collider2D projHitColl;
    protected Rigidbody2D rb;

    ResetDel resetShuttleDel;   //�ʱ�ȭ�뵵 
    int curIndex;

    //�ʱ� ���� �� 
    public void ShuttleSkillInitialize(int index)
    {
        curIndex = index;
        ResetSkill();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    //����
    public virtual void ActivateShuttleSkill(ResetDel del)
    {
        resetShuttleDel = del;
        kickedColl.enabled = true;
        projHitColl.enabled = true;
        this.transform.localScale = Vector3.one * sizeDefaultMultiplier;
        activate = true;
    }

    //�÷��̾� �ѿ� �¾��� ��
    public abstract void DamageEvent(float damage, Vector2 hitPoint);

    //�÷��̾� �����⿡ �¾��� �� 
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
    //���ð� ī��Ʈ
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

    //���ð� ī��Ʈ
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

    //���� ����, ��� ����
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
