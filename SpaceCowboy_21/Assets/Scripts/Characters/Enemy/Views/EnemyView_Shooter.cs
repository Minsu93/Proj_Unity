using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView_Shooter : EnemyView
{
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
        //GunTipPos 관련 
        skeleton = skeletonAnimation.skeleton;
        skeletonData = skeleton.Data;
        GetPoint();     //애너미의 포인트 감지.
    }


    //포인트 감지 (Start에서 한번만 실행)
    void GetPoint()
    {
        curSkin = skeletonData.FindSkin(skinName);
        slot = skeleton.FindSlot(slotName);
        int slotIndex = skeletonData.FindSlot(slotName).Index;
        curPoint = curSkin.GetAttachment(slotIndex, attachmentName) as PointAttachment;
        bone = skeleton.FindBone(boneName);

    }

    //GunTip의 위치, 회전값 전달
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

}
