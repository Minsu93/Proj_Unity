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

        skeletonAnimation.UpdateLocal += AimUpdate; //Spine �ִϸ��̼ǿ��� ���� ���� ����. �� ��ġ ������Ʈ�� ����뿡�� ���� ���ϴ� ��.

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
        inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //���콺 ���� ��ġ
        mousePos.z = 0;


        //testObj.transform.localPosition = skeletonLocalPosition;
        //testObj.transform.position = mousePos;


        //playerBehavior �� �� �Ҵ�
        playerBehavior.mousePos = mousePos;

        Transform parentTr = transform.parent;
        Vector3 playerCenter = parentTr.position;
        Vector3 dir = (mousePos - playerCenter).normalized;
        playerBehavior.aimDirection = (Vector2)dir;

        Vector3 pos = playerCenter + (dir * maxDist);

        //�� ���� ��ġ ����
        Vector3 skeletonLocalPosition = transform.InverseTransformPoint(pos);

        skeletonLocalPosition.x *= skeletonAnimation.skeleton.ScaleX;
        skeletonLocalPosition.y *= skeletonAnimation.skeleton.ScaleY;
        bone.SetLocalPosition(skeletonLocalPosition);
        //bone.SetPositionSkeletonSpace(skeletonLocalPosition);

    }
}
