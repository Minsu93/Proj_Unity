using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    /// <summary>
    /// 신호(발사 위치, 발사 각도)만 오면 발사한다. 
    /// 
    /// 1. 공격 준비 (초기화)
    /// 2. 공격 쿨타임
    /// 3. 공격 행동
    /// </summary>
    /// 

    [Header("Common")]
    public float attackCoolTime = 3.0f;
    public float preAttackDelay = 0f;
    public float afterAttackDelay = 0f;

    protected bool _attackCool;
    public bool AttackCool { get { return _attackCool; } }

    //스크립트
    protected EnemyAction enemyAction;

    protected virtual void Awake()
    {
        enemyAction = GetComponent<EnemyAction>();
    }


    protected IEnumerator AttackCoolRoutine()
    {
        yield return new WaitForSeconds(attackCoolTime);

        _attackCool = false;
    }


    //공격 행동 
    public abstract void OnAttackAction();
    public virtual void StopAttackAction()
    {
        StopAllCoroutines();
    }
}
