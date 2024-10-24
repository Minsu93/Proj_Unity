using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Air : EnemyAction
{
    /// <summary>
    /// ���� ������ ��� �ൿ�� ���� �ʰ�, �÷��̾ �ٰ����� �� �����Ѵ�. 
    /// �˹� ��Ȳ�� ���� ����ϸ� ���� ��. 
    /// </summary>
    [Header("KnockBack on Air")] 
    [SerializeField] AnimationCurve knockBackCurve;
    EnemyChase_Air chase_Air;
    protected override void Awake()
    {
        base.Awake();
        chase_Air = chase as EnemyChase_Air;
    }

    public override void BrainStateChange()
    {
        switch (enemyState)
        {
            case EnemyState.Idle:
                //�÷��̾ ���� �ʾҴٸ�, �ٷ� �߰��Ѵ�.
                if (GameManager.Instance.playerIsAlive)
                    enemyState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (brain.inAttackRange && brain.isVisible)
                    enemyState = EnemyState.Attack;
                break;

            case EnemyState.Attack:
                if (!brain.inAttackRange || !brain.isVisible)
                    enemyState = EnemyState.Chase;

                break;
        }
    }


    protected override void ActionByState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                onChase = false;
                onAttack = false;
                StartIdleView();
                break;

            case EnemyState.Chase:
                onChase = true;
                onAttack = false;
                break;

            //�߰��� �κ� : ���� �� �̵�
            case EnemyState.Attack:
                onChase = false;
                onAttack = true;
                break;

        }
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        //strike ���̸� �����Ѵ�. 
        StopAllCoroutines();
        //EnemyPause(knockbackTime + 0.3f);
        onWait = true;

        //chase ����
        if (chase_Air != null)
        {
            chase_Air.StopSwim();
        }
        //attack ����
        attack.StopAttackAction();

        //�˹�
        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);

        StartCoroutine(StopRoutine(pauseTime));
    }

    IEnumerator StopRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        float timer = 0;
        Vector2 vel = rb.velocity;

        //�ӵ��� ���缭 �����մϴ�.
        while (timer < pauseTime)
        {
            timer += Time.deltaTime;
            rb.velocity = Vector2.Lerp(vel, Vector2.zero, knockBackCurve.Evaluate(timer));
            yield return null;
        }

        //�ٽ� �������� Ȱ��ȭ �մϴ�. 
        onWait = false;
    }

    protected override IEnumerator StrikeRoutine(Vector2 strikePos, Planet planet)
    {
        Vector2 startPos = transform.position;
        Vector2 normal = (startPos - strikePos).normalized;
        Vector2 strikeTargetPos = strikePos + (normal * (distanceFromStrikePoint + enemyHeight));

        float strikeTime = (startPos - strikeTargetPos).magnitude / strikeSpeed;
        float time = 0; //���� �ð�
        while (time < strikeTime)
        {
            time += Time.deltaTime;
            rb.MovePosition(Vector2.Lerp(startPos, strikeTargetPos, time / strikeTime));

            Debug.DrawLine(startPos, strikePos, Color.green);
            yield return null;
        }
        //�����ϸ� Ȱ�� ����. 
        yield return new WaitForSeconds(finishStrikeDelay);

        AfterStrikeEvent();

        //Vector2 startPos = transform.position;
        //float dist = (strikePos - startPos).magnitude;
        //float strikeTime = dist / strikeSpeed;
        //float time = 0;

        //while (time < strikeTime)
        //{
        //    time += Time.deltaTime;
        //    transform.position = (Vector2.Lerp(startPos, strikePos, time / strikeTime));

        //    yield return null;
        //}

        //yield return new WaitForSeconds(0.5f);

        //AfterStrikeEvent();
    }

    public override void AfterStrikeEvent()
    {
        enemyState = EnemyState.Chase;
        activate = true;
    }
}
