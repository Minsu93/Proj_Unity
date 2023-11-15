using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class AimMousePosition : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Camera cam;

    [SpineBone(dataField: "skeletonAnimation")]
    public string boneName;

    public float maxDist = 1.0f;

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
        Vector3 inputPos = Input.mousePosition;
        inputPos.z = 10;

        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(inputPos);
        mouseWorldPosition.z = 0;
        Vector3 mouseVec = mouseWorldPosition - transform.position;
        Vector3 mousePos = transform.position + (mouseVec.normalized * maxDist);

        Vector3 skeletonLocalPosition = skeletonAnimation.transform.InverseTransformPoint(mousePos);
        skeletonLocalPosition.x *= skeletonAnimation.skeleton.ScaleX;
        skeletonLocalPosition.y *= skeletonAnimation.skeleton.ScaleY;
        bone.SetLocalPosition(skeletonLocalPosition);
        //bone.SetPositionSkeletonSpace(skeletonLocalPosition);

    }
}
