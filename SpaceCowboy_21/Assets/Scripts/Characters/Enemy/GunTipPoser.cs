using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTipPoser : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
   
    Slot slot;
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


    //포인트 감지 (Start에서 한번만 실행)
    void Start()
    {
        Skeleton skeleton = skeletonAnimation.skeleton;
        SkeletonData skeletonData = skeleton.Data;

        Skin curSkin = skeletonData.FindSkin(skinName);
        slot = skeleton.FindSlot(slotName);
        int slotIndex = skeletonData.FindSlot(slotName).Index;
        curPoint = curSkin.GetAttachment(slotIndex, attachmentName) as PointAttachment;
        bone = skeleton.FindBone(boneName);

    }

    //GunTip의 위치, 회전값 전달
    public (Vector2, Quaternion) GetGunTipPos()
    {
        Vector2 Pos = curPoint.GetWorldPosition(slot, transform);
        Quaternion Rot = Quaternion.Euler(0, 0, skeletonAnimation.transform.rotation.eulerAngles.z + curPoint.ComputeWorldRotation(bone));
        Vector2 direction = Rot * Vector2.right;  // Quaternion 회전을 사용해 방향 Vector 계산
        Vector2 endPos = Pos + ((Vector2)direction * 5f);  // 선의 종료 위치 계산
        Debug.DrawLine(Pos, endPos, Color.red, 3f);  // 선 그리기


        return (Pos, Rot);
    }

}
