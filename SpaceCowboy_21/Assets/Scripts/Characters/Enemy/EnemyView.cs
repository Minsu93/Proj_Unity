using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    public EnemyAction enemyAction;
    public SkeletonAnimation skeletonAnimation;


    bool preTurnRight;
    bool seePlayer;

    public AnimationReferenceAsset idle, run, attack, die, aimOn, strike;

    EnemyState previousEnemyState;

    MeshRenderer _renderer;
    MaterialPropertyBlock block;
    



    // Start is called before the first frame update
    void Start()
    {
        if (enemyAction == null) return;

        enemyAction.EnemyAttackEvent += PlayAttack;
        enemyAction.EnemyHitEvent += PlayDamaged;
        enemyAction.EnemyClearEvent += PlayStageClear;
        enemyAction.EnemyResetEvent += PlayReset;
        if(aimOn != null)
            enemyAction.EnemyAimOnEvent += PlayAimOn;
        if (aimOn != null)
            enemyAction.EnemyAimOffEvent += PlayAimOff;
        enemyAction.EnemySeeDirection += FlipScaleXToDirection;

        //상태 변경 시 상태에 따른 애니메이션
        enemyAction.EnemyChangeStateEvent += ChangeView;

        _renderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(block);

        preTurnRight = true; //시작시 오른쪽을 보고 있나요?

    }

    public void ChangeView(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                PlayIdle();
                break;
            case EnemyState.Chase:
                PlayRun();
                break;
            case EnemyState.Attack:
                PlayIdle();
                break;
            case EnemyState.Die:
                StopAllCoroutines();
                PlayDead();
                break;
            case EnemyState.Wait:
                break;
            case EnemyState.Strike:
                PlayStrike();
                break;
            case EnemyState.Groggy:
                PlayIdle();
                PlayAimOff();
                StopAllCoroutines();
                GroggyView();
                break;
        }
    }

    //루핑되는 애니메이션
    void PlayIdle()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
    }

    void PlayRun()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, run, true);
    }

    
    void PlayStrike()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, strike, true);
    }


    void PlayAttack()
    {
        TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(1, attack, false);
        skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);
    }

    void PlayDamaged()
    {
        StartCoroutine(DamageRoutine());
    }


    IEnumerator DamageRoutine()
    {
        int bID = Shader.PropertyToID("_Black");
        block.SetColor(bID, Color.white);
        _renderer.SetPropertyBlock(block);

        yield return new WaitForSeconds(0.1f);

        block.SetColor(bID, Color.black);
        _renderer.SetPropertyBlock(block);
    }


    void PlayDead()
    {
        StartCoroutine(DeadRoutine());
    }

    IEnumerator DeadRoutine()
    {
        skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);
        skeletonAnimation.AnimationState.AddEmptyAnimation(2, 0f, 0f);
        TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(0, die, false);

        yield return new WaitForSeconds(1f);
        //애니메이션이 끝나면

        StartCoroutine(BeingTransparentRoutine(1.0f));
    }

    void PlayStageClear()
    {
        StartCoroutine(BeingTransparentRoutine(3.0f));
    }

    //점차 투명해지는 루틴
    IEnumerator BeingTransparentRoutine(float duration)
    {
        float alpha = 1;
        int cID = Shader.PropertyToID("_Color");

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / duration;

            Color deadColor = new Color(1, 1, 1, alpha);
            block.SetColor(cID, deadColor);
            _renderer.SetPropertyBlock(block);

            yield return null;
        }
    }

    void GroggyView()
    {
        Debug.Log("Groggy Start");

        int bID = Shader.PropertyToID("_Black");
        block.SetColor(bID, Color.red);
        _renderer.SetPropertyBlock(block);
    }


    private void PlayReset()
    {
        //skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
        skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);

        int cID = Shader.PropertyToID("_Color");
        Color deadColor = new Color(1, 1, 1, 1);
        block.SetColor(cID, deadColor);
        int bID = Shader.PropertyToID("_Black");
        block.SetColor(bID, Color.black);
        _renderer.SetPropertyBlock(block);
    
    }

    void PlayAimOn()
    {
        skeletonAnimation.AnimationState.SetAnimation(2, aimOn, true);
    }
    void PlayAimOff()
    {
        skeletonAnimation.AnimationState.AddEmptyAnimation(2, 0f, 0f);
    }

    void FlipScaleXToDirection()
    {
        bool faceRight = enemyAction.faceRight;
        skeletonAnimation.skeleton.ScaleX = faceRight ? 1 : -1;
    }

}
