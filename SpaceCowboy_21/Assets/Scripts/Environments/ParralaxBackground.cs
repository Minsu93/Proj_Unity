using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ParralaxBackground : MonoBehaviour
{
    [SerializeField] private float parralaxEffectMultiplier;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    CinemachineVirtualCamera virtualCam;
    float defaultLensSize;
    float defaultFOV;
    Vector3 startScale;



    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;


        virtualCam = CameraManager.instance.virtualCamera;
        startScale = transform.localScale;
        defaultFOV = CameraManager.instance.defaultFOV;

        //���� ��ġ ��¦ ����ֱ�

    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        transform.position += deltaMovement * parralaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;



        ////��� ���� ���̽� ��ġ, ������ 
        //float lens = virtualCam.m_Lens.OrthographicSize;
        float curFOV = virtualCam.m_Lens.FieldOfView;
        float baseScaler;
        //Vector2 basePos;

        //baseScaler = defaultLensSize / lens; // 1 ~ 0.5�� ��
        baseScaler = defaultFOV / curFOV;
        float a = 1 / baseScaler;      //a�� 1~2�� ���� ���´�. 
        transform.localScale = startScale * a;


    }
}
