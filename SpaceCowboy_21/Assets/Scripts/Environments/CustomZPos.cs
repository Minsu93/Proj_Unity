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

        //���� ��ġ ���� ����
        startPos = transform.position;
        //Vector2 p = startCamPos - startPos;
        //startPos += p  * (1 - (baseZPos / customZPos));

        //transform.position = startPos;

    }

    void LateUpdate()
    {
        ////��� ���� ���̽� ��ġ, ������ 
        float lens = virtualCam.m_Lens.OrthographicSize;
        float baseScaler;
        //Vector2 basePos;

        Vector2 camPos = cameraTr.position;

        //Vector2 v = camPos - startPos;
        //v *= ( 1 - defaultLensSize / lens);
        //basePos = startPos + v;

        baseScaler = defaultLensSize / lens; // 1 ~ 0.5�� ��
        float a = 1 / baseScaler;      //a�� 1~2�� ���� ���´�. 
        float w = customZPos / 1000f;   //w �� 0~1�� ���� ���´�.
        float editScaler = (1 - w) + (a * w);   //w �� 0�϶� ������� 1, w�� 1�϶� ������� a
        transform.localScale = startScale * editScaler;

        ////������ : start��ġ ����
        //Vector2 deltaMovement = camPos - startCamPos;
        //basePos += deltaMovement * (1 - (baseZPos / customZPos) * (defaultLensSize / lens));

        //float f = baseZPos / customZPos;


        ////���� ��ġ�� ������
        //transform.position = basePos;
        ////transform.localScale = startScale * (1 - (baseScaler * f));


        //���� ��ġ�� ��´�.

        Vector2 basePos;

        //���� ��ġ�� ���� �� �༺���� �ɸ��� �Ÿ���ŭ �̸� ���߳��� �ִ� ��. �þ߰� ����. 
        Vector2 v = startCamPos - startPos;
        v *= (1 - (baseZPos / customZPos) * (defaultLensSize / lens));
        basePos = startPos + v;


        //�����̰� �����
        Vector2 t = camPos - startCamPos;
        t *= (1 - (baseZPos / customZPos) * (defaultLensSize / lens));
        transform.position = basePos + t;

    }
}
