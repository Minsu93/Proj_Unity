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
        skeletonAnimation.UpdateLocal += AimUpdate; //Spine �ִϸ��̼ǿ��� ���� ���� ����. �� ��ġ ������Ʈ�� ����뿡�� ���� ���ϴ� ��.

    }

    // Update is called once per frame
    void AimUpdate(ISkeletonAnimation anim)
    {
        if (!activate) return; 

        Vector3 inputPos = Input.mousePosition;
        inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //���콺 ���� ��ġ
        mousePos.z = 0;

        Transform parentTr = transform.parent;
        Vector3 playerCenter = parentTr.position;
        Vector3 dir = (mousePos - playerCenter).normalized;
        float dist = (mousePos - playerCenter).magnitude;

        //playerBehavior �� �� �Ҵ�
        playerBehavior.mousePos = mousePos;
        playerBehavior.aimDirection = (Vector2)dir;
        playerBehavior.mouseDist = dist;

        //�� ���� ��ġ ����
        Vector3 skeletonLocalPosition = transform.InverseTransformPoint(mousePos);

        skeletonLocalPosition.x *= skeletonAnimation.skeleton.ScaleX;
        skeletonLocalPosition.y *= skeletonAnimation.skeleton.ScaleY;
        bone.SetLocalPosition(skeletonLocalPosition);
    }
}
