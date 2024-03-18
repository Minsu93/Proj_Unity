using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EV_OrbitCannon : EnemyView
{
    public AnimationReferenceAsset startAttackMode;


    Skeleton skeleton;
    SkeletonData skeletonData;
    Slot slot;
    Skin curSkin;
    Bone bone;
    PointAttachment curPoint;
    [SpineSkin]
    public string skinName;
    [SpineBone]
    public string boneName;
    [SpineSlot]
    public string slotName;
    [SpineAttachment]
    public string attachmentName;

    protected override void Initialize()
    {
        //�ִϸ��̼� ����
        enemyAction.EnemyAttackTransitionEvent += AttackModeTransition;

        //GunTipPos ���� 
        skeleton = skeletonAnimation.skeleton;
        skeletonData = skeleton.Data;
        GetPoint();     //�ֳʹ��� ����Ʈ ����.
    }


    //����Ʈ ���� (Start���� �ѹ��� ����)
    void GetPoint()
    {
        curSkin = skeletonData.FindSkin(skinName);
        slot = skeleton.FindSlot(slotName);
        int slotIndex = skeletonData.FindSlot(slotName).Index;
        curPoint = curSkin.GetAttachment(slotIndex, attachmentName) as PointAttachment;
        bone = skeleton.FindBone(boneName);

    }

    //����Ʈ�� ��ġ ����
    public (Vector2, Quaternion) GetGunTipPos()
    {
        Vector2 Pos = Vector2.zero;
        Quaternion Rot = Quaternion.identity;


        if (curPoint != null)
        {
            Pos = curPoint.GetWorldPosition(slot, transform);
            Rot = Quaternion.Euler(0, 0, skeletonAnimation.transform.rotation.eulerAngles.z + curPoint.ComputeWorldRotation(bone));
        }

        return (Pos, Rot);
    }

    void AttackModeTransition()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, startAttackMode, false);
        skeletonAnimation.AnimationState.AddAnimation(0, run, true, 0);
    }
}
