using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack_Dash : EnemyAttack
{
    [SerializeField] float dashDamage = 5.0f;
    [SerializeField] float dashDuration = 3.0f;
    [SerializeField] float dashSpeed = 3.0f;
    [SerializeField] float dashRadius = 1.0f;
    Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnAttackAction()
    {
        if (_attackCool) return;

        _attackCool = true;
        //���� ��Ÿ�� ����
        //StartCoroutine(AttackCoolRoutine());

        //�����ϴ� ���� Enemy�ൿ ����
        //enemyAction.AfterAttack();
        enemyAction.EnemyForcePause();

        //���� ���� ��ƾ
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        yield return null;
        /// 1. �÷��̾� �������� ȸ��
        /// 2. ������ �ε��� ������, ȭ�� ������ ���� ������, Ȥ�� ������ �ð��� ���� ������ ����
        /// 3. �ε����� �ѵ��� �����ִ�(?) - ����.
        /// 

        //1. ȸ��
        float timer = 0;
        Vector2 targetDir = Vector2.zero;
        while(timer < preAttackDelay)
        {
            timer += Time.deltaTime;
            Vector2 dir = ((Vector2)GameManager.Instance.player.position - rb.position).normalized;
            Vector2 upVec = Quaternion.Euler(0, 0, 90) * dir;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, upVec);
            targetDir = dir;
            yield return null;
        }
        //��� ���
        yield return new WaitForSeconds(0.2f);

        //2. ���
        timer = 0;
        Vector2 startPos = rb.position;
        int targetLayer = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("StageObject");

        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            Vector2 movePos = startPos + (targetDir * dashSpeed * timer);
            rb.MovePosition(movePos);
            
            //������ �ε����� �����Ѵ�
            Collider2D hit = Physics2D.OverlapCircle(transform.position, dashRadius, targetLayer);
            if (hit != null)
            {
                //�÷��̾��� ���
                if (hit.TryGetComponent<IEnemyHitable>(out IEnemyHitable e_hitable))
                {
                    e_hitable.DamageEvent(dashDamage, rb.position);
                }
                //�÷��̾ �ƴϸ� �������� �����.
                else break;

            }

            yield return null;
        }


        //��� ���
        yield return new WaitForSeconds(afterAttackDelay);

        _attackCool = false;
        enemyAction.EnemyForceWake();
    }
}
