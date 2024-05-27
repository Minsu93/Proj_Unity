using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CameraPos cameraPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    //ī�޶�FOV ����
    public float defaultFOV = 90f;
    public float currFOV { get; private set; }
    float targetFOV;

    float approx = 0.05f;
    [SerializeField] float defaultFovControlSpd = 5.0f;
    float fovControlSpd;

    //CameraPos����
    public float defaultCamSpeed = 3f;
    public float defaultThreshold = 2f;


    private void Awake()
    {
        GameManager.Instance.cameraManager = this;

        //�ʱ� FOV
        //fovControlSpd = defaultFovControlSpd;
        currFOV = defaultFOV;
        //targetFOV = currFOV;
    }


    //private void FixedUpdate()
    //{
    //    //ī�޶� ����
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
    //ī�޶� �̺�Ʈ
    public void StopCameraFollow()
    {
        cameraPos.StopCameraFollow();
    }

    public void StartCameraFollow()
    {
        cameraPos.StartCameraFollow();
    }

    //�༺ �̵��� �⺻ FOV ����
    public void ChangeCamera(float fov)
    {
        //defaultFOV = fov;
        //targetFOV = defaultFOV;
        //fovControlSpd = defaultFovControlSpd;
    }

    //�ӵ� ���� �� ī�޶� Ȯ��,��� 
    public void ChangeCamera(float fov, float spd)
    {
        //defaultFOV = fov;
        //targetFOV = defaultFOV;
        //fovControlSpd = spd;
    }

}
