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
        //공격 쿨타임 시작
        //StartCoroutine(AttackCoolRoutine());

        //공격하는 동안 Enemy행동 정지
        //enemyAction.AfterAttack();
        enemyAction.EnemyForcePause();

        //실제 공격 루틴
        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        yield return null;
        /// 1. 플레이어 방향으로 회전
        /// 2. 뭔가에 부딪힐 때까지, 화면 밖으로 나갈 때까지, 혹은 정해진 시간이 끝날 때까지 돌진
        /// 3. 부딪히면 한동안 꼽혀있다(?) - 정지.
        /// 

        //1. 회전
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
        //잠깐 대기
        yield return new WaitForSeconds(0.2f);

        //2. 대시
        timer = 0;
        Vector2 startPos = rb.position;
        int targetLayer = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("StageObject");

        while (timer < dashDuration)
        {
            timer += Time.deltaTime;
            Vector2 movePos = startPos + (targetDir * dashSpeed * timer);
            rb.MovePosition(movePos);
            
            //뭔가에 부딪히면 정지한다
            Collider2D hit = Physics2D.OverlapCircle(transform.position, dashRadius, targetLayer);
            if (hit != null)
            {
                //플레이어인 경우
                if (hit.TryGetComponent<IEnemyHitable>(out IEnemyHitable e_hitable))
                {
                    e_hitable.DamageEvent(dashDamage, rb.position);
                }
                //플레이어가 아니면 움직임을 멈춘다.
                else break;

            }

            yield return null;
        }


        //잠깐 대기
        yield return new WaitForSeconds(afterAttackDelay);

        _attackCool = false;
        enemyAction.EnemyForceWake();
    }
}
