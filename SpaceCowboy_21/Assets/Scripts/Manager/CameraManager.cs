using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CameraPos cameraPos;
    public CinemachineVirtualCamera virtualCamera;

    //카메라 관련
    public float mapFOV = 120f;
    public float defaultFOV = 90f;
    float currFOV;
    float targetFOV;

    float approx = 0.05f;
    public float defaultFovControlSpd = 5.0f;
    float fovControlSpd;

    bool changeCam = false;

    //CameraPos관련
    public float defaultCamSpeed = 3f;
    public float defaultThreshold = 2f;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //초기 FOV
        currFOV = defaultFOV;
        targetFOV = currFOV;
        fovControlSpd = defaultFovControlSpd;
        //초기 threshold, spd
        ResetCameraThreshold();

        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;
    }

    private void FixedUpdate()
    {
        //카메라 조절

        if (Mathf.Abs(currFOV - targetFOV) > approx)
        {
            currFOV = Mathf.Lerp(currFOV, targetFOV, Time.deltaTime * fovControlSpd);
            virtualCamera.m_Lens.FieldOfView = currFOV;
        }

    }

    //카메라 이벤트
    public void MapOpen()
    {
        targetFOV = mapFOV;

        cameraPos.StopCameraFollow();

    }

    public void MapClose()
    {
        targetFOV = defaultFOV;

        cameraPos.StartCameraFollow();
    }

    //행성 이동시 기본 FOV 변경
    public void ChangeCamera(float fov)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        fovControlSpd = defaultFovControlSpd;

    }

    //느리게, 혹은 빠르게 카메라 확대,축소 용도
    public void ChangeCamera(float fov, float spd)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        fovControlSpd = spd;
    }


    //카메라 Threshold변경
    public void ChangeCameraThreshold(float threshold, float camSpd)
    {
        cameraPos.threshold = threshold;
        cameraPos.camSpeed = camSpd;

        targetFOV = mapFOV;
    }
    public void ResetCameraThreshold()
    {
        cameraPos.threshold = defaultThreshold;
        cameraPos.camSpeed = defaultCamSpeed;

        targetFOV = defaultFOV;

    }


    public void PlayerIsDead()
    {
        virtualCamera.Follow = null;
    }
}
