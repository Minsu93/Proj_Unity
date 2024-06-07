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



    protected override void Awake()
    {
        base.Awake();
        chase_Orbit = GetComponent<EnemyChase_Orbit>();
    }

    protected override bool BeforeUpdate()
    {
        if (!activate) return true;

        //���� �ð� �߰�
        if (pTime > 0)
        {
            pTime -= Time.deltaTime;
            return true;
        }

        return false;
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


    protected override void OnDieAction()
    {
        base.OnDieAction();

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

    //base�� �ǰݽ� �����ϴ� pTime �� �߰���.
    public override void DamageEvent(float damage, Vector2 hitVec)
    {
        if (enemyState == EnemyState.Die) return;

        if (health.AnyDamage(damage))
        {
            StartHitView();

            //�ǰݽ� ���� �߰�
            pTime = pauseTimer;

            if (hitEffect != null) GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);

            if (health.IsDead())
            {
                WhenDieEvent();

                GameManager.Instance.playerManager.ChargeFireworkEnergy();
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
        EnemyPause(1f);

        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;

        rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);
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
