using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class AimPlayerPos : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    [SpineBone(dataField: "skeletonAnimation")]
    public string boneName;

    Bone bone;

    // Start is called before the first frame update
    void OnValidate()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        bone = skeletonAnimation.skeleton.FindBone(boneName);

        skeletonAnimation.UpdateLocal += AimUpdate; //Spine 애니메이션에서 실행 순서 참고. 본 위치 업데이트를 어디쯤에서 할지 정하는 곳.
    }

    // Update is called once per frame
    void AimUpdate(ISkeletonAnimation anim)
    {
        Vector3 playerPosition = GameManager.Instance.player.position;
        Vector3 skeletonLocalPosition = skeletonAnimation.transform.InverseTransformPoint(playerPosition);
        skeletonLocalPosition.x *= skeletonAnimation.skeleton.ScaleX;
        skeletonLocalPosition.y *= skeletonAnimation.skeleton.ScaleY;
        bone.SetLocalPosition(skeletonLocalPosition);
        //bone.SetPositionSkeletonSpace(skeletonLocalPosition);

    }
}
