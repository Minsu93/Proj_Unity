using Cinemachine;
using System;
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

    public event System.Func<Vector2> pMoveAction;

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
    [SerializeField] float ratio;
    [SerializeField] float _scale;
    [SerializeField] float _pos;
    [SerializeField] float reducingRatio;
    [SerializeField] Vector2 moveVec = Vector2.zero;
    [SerializeField] float r;
    private void FixedUpdate()
    {
        ////��� ���� ���̽� ��ġ, ������ 
        float curSize = GameManager.Instance.cameraManager.curLensSize;
        float defaultSize = GameManager.Instance.cameraManager.defaultLensSize;
        ratio = curSize / defaultSize;

        //�Ÿ��� ���� �����ϸ�. 
        _scale = ratio * speed + (1 - speed);    // 0 ~ 1 >> 1 ~ ratio
        _pos = ratio * (1 - speed) + speed;  // 0 ~ 1 >> ratio ~ 1

        transform.localScale = Vector3.one * _scale;

        //�����Ͽ� ���� �⺻ ��ġ 
        Vector2 scaledBasePos = basePositionFromCamera * _scale;

        //ī�޶� ���� ����� ��ġ, ī�޶�� (0,0) ���� ���۵Ǳ� ������ cameraPos�� ī�޶� ������ ����. 
        Vector3 cameraPos = Camera.main.transform.position;
        reducingRatio =  speed / ratio;
        //float reducingRatio = speed / _pos;
        Vector2 scaledCamMove = (Vector2)cameraPos * reducingRatio;

        //���� ������...������ ����
        //pMoveAction()���� �ð��� ���� ������ ������ �޾ƿ´�.
        if (pMoveAction != null)
        {
            r = (1-speed) * _scale;
            //float r = (1 - speed) / _scale;
            //r = 1 - (speed / _scale);
            moveVec = pMoveAction() * r;
        }

        transform.position = scaledBasePos + moveVec + scaledCamMove;


    }
}
