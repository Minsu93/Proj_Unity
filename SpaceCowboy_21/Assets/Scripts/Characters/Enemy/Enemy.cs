using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool activate = true;
    public bool isGrabable = false;

    protected Health health;
    protected EnemyAction action;
    protected Collider2D coll;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
        health.ResetHealth();

        if (action == null)
            action = GetComponent<EnemyAction>();
    }


    protected virtual void Start()
    {
        coll = GetComponent<Collider2D>();

        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!activate)
            return;

    }




    public virtual void DamageEvent(float dmg)
    {
        //�������� ����
        if (health.AnyDamage(dmg))
        {
            //�ǰ� ����Ʈ�� ����
            action.HitEvent();
        }

        if (health.IsDead())
        {
            //true �� ��� ü���� 0 ���Ϸ� �������ٴ� ��.
            DeathEvent();
            return;
        }


    }

   
    public virtual void DeathEvent()
    {
        //���� ��Ȱ��ȭ �Ѵ�.

        action.DieEvent();
        coll.enabled = false;

    }

    public virtual void DeadActive()
    {
        //���� View���� ����� �״� �ִϸ��̼��� ������ ��ȣ�� �༭ ��Ȱ��ȭ ��Ŵ.
        this.gameObject.SetActive(false);
    }



    public virtual void PlayerIsDead()
    {
        //�÷��̾ ������ ��������Ʈ�� ���� �ߵ�

        activate = false;
    }
}

