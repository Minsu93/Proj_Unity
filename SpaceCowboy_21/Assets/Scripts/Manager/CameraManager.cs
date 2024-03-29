using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CameraPos cameraPos;
    public CinemachineVirtualCamera virtualCamera;

    //ī�޶� ����
    public float mapFOV = 120f;
    public float defaultFOV = 90f;
    float currFOV;
    float targetFOV;

    float approx = 0.05f;
    public float defaultFovControlSpd = 5.0f;
    float fovControlSpd;

    bool changeCam = false;

    //CameraPos����
    public float defaultCamSpeed = 3f;
    public float defaultThreshold = 2f;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //�ʱ� FOV
        currFOV = defaultFOV;
        targetFOV = currFOV;
        fovControlSpd = defaultFovControlSpd;
        //�ʱ� threshold, spd
        ResetCameraThreshold();

        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;
    }

    private void FixedUpdate()
    {
        //ī�޶� ����

        if (Mathf.Abs(currFOV - targetFOV) > approx)
        {
            currFOV = Mathf.Lerp(currFOV, targetFOV, Time.deltaTime * fovControlSpd);
            virtualCamera.m_Lens.FieldOfView = currFOV;
        }

    }

    //ī�޶� �̺�Ʈ
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

    //�༺ �̵��� �⺻ FOV ����
    public void ChangeCamera(float fov)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        fovControlSpd = defaultFovControlSpd;

    }

    //������, Ȥ�� ������ ī�޶� Ȯ��,��� �뵵
    public void ChangeCamera(float fov, float spd)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        fovControlSpd = spd;
    }


    //ī�޶� Threshold����
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
