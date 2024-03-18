using SpaceCowboy;
using SpaceEnemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Air : EnemyAction
{
    /// <summary>
    /// Air타입은 공중에 '정지' 해 있다가,  플레이어를 발견하면 공격하는 타입이다. 
    /// </summary>

    public float pauseTimer = 0.2f; //피격 시 정지 시간
    public float tackleDamage = 10f;    //태클 데미지
    float pTime;
    [SerializeField] AnimationCurve moveCurve;
    public float yMove = 2f;
    public float cycle = 2f;

    Quaternion moveRot;
    Vector2 startPos;

    float attimer;
    bool once = false;      //한대만 때리자.
    

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (!attackOn || once) return;

        if (CheckHitPlayer())
        {
            once = true;
            brain.playerTr.GetComponent<PlayerBehavior>().DamageEvent(tackleDamage);
        }
    }

    private void FixedUpdate()
    {
        if (pTime > 0)
        {
            pTime -= Time.deltaTime;
            return;
        }

        if(!attackOn) return;

        attimer += Time.deltaTime;

        Vector2 moveVec = new Vector2(moveSpeed * attimer, moveCurve.Evaluate((attimer/cycle) % 1f ) * yMove);
        moveVec = moveRot * moveVec;

        rb.MovePosition(startPos + moveVec);
    }

    public override void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Ambush:
                break;

            case EnemyState.Idle:
                StartIdleView();
                break;

            case EnemyState.Chase:
                break;

            case EnemyState.Attack:
                StopAllCoroutines();
                AttackAction();
                break;

            case EnemyState.Wait:
                break;

            case EnemyState.Die:

                StopAllCoroutines();
                attackOn = false;
                DieView();
                hitCollObject.SetActive(false);
                gameObject.SetActive(false);
                break;
        }
    }

    void AttackAction()
    {
        RotateToPlayer();
        attackOn = true;
        startPos = transform.position;
        Vector2 dir = Quaternion.Euler(0,0,90f) * (brain.playerTr.position - transform.position).normalized;
        moveRot = Quaternion.LookRotation(Vector3.forward, dir);
    }

    public override void HitView()
    {
        base.HitView();

        //약간 경직? 
        pTime = pauseTimer;
    }


    public override void AmbushEndEvent()
    {
        hitCollObject.SetActive(false);
    }


    bool CheckHitPlayer()
    {
        bool hitP = false;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHeight, Vector2.right, 0f, LayerMask.GetMask("Player"));
        if(hit.collider!= null)
        {
            hitP = true;
        }
        return hitP;
    }

    void RotateToPlayer()
    {
        Vector2 upVec = Quaternion.Euler(0, 0, 90f) * (brain.playerTr.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, upVec);
    }


}

