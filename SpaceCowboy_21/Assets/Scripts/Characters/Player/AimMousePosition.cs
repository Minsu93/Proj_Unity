using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class AimMousePosition : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;

    [SpineBone(dataField: "skeletonAnimation")]
    public string boneName;

    public bool activate;
    Vector3 mousePos;

    Bone bone;
    PlayerBehavior playerBehavior;
    

    // Start is called before the first frame update

    private void Awake()
    {
        if (skeletonAnimation == null)
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        playerBehavior = transform.parent.GetComponent<PlayerBehavior>();   
    }

    private void Start()
    {
        bone = skeletonAnimation.skeleton.FindBone(boneName);

        skeletonAnimation.UpdateLocal -= AimUpdate;
        skeletonAnimation.UpdateLocal += AimUpdate; //Spine 애니메이션에서 실행 순서 참고. 본 위치 업데이트를 어디쯤에서 할지 정하는 곳.

    }

    // Update is called once per frame
    void AimUpdate(ISkeletonAnimation anim)
    {
        if (!activate) return; 

        Vector3 inputPos = Input.mousePosition;
        inputPos.z = 10;    //z는 카메라에서부터의 거리
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //마우스 월드 위치
        mousePos.z = 0;

        Transform parentTr = transform.parent;
        Vector3 playerCenter = parentTr.position;
        Vector3 dir = (mousePos - playerCenter).normalized;
        float dist = (mousePos - playerCenter).magnitude;

        //playerBehavior 에 값 할당
        playerBehavior.mousePos = mousePos;
        playerBehavior.aimDirection = (Vector2)dir;
        playerBehavior.mouseDist = dist;

        //본 로컬 위치 조정
        Vector3 skeletonLocalPosition = transform.InverseTransformPoint(mousePos);

        skeletonLocalPosition.x *= skeletonAnimation.skeleton.ScaleX;
        skeletonLocalPosition.y *= skeletonAnimation.skeleton.ScaleY;
        bone.SetLocalPosition(skeletonLocalPosition);
    }
}
