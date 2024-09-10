using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    [Tooltip("0 에 가까울수록 빠르게 움직인다. 1에 가까울수록 느리게 움직인다. (-1 == Inverse, 0 == VecyClose, 1 == Very Far)")]
    [Range(-1f, 1f)]
    [SerializeField] private float speed = 0;
    
    Vector2 basePositionFromCamera;

    public event System.Func<Vector2> pMoveAction;

    private void Start()
    {
        basePositionFromCamera = transform.position;

        //자손 스프라이트들의 SortingOrder 조절
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
        ////렌즈에 따른 베이스 위치, 스케일 
        float curSize = GameManager.Instance.cameraManager.curLensSize;
        float defaultSize = GameManager.Instance.cameraManager.defaultLensSize;
        ratio = curSize / defaultSize;

        //거리에 따른 스케일링. 
        _scale = ratio * speed + (1 - speed);    // 0 ~ 1 >> 1 ~ ratio
        _pos = ratio * (1 - speed) + speed;  // 0 ~ 1 >> ratio ~ 1

        transform.localScale = Vector3.one * _scale;

        //스케일에 따른 기본 위치 
        Vector2 scaledBasePos = basePositionFromCamera * _scale;

        //카메라에 따른 변경된 위치, 카메라는 (0,0) 에서 시작되기 때문에 cameraPos는 카메라가 움직인 정도. 
        Vector3 cameraPos = Camera.main.transform.position;
        reducingRatio =  speed / ratio;
        //float reducingRatio = speed / _pos;
        Vector2 scaledCamMove = (Vector2)cameraPos * reducingRatio;

        //뭔지 모르지만...움직임 구현
        //pMoveAction()에서 시간에 따른 움직임 정도를 받아온다.
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
