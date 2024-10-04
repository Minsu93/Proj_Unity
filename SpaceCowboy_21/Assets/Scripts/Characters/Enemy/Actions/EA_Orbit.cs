using System.Collections;
using System.Threading;
using UnityEngine;

public class EA_Orbit : EnemyAction
{
    /// <summary>
    /// ��Ʈ����ũ ����϶� �༺�� �׳� �����ع�����. 
    /// ��ֹ��� ������ ������ �ٲ۴�. 
    /// </summary>
    /// 

    [Header("Orbit")]
    [SerializeField] float pauseTimer = 0.1f; //�ǰ� �� ���� �ð�

    [Header("KnockBack_Orbit")]
    [SerializeField] AnimationCurve knockBackCurve;
    [SerializeField] float knockbackTime = 1f;   //���ư��� �ð� 
    [SerializeField] float stopDuration = 1f;

    //����
    float pTime;

    //��ƼŬ
    [SerializeField] ParticleSystem boosterParticle;
    EnemyChase_Orbit chase_Orbit;


    //override �� �κ�

    protected override void Awake()
    {
        base.Awake();
        chase_Orbit = chase as EnemyChase_Orbit;
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
        DoAction();

        ///������ �κ�
        if (enemyState == EnemyState.Wait) return;

        if (onAttack)
        {
            if (attack != null)
                attack.OnAttackAction();
        }

        if (gravity.nearestPlanet == null) return;

        if (onChase)
        {
            if (chase_Orbit != null)
            {
                chase_Orbit.OnChaseAction();
            }
        }
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

    protected override void ActionByState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                StartIdleView();
                onChase = false;
                onAttack = false;
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

        AfterStrikeEvent();
    }

    public override void AfterStrikeEvent()
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

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        EnemyPause(knockbackTime + 0.3f);

        chase_Orbit.ResetCenterPoint();

        Vector2 dir = (Vector2)transform.position - hitPos;
        dir = dir.normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);

        StartCoroutine(StopRoutine(knockbackTime + 0.3f));
    }

    IEnumerator StopRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Planet nearestPlanet = gravity.nearestPlanet;
        while(nearestPlanet == null)
        {
            yield return null;
        }
        

        float timer = 0;
        Vector2 vel = rb.velocity;

        //�ӵ��� ���缭 �����մϴ�. �ð��� 1��.
        float secToStop = 1.0f;
        while(timer < secToStop)
        {
            timer += Time.deltaTime;
            rb.velocity = Vector2.Lerp(vel, Vector2.zero, knockBackCurve.Evaluate(timer));
            yield return null;
        }

        if(nearestPlanet != null)
        {
            chase_Orbit.SetCenterPoint(nearestPlanet);
        }
    }

    public override void WhenDieEvent()
    {
        base.WhenDieEvent();

        //������ �˵� ��ü �������� ������.
        //gravity.activate = true;

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
