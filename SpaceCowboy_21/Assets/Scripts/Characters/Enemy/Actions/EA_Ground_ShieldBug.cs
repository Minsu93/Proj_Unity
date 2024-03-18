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
                //Wait상태로 넘어갈 때는 하던 모든 행동을 멈추고 가만히 있는다. 
                StopAllCoroutines();
                break;

            case EnemyState.Die:
                StopAllCoroutines();
                DieView();

                ShieldExplode();

                //총알에 맞는 Enemy Collision 해제
                hitCollObject.SetActive(false);
                break;
        }
    }

    void ShieldExplode()
    {
        //캐릭터 비활성화
        activate = false;
        StartCoroutine(ShieldExplodeRoutine());
    }

    IEnumerator ShieldExplodeRoutine()
    {
        //애니메이션 실행 
        animator.SetTrigger("Explode");
        //1초후 폭발
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
