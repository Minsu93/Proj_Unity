using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    [Tooltip("0 �� �������� ������ �����δ�. 1�� �������� ������ �����δ�. (-1 == Inverse, 0 == VecyClose, 1 == Very Far)")]
    [Range(-1f, 1f)]
    [SerializeField] private float speed = 0;
    
    Vector2 basePositionFromCamera;

    private void Start()
    {
        basePositionFromCamera = transform.position;

        //�ڼ� ��������Ʈ���� SortingOrder ����
        SpriteRenderer[] sprs = transform.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0;  i < sprs.Length; i++)
        {
            sprs[i].sortingOrder = Mathf.FloorToInt(speed * -100);
        }
    }


    private void FixedUpdate()
    {
        ////��� ���� ���̽� ��ġ, ������ 
        float curSize = GameManager.Instance.cameraManager.curLensSize;
        float defaultSize = GameManager.Instance.cameraManager.defaultLensSize;
        float ratio = curSize / defaultSize;

        //�Ÿ��� ���� �����ϸ�. 
        float pos = ratio * speed + (1 - speed);    // 0 ~ 1 >> 1 ~ ratio
        float neg = ratio * (1 - speed) + speed;  // 0 ~ 1 >> ratio ~ 1

        transform.localScale = Vector3.one * pos;

        //�⺻ ��ġ 
        Vector2 scaledBasePos = basePositionFromCamera * pos;

        //ī�޶� ���� ����� ��ġ, ī�޶�� (0,0) ���� ���۵Ǳ� ������ cameraPos�� ī�޶� ������ ����. 
        Vector3 cameraPos = Camera.main.transform.position;
        Vector2 baseCamMovement = (Vector2)cameraPos * speed;
        Vector2 scaledCamMove = baseCamMovement / neg;

        transform.position = scaledBasePos + scaledCamMove;
    }
}
