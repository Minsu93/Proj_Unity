using SpaceEnemy;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCowboy
{
    public class PlayerView : MonoBehaviour
    {
        public PlayerBehavior playerBehavior;
        public SkeletonAnimation skeletonAnimation;

        public bool flipOn = true;
        bool turnRight = true;
        bool preTurnRight;
        bool _changeState;  //애니메이션 스테이트를 바꿉니다

        [Space]

        public AnimationReferenceAsset idle, onJump, onSpace, runForward, runBackward, shoot, aim, reload, die;

        PlayerState previousState;
        MeshRenderer _renderer;
        MaterialPropertyBlock block;

        [Space]

        [Header("Skins")]

        [SpineSkin] public string baseSkin;
        [SpineSkin] public string _1HandSkin;
        [SpineSkin] public string _2HandSkin;

        [SpineSkin] public string revolver;
        [SpineSkin] public string bubbleGun;
        [SpineSkin] public string tripleGun;

        Skin characterSkin;
        Skeleton skel;

        // Start is called before the first frame update
        void Start()
        {

            if (playerBehavior == null) return;
            playerBehavior.ShootEvent += PlayShoot;
            playerBehavior.StartAimEvent += PlayAim;
            playerBehavior.StopAimEvent += StopAim;
            playerBehavior.PlayerDieEvent += Dead;
            playerBehavior.PlayerReloadEvent += Reload;
            playerBehavior.PlayerStopReloadEvent += StopReload;
            PlayAim();

            playerBehavior.PlayerHitEvent += DamageHit;
            _renderer = GetComponent<MeshRenderer>();
            block = new MaterialPropertyBlock();
            _renderer.SetPropertyBlock(block);

            skel = skeletonAnimation.skeleton;

        }

        // Update is called once per frame
        void Update()
        {
            if (playerBehavior == null) return;
            if (skeletonAnimation == null) return;


            PlayerState state = playerBehavior.state;

            if (state == PlayerState.Die)
                return;

            if (state != previousState || _changeState)
            {
                _changeState = false;
                PlayStableAnimation();
            }
            previousState = state;

            //캐릭터 반전. 실제로 돌아가는게 아니라 view의 skeleton만 돌아간다.
            FlipScaleX();
        }

        void PlayStableAnimation()
        {   //state가 바뀔 때 마다 실행하는것. 
            PlayerState currentState = playerBehavior.state;
            Spine.Animation animation;
            switch (currentState)
            {
                case PlayerState.Idle:
                    animation = idle;

                    break;

                case PlayerState.Running:

                    if (playerBehavior.faceRight && Vector2.SignedAngle(transform.up, playerBehavior.aimDirection) < 0)
                    {
                        animation = runForward;
                    }
                    else
                    {
                        animation = runBackward;
                    }

                    break;

                case PlayerState.Jumping:
                    if (playerBehavior.onSpace)
                    {
                        animation = onSpace;
                    } 
                    else
                    {
                        animation = onJump;
                    }
                    break;

                default:
                    animation = idle;
                    break;
            }


            skeletonAnimation.AnimationState.SetAnimation(0, animation, true);
        }

        public void ChangeState()
        {
            _changeState = true;
        }

        public void PreShoot()
        {
            //자세 초기화
            skeletonAnimation.AnimationState.ClearTrack(1);
            skel.SetToSetupPose();

        }
        public void PlayShoot()
        {
            //skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);

            TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);
            //entry.AttachmentThreshold = 1;
            //entry.MixDuration = 0;
            //skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);


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


        public void DamageHit()
        {
            StartCoroutine(DamageRoutine());
        }

        IEnumerator DamageRoutine()
        {

            //int cID = Shader.PropertyToID("_Color");
            int id = Shader.PropertyToID("_Color");
            Color tColor = Color.clear;

            block.SetColor(id, Color.red);
            _renderer.SetPropertyBlock(block);

            yield return new WaitForSeconds(0.1f);

            //block.SetColor(cID, Color.black);
            block.SetColor(id, Color.white);
            _renderer.SetPropertyBlock(block);

            //피격 후 깜빡임
            for(int i = 0; i < 4; i ++)
            {
                yield return new WaitForSeconds(0.1f);
                block.SetColor(id, Color.clear);
                _renderer.SetPropertyBlock(block);

                yield return new WaitForSeconds(0.1f);

                block.SetColor(id, Color.white);
                _renderer.SetPropertyBlock(block);
            }
            

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

            /*
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
            */

        }

        public void Reload()
        {
            skeletonAnimation.AnimationState.SetAnimation(1, reload, false);
        }

        public void StopReload()
        {
            skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);
        }

        void FlipScaleX()
        {
            if (!flipOn) return;

            turnRight = Vector2.SignedAngle(transform.up, playerBehavior.aimDirection) < 0 ? true : false;
            if (turnRight != preTurnRight)
            {
                skeletonAnimation.skeleton.ScaleX = turnRight ? 1 : -1;
                preTurnRight = turnRight;
            }
        }



        public void ChangeWeapon(int index, bool oneHanded)
        {
            //스킨 변경
            Skeleton skeleton = skeletonAnimation.skeleton;
            SkeletonData skeletonData = skeleton.Data;

            characterSkin = new Skin(baseSkin);

            if (oneHanded)
            {
                characterSkin.AddSkin(skeletonData.FindSkin(_1HandSkin));
            }
            else
            {
                characterSkin.AddSkin(skeletonData.FindSkin(_2HandSkin));
            }

            switch (index)
            {
                case 0:
                    characterSkin.AddSkin(skeletonData.FindSkin(revolver));
                    break;

                case 1:
                    characterSkin.AddSkin(skeletonData.FindSkin(bubbleGun));
                    break;

                case 2:
                    characterSkin.AddSkin(skeletonData.FindSkin(tripleGun));
                    break;



                default:
                    characterSkin.AddSkin(skeletonData.FindSkin(revolver));
                    break;
            }


            skeleton.SetSkin(characterSkin);
            skeleton.SetSlotsToSetupPose();
        }
    }
}

