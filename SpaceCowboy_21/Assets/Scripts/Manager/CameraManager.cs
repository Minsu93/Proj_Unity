using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CameraPos cameraPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    //카메라FOV 관련
    public float defaultFOV = 90f;
    public float currFOV { get; private set; }
    float targetFOV;

    float approx = 0.05f;
    [SerializeField] float defaultFovControlSpd = 5.0f;
    float fovControlSpd;

    //CameraPos관련
    public float defaultCamSpeed = 3f;
    public float defaultThreshold = 2f;


    private void Awake()
    {
        GameManager.Instance.cameraManager = this;

        //초기 FOV
        //fovControlSpd = defaultFovControlSpd;
        currFOV = defaultFOV;
        //targetFOV = currFOV;
    }


    //private void FixedUpdate()
    //{
    //    //카메라 조절
    //    if (virtualCamera == null) return;

    //    if (Mathf.Abs(currFOV - targetFOV) > approx)
    //    {
    //        currFOV = Mathf.Lerp(currFOV, targetFOV, Time.deltaTime * fovControlSpd);
    //        virtualCamera.m_Lens.FieldOfView = currFOV;
    //    }

    //}

    public void InitCam()
    {
        cameraPos.CameraPosInit();
    }
    //카메라 이벤트
    public void StopCameraFollow()
    {
        cameraPos.StopCameraFollow();
    }

    public void StartCameraFollow()
    {
        cameraPos.StartCameraFollow();
    }

    //행성 이동시 기본 FOV 변경
    public void ChangeCamera(float fov)
    {
        //defaultFOV = fov;
        //targetFOV = defaultFOV;
        //fovControlSpd = defaultFovControlSpd;
    }

    //속도 지정 식 카메라 확대,축소 
    public void ChangeCamera(float fov, float spd)
    {
        //defaultFOV = fov;
        //targetFOV = defaultFOV;
        //fovControlSpd = spd;
    }

}
