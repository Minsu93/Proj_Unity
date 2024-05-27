using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Security.Cryptography;
using SpaceCowboy;
using System.Reflection;

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
        if (attack != null)
        {
            enemyAction.EnemyAttackEvent += PlayAttack;
        }     
        if( die != null)
        {
            enemyAction.EnemyDieEvent += Dead;
        }
        enemyAction.EnemyStartIdle += StartIdle;
        enemyAction.EnemyStartRun += StartRun;
        enemyAction.EnemyHitEvent += DamageHit;
        enemyAction.EnemyAimOnEvent += AimOn;
        enemyAction.EnemyAimOffEvent += AimOff;
        enemyAction.EnemySeeDirection += FlipScaleXToDirection;
        enemyAction.EnemyResetEvent += ResetEvent;
        enemyAction.EnemyStrikeEvent += StartStrike;
        enemyAction.EnemyClearEvent += StartClear;

        _renderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(block);

        preTurnRight = true; //시작시 오른쪽을 보고 있나요?

    }



    void StartRun()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, run, true);
    }

    void StartIdle()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
        
        //int cID = Shader.PropertyToID("_Color");
        //Color deadColor = Color.white;
        //block.SetColor(cID, deadColor);
        //_renderer.SetPropertyBlock(block);
    }
    
    void StartStrike()
    {
        if(strike != null)
            skeletonAnimation.AnimationState.SetAnimation(0, strike, true);
    }

    void StartClear()
    {
        StartCoroutine(BeingTransparentRoutine(3.0f));
    }


    void PlayAttack()
    {
        //skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);

        TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(1, attack, false);
        skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);
    }

    void DamageHit()
    {
        StartCoroutine(DamageRoutine());
    }


    IEnumerator DamageRoutine()
    {
        //if(hit != null)
        //    skeletonAnimation.AnimationState.SetAnimation(0, hit, false);

        //int cID = Shader.PropertyToID("_Color");
        int bID = Shader.PropertyToID("_Black");
        block.SetColor(bID, Color.white);
        _renderer.SetPropertyBlock(block);

        yield return new WaitForSeconds(0.1f);

        block.SetColor(bID, Color.black);
        _renderer.SetPropertyBlock(block);
    }


    void Dead()
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

    private void ResetEvent()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, idle, true);

        int cID = Shader.PropertyToID("_Color");
        Color deadColor = new Color(1, 1, 1, 1);
        block.SetColor(cID, deadColor);
        _renderer.SetPropertyBlock(block);
    
    }

    void AimOn()
    {
        skeletonAnimation.AnimationState.SetAnimation(2, aimOn, true);
    }
    void AimOff()
    {
        skeletonAnimation.AnimationState.AddEmptyAnimation(2, 0f, 0f);
    }

    void FlipScaleXToDirection()
    {
        // 적이 움직여야 할 방향은 어디인가
        // 해당 방향으로 고개를 틀어라. 
        bool faceRight = enemyAction.faceRight;
        skeletonAnimation.skeleton.ScaleX = faceRight ? 1 : -1;
    }

}
