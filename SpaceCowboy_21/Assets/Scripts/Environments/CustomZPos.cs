using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomZPos : MonoBehaviour
{

    public float customZPos = 10f;
    float baseZPos = 10f;
    public CinemachineVirtualCamera virtualCam;

    public float movementMultiplierBase = 0.5f;
    float defaultLensSize;
    Transform cameraTr;
    Vector3 startScale;
    Vector2 startPos;
    Vector2 startCamPos;

    void Start()
    {
        cameraTr = Camera.main.transform;
        startScale = transform.localScale;

        startCamPos = cameraTr.position;
        defaultLensSize = GameManager.Instance.defaultLens;

        //시작 위치 기준 조절
        startPos = transform.position;
        //Vector2 p = startCamPos - startPos;
        //startPos += p  * (1 - (baseZPos / customZPos));

        //transform.position = startPos;

    }

    void LateUpdate()
    {
        ////렌즈에 따른 베이스 위치, 스케일 
        float lens = virtualCam.m_Lens.OrthographicSize;
        float baseScaler;
        //Vector2 basePos;

        Vector2 camPos = cameraTr.position;

        //Vector2 v = camPos - startPos;
        //v *= ( 1 - defaultLensSize / lens);
        //basePos = startPos + v;

        baseScaler = defaultLensSize / lens; // 1 ~ 0.5의 값
        float a = 1 / baseScaler;      //a는 1~2의 값을 갖는다. 
        float w = customZPos / 1000f;   //w 는 0~1의 값을 갖는다.
        float editScaler = (1 - w) + (a * w);   //w 가 0일때 결과값은 1, w가 1일때 결과값은 a
        transform.localScale = startScale * editScaler;

        ////움직임 : start위치 기준
        //Vector2 deltaMovement = camPos - startCamPos;
        //basePos += deltaMovement * (1 - (baseZPos / customZPos) * (defaultLensSize / lens));

        //float f = baseZPos / customZPos;


        ////최종 위치와 스케일
        //transform.position = basePos;
        ////transform.localScale = startScale * (1 - (baseScaler * f));


        //시작 위치를 잡는다.

        Vector2 basePos;

        //시작 위치는 내와 그 행성까지 걸리는 거리만큼 미리 마중나와 있는 것. 시야각 포함. 
        Vector2 v = startCamPos - startPos;
        v *= (1 - (baseZPos / customZPos) * (defaultLensSize / lens));
        basePos = startPos + v;


        //움직이게 만든다
        Vector2 t = camPos - startCamPos;
        t *= (1 - (baseZPos / customZPos) * (defaultLensSize / lens));
        transform.position = basePos + t;

    }
}
