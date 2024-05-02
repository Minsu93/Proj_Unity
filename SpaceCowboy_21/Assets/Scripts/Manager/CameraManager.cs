using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CameraPos cameraPos;
    public CinemachineVirtualCamera virtualCamera;
    public GameObject miniMapCam;
    public GameObject worldMapCam;

    //ī�޶� ����
    public float mapFOV = 120f;
    public float defaultFOV = 90f;
    float currFOV;
    float targetFOV;

    float approx = 0.05f;
    public float defaultFovControlSpd = 5.0f;
    float fovControlSpd;

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
        //targetFOV = mapFOV;

        cameraPos.StopCameraFollow();
        miniMapCam.SetActive(false);
        worldMapCam.SetActive(true);

    }

    public void MapClose()
    {
        //targetFOV = defaultFOV;

        cameraPos.StartCameraFollow();
        miniMapCam.SetActive(true);
        worldMapCam.SetActive(false);
    }

    //�༺ �̵��� �⺻ FOV ����
    public void ChangeCamera(float fov)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        fovControlSpd = defaultFovControlSpd;

    }

    //�ӵ� ���� �� ī�޶� Ȯ��,��� 
    public void ChangeCamera(float fov, float spd)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        fovControlSpd = spd;
    }





    public void PlayerIsDead()
    {
        virtualCamera.Follow = null;
    }
}
