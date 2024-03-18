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
    public float controlSpeed = 5.0f;
    float cSpeed;

    bool changeCam = false;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //�ʱ� FOV
        currFOV = defaultFOV;
        targetFOV = currFOV;
        cSpeed = controlSpeed;

        GameManager.Instance.PlayerDeadEvent += PlayerIsDead;
    }

    private void FixedUpdate()
    {
        //ī�޶� ����

        if (Mathf.Abs(currFOV - targetFOV) > approx)
        {
            currFOV = Mathf.Lerp(currFOV, targetFOV, Time.deltaTime * cSpeed);
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
        cSpeed = controlSpeed;

    }

    public void ChangeCamera(float fov, float spd)
    {
        defaultFOV = fov;
        targetFOV = defaultFOV;
        cSpeed = spd;
    }



    public void PlayerIsDead()
    {
        virtualCamera.Follow = null;
    }
}
