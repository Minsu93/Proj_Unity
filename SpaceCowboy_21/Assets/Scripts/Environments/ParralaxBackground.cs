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
    float defaultFOV;
    Vector3 startScale;



    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        transform.position = new Vector2(lastCameraPosition.x, lastCameraPosition.y);

        virtualCam = CameraManager.instance.virtualCamera;
        startScale = transform.localScale;
        defaultFOV = CameraManager.instance.defaultFOV;

        //시작 위치 살짝 잡아주기

    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        
        transform.position += deltaMovement * parralaxEffectMultiplier;
        lastCameraPosition = cameraTransform.position;



        ////렌즈에 따른 베이스 위치, 스케일 
        float curFOV = virtualCam.m_Lens.FieldOfView;
        float baseScaler;

        baseScaler = defaultFOV / curFOV;
        float a = 1 / baseScaler;      //a는 1~2의 값을 갖는다. 
        transform.localScale = startScale * a;


    }
}
