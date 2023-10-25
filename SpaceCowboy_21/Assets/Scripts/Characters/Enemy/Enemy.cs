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
        //데미지를 적용
        if (health.AnyDamage(dmg))
        {
            //피격 이펙트를 적용
            action.HitEvent();
        }

        if (health.IsDead())
        {
            //true 인 경우 체력이 0 이하로 떨어졌다는 뜻.
            DeathEvent();
            return;
        }


    }

   
    public virtual void DeathEvent()
    {
        //적을 비활성화 한다.

        action.DieEvent();
        coll.enabled = false;

    }

    public virtual void DeadActive()
    {
        //구지 View에서 여기로 죽는 애니메이션이 끝나면 신호를 줘서 비활성화 시킴.
        this.gameObject.SetActive(false);
    }



    public virtual void PlayerIsDead()
    {
        //플레이어가 죽으면 델리케이트를 통해 발동

        activate = false;
    }
}

