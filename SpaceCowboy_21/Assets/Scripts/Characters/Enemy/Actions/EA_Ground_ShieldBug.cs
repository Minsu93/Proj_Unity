using SpaceEnemy;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_ShieldBug : EA_Ground
{
    public float explodeRadius = 3f;
    public ParticleSystem explodeParticle;
    Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponentInChildren<Animator>();
    }
    public override void DoAction(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Ambush:
                AmbushStartEvent();
                break;

            case EnemyState.Idle:
                StartIdleView();
                break;

            case EnemyState.Chase:
                StopAllCoroutines();
                StartCoroutine(ChaseRepeater());
                break;

            case EnemyState.ToJump:
                StopAllCoroutines();
                StartCoroutine(StartToJump());
                break;

            case EnemyState.Attack:
                StopAllCoroutines();

                ShieldExplode();

                break;

            case EnemyState.Wait:
                //Wait���·� �Ѿ ���� �ϴ� ��� �ൿ�� ���߰� ������ �ִ´�. 
                StopAllCoroutines();
                break;

            case EnemyState.Die:
                StopAllCoroutines();
                DieView();

                ShieldExplode();

                //�Ѿ˿� �´� Enemy Collision ����
                hitCollObject.SetActive(false);
                break;
        }
    }

    void ShieldExplode()
    {
        //ĳ���� ��Ȱ��ȭ
        activate = false;
        StartCoroutine(ShieldExplodeRoutine());
    }

    IEnumerator ShieldExplodeRoutine()
    {
        //�ִϸ��̼� ���� 
        animator.SetTrigger("Explode");
        //1���� ����
        yield return new WaitForSeconds(1f);

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, explodeRadius, Vector2.right, 0f, LayerMask.GetMask("Player"));
        if(hit.collider != null)
        {
            hit.transform.GetComponent<PlayerHealth>().AnyDamage(99f);
        }

        ParticleHolder.instance.GetParticle(explodeParticle, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
