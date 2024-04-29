using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    /// <summary>
    /// ��ȣ(�߻� ��ġ, �߻� ����)�� ���� �߻��Ѵ�. 
    /// 
    /// 1. ���� �غ� (�ʱ�ȭ)
    /// 2. ���� ��Ÿ��
    /// 3. ���� �ൿ
    /// </summary>
    /// 

    [Header("Common")]
    public float attackCoolTime = 3.0f;
    public float preAttackDelay = 0f;
    public float afterAttackDelay = 0f;

    protected bool _attackCool;
    public bool AttackCool { get { return _attackCool; } }

    //��ũ��Ʈ
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


    //���� �ൿ 
    public abstract void OnAttackAction();
    public virtual void StopAttackAction()
    {
        StopAllCoroutines();
    }
}
