using SpaceEnemy;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Security.Cryptography;

public class EnemyView : MonoBehaviour
{
    public EnemyAction enemyAction;
    public EnemyBrain enemyBrain;
    public SkeletonAnimation skeletonAnimation;


    bool preTurnRight;

    public AnimationReferenceAsset idle, chase, shoot, aim, guard, die;
    EnemyState previousEnemyState;

    MeshRenderer _renderer;
    MaterialPropertyBlock block;

    // Start is called before the first frame update
    void Start()
    {

        if (enemyAction == null) return;
        if (shoot != null)
        {
            enemyAction.EnemyShootEvent += PlayShoot;
        }     
        if(aim != null)
        {
            enemyAction.EnemyStartAimEvent += PlayAim;
            enemyAction.EnemyStopAImEvent += StopAim;
        }
        if (guard != null)
        {
            enemyAction.EnemyStartGuardEvent += StartGuard;
            enemyAction.EnemyStopGuardEvent += StopGuard;
        }
        if( die != null)
        {
            enemyAction.EnemyDieEvent += Dead;
        }


        enemyAction.EnemyHitEvent += DamageHit;
        _renderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(block);

        preTurnRight = true; //시작시 오른쪽을 보고 있나요?

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyAction == null) return;
        if (skeletonAnimation == null) return;
        if (enemyBrain.enemyState == EnemyState.Die)
            return;

        EnemyState state = enemyBrain.enemyState;

        if (state != previousEnemyState)
        {
            PlayStableAnimation();
        }
        previousEnemyState = state;


        if (enemyBrain.filpToPlayerOn)
        {
            //캐릭터 반전. 실제로 돌아가는게 아니라 view의 skeleton만 돌아간다.
            if (enemyBrain.enemyState == EnemyState.Chase || enemyBrain.enemyState == EnemyState.Attack)
            {
                FlipScaleX();
            }
        }
    }

    void PlayStableAnimation()
    {

        EnemyState currentState = enemyBrain.enemyState;
        Spine.Animation animation;

        if (currentState == EnemyState.Chase)
        {
            animation = chase;

        }
        else
        {
            animation = idle;
        }

        skeletonAnimation.AnimationState.SetAnimation(0, animation, true);
    }

    public void PlayShoot()
    {
        //skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);

        TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);
        entry.AttachmentThreshold = 1;
        entry.MixDuration = 0;
        skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);


    }

    public void PlayAim()
    {
        TrackEntry aimTrack = skeletonAnimation.AnimationState.SetAnimation(2, aim, true);
        //aimTrack.AttachmentThreshold = 1;
        //aimTrack.MixDuration = 0;
    }

    public void StopAim()
    {
        skeletonAnimation.AnimationState.AddEmptyAnimation(2, 0.5f, 0.1f);
    }

    public void StartGuard()
    {
        TrackEntry aimTrack = skeletonAnimation.AnimationState.SetAnimation(2, guard, true);

    }

    public void StopGuard()
    {
        skeletonAnimation.AnimationState.AddEmptyAnimation(2, 0.5f, 0.1f);

    }


    public void DamageHit()
    {
        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        //int cID = Shader.PropertyToID("_Color");
        int bID = Shader.PropertyToID("_Black");
        block.SetColor(bID, Color.white);
        _renderer.SetPropertyBlock(block);

        yield return new WaitForSeconds(0.1f);

        //block.SetColor(cID, Color.black);
        block.SetColor(bID, Color.black);
        _renderer.SetPropertyBlock(block);

        //yield return new WaitForSeconds(0.04f);

        //block.SetColor(cID, Color.white);
        //_renderer.SetPropertyBlock(block);


    }


    public void Dead()
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

        float alpha = 1;
        int cID = Shader.PropertyToID("_Color");

        while (alpha > 0)
        {
            alpha -= Time.deltaTime;

            Color deadColor = new Color(1, 1, 1, alpha);
            block.SetColor(cID, deadColor);
            _renderer.SetPropertyBlock(block);

            yield return null;
        }

        //이 오브젝트를 비활성화
        enemyBrain.DeadActive();
    }

    void FlipScaleX()
    {
        Vector3 dir = (enemyBrain.playerTr.position - transform.position).normalized; 
        bool turnRight = Vector2.SignedAngle(transform.up, dir) < 0 ? true : false;
        if (turnRight != preTurnRight)
        {
            skeletonAnimation.skeleton.ScaleX = turnRight ? 1 : -1;
            preTurnRight = turnRight;
        }
    }
}
