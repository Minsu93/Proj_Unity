using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class EA_Orbit : EnemyAction
{
    /// <summary>
    /// ��Ʈ����ũ ����϶� �༺�� �׳� �����ع�����. 
    /// ��ֹ��� ������ ������ �ٲ۴�. 
    /// </summary>
    /// 

    [Header("Orbit")]
    public float pauseTimer = 0.1f; //�ǰ� �� ���� �ð�

    //����
    float pTime;

    //��ƼŬ
    public ParticleSystem boosterParticle;

    EnemyChase_Orbit chase_Orbit;
    [SerializeField] AnimationCurve knockBackCurve;
    [SerializeField] float knockbackTime = 1f;   //���ư��� �ð� 

    //override �� �κ�

    protected override void Awake()
    {
        base.Awake();
        chase_Orbit = chase as EnemyChase_Orbit;
    }

    protected override bool BeforeUpdate()
    {
        if (!activate) return true;

        if (enemyState == EnemyState.Groggy) return true;
        else if (enemyState == EnemyState.Die) return true;

        //���� �ð� �߰�
        if (pTime > 0)
        {
            pTime -= Time.deltaTime;
            return true;
        }

        return false;
    }
    protected override void Update()
    {
        //Enemy�� �װų� Strike�� ������ ������ Update���� �ʴ´�. 
        if (BeforeUpdate()) return;

        //�극�ο��� �÷��̾� ���� ���� ������Ʈ
        brain.TotalCheck();

        //������Ʈ�� ���� enemyState ����. 
        BrainStateChange();

        //enemyState�� ���� Action ���� 
        EnemyStateChanged();

        ///������ �κ�
        if (enemyState == EnemyState.Wait) return;

        if (onAttack)
        {
            if (attack != null)
                attack.OnAttackAction();
        }

        if (onChase)
        {
            if (chase != null)
                chase.OnChaseAction();
        }
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


    protected override void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                onChase = false;
                onAttack = false;
                StartIdleView();
                break;

            case EnemyState.Chase:
                StartRunView();
                onChase = true;
                onAttack = false;
                break;

                //�߰��� �κ� : ���� �� �̵�
            case EnemyState.Attack:
                onChase = true;
                onAttack = true;
                break;

        }
    }

    
    public override void WakeUpEvent()
    {
        enemyState = EnemyState.Chase;
        activate = true;

        //�߷� Ȱ��ȭ ����
        //gravity.activate = true;

        //���� ���� �߰�
        ChangeDirection(brain.playerIsRight);

        //��ƼŬ ���� �߰�
        if (boosterParticle != null) boosterParticle.Play();

    }

    //base�� �ǰݽ� �����ϴ� pTime �� �߰���.
    public override void DamageEvent(float damage, Vector2 hitVec)
    {
        if (enemyState == EnemyState.Die) return;

        if (health.AnyDamage(damage))
        {

            //�ǰݽ� ���� �߰�
            pTime = pauseTimer;


            if (health.IsDead())
            {
                //�׷α� ����
                if (UnityEngine.Random.Range(0, 1f) < groggyChance)
                {
                    StartCoroutine(GroggyEvent());
                }
                else
                    WhenDieEvent();
            }
            else
            {
                StartHitView();
                if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

            }

        }
    }

    //�༺ ���� �� �༺ center�� �������� ���� �߰���. 
    protected override IEnumerator StrikeRoutine(Vector2 strikePos)
    {
        Vector2 dir = strikePos - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, enemyHeight, dir.normalized, float.MaxValue, LayerMask.GetMask("Planet"));
        if (hit.collider == null) yield break;

        //�߰��� �κ�
        Planet planet = hit.collider.GetComponent<Planet>();

        Vector2 strikeStartPos = transform.position;
        Vector2 strikeTargetPos = hit.point;
        float strikeTime = hit.distance / strikeSpeed;
        float time = 0; 
        float distance = hit.distance; 
        while (distance > distanceFromStrikePoint + enemyHeight)
        {
            time += Time.deltaTime;
            distance = Vector2.Distance(transform.position, strikeTargetPos);
            transform.position = (Vector2.Lerp(strikeStartPos, strikeTargetPos, time / strikeTime));

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        //�߰��� �κ�
        chase_Orbit.SetCenterPoint(planet);

        WakeUpEvent();
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        EnemyPause(knockbackTime + 0.3f);

        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;
        Vector2 startPos = (Vector2)transform.position;
        Vector2 targetPos = (Vector2)transform.position + (dir* forceAmount);
        StartCoroutine(KnockBackRoutine(startPos, targetPos));
        
        //rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);
    }
    IEnumerator KnockBackRoutine(Vector2 start, Vector2 end)
    {
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / knockbackTime;
            rb.MovePosition(Vector2.Lerp(start, end, knockBackCurve.Evaluate(time)));
            yield return null;
        }
        chase_Orbit.ResetCenterPoint();
    }

    public override void WhenDieEvent()
    {
        base.WhenDieEvent();

        //������ �˵� ��ü �������� ������.
        gravity.activate = true;

        //��ƼŬ �߰�
        if (boosterParticle != null) boosterParticle.Stop();
    }

    void ChangeDirection()
    {
        faceRight = !faceRight;
        FlipToDirectionView();
        chase_Orbit.ChangeDirection(faceRight);
    }

    void ChangeDirection(bool isRight)
    {
        faceRight = isRight;
        FlipToDirectionView();
        chase_Orbit.ChangeDirection(faceRight);
    }


    //�༺�� �ε����� ���� ��ȯ. 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Planet"))
        {
            ChangeDirection();
        }
    }
}
