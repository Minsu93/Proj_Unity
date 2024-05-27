using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
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
        int preAimInt = -1;

        [Space]

        public AnimationReferenceAsset idle, jumpStart, onJump, runForward, runBackward, shoot, slide, die;

        PlayerState previousState;
        int previousFaceRight;
        MeshRenderer _renderer;
        MaterialPropertyBlock block;


        Skeleton skeleton;
        SkeletonData skeletonData;

        Skin curSkin;

        Bone bone;
        Slot slot;
        PointAttachment curPoint;
        Vector2 pointVec;


        //임시
        public GunType gunType;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            block = new MaterialPropertyBlock();
            _renderer.SetPropertyBlock(block);

            skeleton = skeletonAnimation.skeleton;
            skeletonData = skeleton.Data;

        }

        // Start is called before the first frame update
        void Start()
        {
            if (playerBehavior == null) return;

            playerBehavior.ShootEvent += PlayShoot;
            playerBehavior.PlayerDieEvent += Dead;
            playerBehavior.PlayerHitEvent += DamageHit;
            playerBehavior.PlayerJumpStartEvent += PlayJumpStart;
            playerBehavior.PlayerJumpEvent += PlayJump;
           

            //SetSkin(gunType);
            //Debug.Log(gunType.skinName + "," + gunType.boneName + "," + gunType.slotName + "," + gunType.attachmentName);
        }

        // Update is called once per frame
        void Update()
        {
            if (playerBehavior == null) return;
            if (!playerBehavior.activate) return;
            if (skeletonAnimation == null) return;


            PlayerState state = playerBehavior.playerState;


            //state가 Running 이 될 때 
            if(state == PlayerState.Running)
            {
                int aimInt;

                if (playerBehavior.faceRight == playerBehavior.aimRight) aimInt = 1;
                else aimInt = -1;

                if(aimInt != preAimInt)
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, aimInt == -1 ? runForward : runBackward, true);

                    preAimInt = aimInt;
                }

            }

            if(state != previousState)
            {
                switch (state)
                {
                    case PlayerState.Idle:
                        skeletonAnimation.AnimationState.SetAnimation(0, idle, true);

                        //러닝 변수 초기화
                        preAimInt = 0;

                        break;

                    case PlayerState.Sliding:
                        skeletonAnimation.AnimationState.SetAnimation(0, slide, true);

                        break;
                }

                previousState = state;
            }

            //캐릭터 반전. 실제로 돌아가는게 아니라 view의 skeleton만 돌아간다.
            if (state != PlayerState.Sliding)
                FlipScaleX();

            GetGunTipPos();
        }



        public void PlayJumpStart()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, jumpStart, false);
        }

        public void PlayJump()
        {
            skeletonAnimation.AnimationState.SetAnimation(0, onJump, true);

        }


        public void PreShoot()
        {
            //자세 초기화
            skeletonAnimation.AnimationState.ClearTrack(1);
            skeleton.SetToSetupPose();

        }
        public void PlayShoot()
        {
            TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(1, shoot, false);
            //entry.AttachmentThreshold = 1;
            //entry.MixDuration = 0;
            //skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);

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
        }


        public void Dead()
        {
            skeletonAnimation.AnimationState.AddEmptyAnimation(1, 0f, 0f);
            //skeletonAnimation.AnimationState.AddEmptyAnimation(2, 0f, 0f);
            TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(0, die, false);
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



        void GetGunTipPos()
        {
            if (curPoint != null)
            {
                pointVec = curPoint.GetWorldPosition(slot, transform);

                //float rotation2D = curPoint.ComputeWorldRotation(slot.Bone) + skeletonAnimation.transform.rotation.eulerAngles.z;

                playerBehavior.gunTipPos = pointVec;
            }
        }


        public void SetSkin(GunType gunType)
        {
            curSkin = skeletonData.FindSkin(gunType.skinName);
            skeleton.SetSkin(curSkin);
            skeletonAnimation.Skeleton.SetSlotsToSetupPose();
            skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);

            GetPoint(gunType);
        }

        void GetPoint(GunType gunType)
        {
            slot = skeleton.FindSlot(gunType.slotName);
            int slotIndex = skeletonData.FindSlot(gunType.slotName).Index;
            curPoint = curSkin.GetAttachment(slotIndex, gunType.attachmentName) as PointAttachment;
            bone = skeleton.FindBone(gunType.boneName);
        }


    }

    [System.Serializable]
    public class GunType
    {
        public string skinName;

        public string boneName;

        public string slotName;

        public string attachmentName;

    }
}

