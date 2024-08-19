using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ��ų�� �θ� 
/// </summary>
public abstract class ShuttleSkill : MonoBehaviour, ITarget, IHitable, IKickable
{
    public float skillCoolTime = 5.0f; // ��Ʋ ��ų ���� ��� �ð�. ������ ����� ���� ���ĺ��� ���� ��� ������ ���� �����Ѵ�. 
    [SerializeField] float maxWaitTime = 10.0f;    // ��ȣ�ۿ� ���� ���� �� ������� ���ư��� �ð�
    float _waitTimer;
    [SerializeField] float maxUseTime = 5.0f; // ��ȣ�ۿ� �� �۵� �ð� 
    float _useTimer;
    bool activate;  //���� ���� 
    protected bool useStart;  //�����⸦ �°� �۵� ����
    
    [SerializeField] CircleCollider2D coll;
    ResetDel resetShuttleDel;   //�ʱ�ȭ�뵵 
    [SerializeField] protected Rigidbody2D rb;

    //�ʱ� ���� �� 
    public void ShuttleSkillInitialize()
    {
        ResetSkill();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    //����
    public void ActivateShuttleSkill(ResetDel del)
    {
        resetShuttleDel = del;
        coll.enabled = true;
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
        resetShuttleDel();
        this.gameObject.SetActive(false);
    }

    public abstract void CompleteEvent();

    void ResetSkill()
    {
        coll.enabled = false;
        _waitTimer = 0;
        _useTimer = 0;
        activate = false;
        useStart = false;
    }

    public Collider2D GetCollider()
    {
        return coll;
    }

}
