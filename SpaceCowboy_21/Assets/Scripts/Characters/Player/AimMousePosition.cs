using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using SpaceCowboy;

public class AimMousePosition : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Camera cam;

    [SpineBone(dataField: "skeletonAnimation")]
    public string boneName;

    public float maxDist = 1.0f;

    Bone bone;

    //Debug test
    //GameObject testObj;
    //public Sprite testSpr;

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

        skeletonAnimation.UpdateLocal += AimUpdate; //Spine 애니메이션에서 실행 순서 참고. 본 위치 업데이트를 어디쯤에서 할지 정하는 곳.

        //testObj = new GameObject();
        //SpriteRenderer spr = testObj.AddComponent<SpriteRenderer>();
        //spr.sprite = testSpr;
        //spr.sortingLayerName = "Effect";

        //testObj.transform.parent = this.transform;
    }

    // Update is called once per frame
    void AimUpdate(ISkeletonAnimation anim)
    {
        Vector3 inputPos = Input.mousePosition;
        inputPos.z = 10;    //z는 카메라에서부터의 거리
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //마우스 월드 위치
        mousePos.z = 0;


        //testObj.transform.localPosition = skeletonLocalPosition;
        //testObj.transform.position = mousePos;


        //playerBehavior 에 값 할당
        playerBehavior.mousePos = mousePos;

        Transform parentTr = transform.parent;
        Vector3 playerCenter = parentTr.position;
        Vector3 dir = (mousePos - playerCenter).normalized;
        playerBehavior.aimDirection = (Vector2)dir;

        Vector3 pos = playerCenter + (dir * maxDist);

        //본 로컬 위치 조정
        Vector3 skeletonLocalPosition = transform.InverseTransformPoint(pos);

        skeletonLocalPosition.x *= skeletonAnimation.skeleton.ScaleX;
        skeletonLocalPosition.y *= skeletonAnimation.skeleton.ScaleY;
        bone.SetLocalPosition(skeletonLocalPosition);
        //bone.SetPositionSkeletonSpace(skeletonLocalPosition);

    }
}
