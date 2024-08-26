using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineVirtualCamera virtualCamera;

    //CameraPos����
    [Header("CameraPos")]
    [SerializeField] float movementInfluence = 40.0f;
    [SerializeField] float camSpeed = 3f;
    [SerializeField] float threshold = 2f;
    CameraPos cameraPos;


    private void FixedUpdate()
    {
        ControlCamLens();
    }

    #region Initialize Camera
    public void InitCam()
    {
        cameraPos = CreateCamPos();
        cameraPos.CameraPosInitInStage(movementInfluence, camSpeed, threshold);

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = cameraPos.transform;

        curLensSize = middleGroundDist;
        targetLensSize = curLensSize;
        defaultLensSize = curLensSize;

        virtualCamera.m_Lens.OrthographicSize = curLensSize;
    }
    public void InitLobbyCam(Transform tr)
    {
        cameraPos = CreateCamPos();
        cameraPos.CamPosInitLobby(tr);

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = cameraPos.transform;

        curLensSize = middleGroundDist;
        targetLensSize = curLensSize;
        defaultLensSize = curLensSize;


        virtualCamera.m_Lens.OrthographicSize = curLensSize;
    }

    CameraPos CreateCamPos()
    {
        GameObject camPosObj = new GameObject();
        CameraPos camPos = camPosObj.AddComponent<CameraPos>();
        return camPos;
    }

    #endregion

    #region Map Border Events
    //ī�޶� �̺�Ʈ
    public void StopCameraFollow()
    {
        cameraPos.StopCameraFollow();
    }

    public void StartCameraFollow()
    {
        cameraPos.StartCameraFollow();
    }

    public void SetActiveVirtualCam(bool active)
    {
        virtualCamera.gameObject.SetActive(active);
    }

    //ī�޶� �̵�.
    public void MoveCamera(Vector2 pos, Vector2 limitSize)
    {
        //ȭ�� ������ ���̸� ���Ѵ�. 
        Camera cam = Camera.main;
        float cameraHeightHalf = cam.orthographicSize;
        float ratio = (float) Screen.width / Screen.height;
        float cameraWidthHalf = cameraHeightHalf * ratio;

        float x = pos.x;
        float y = pos.y;
        x = Mathf.Clamp(x, -limitSize.x + cameraWidthHalf, limitSize.x - cameraWidthHalf);
        y = Mathf.Clamp(y, -limitSize.y + cameraHeightHalf, limitSize.y - cameraHeightHalf);
        Vector2 newPos = new Vector2(x, y);

        //�̵� �� pos�� ȭ�� �����ڸ����, ȭ�� ���� ���η� �̵���Ų��. 
        cameraPos.MoveCamPos(newPos);
    }
    #endregion

    #region Camera Zoom
    [Header("CameraZoom")]
    [SerializeField] float foreGroundDist= 5.0f;
    [SerializeField] float middleGroundDist = 10.0f;
    [SerializeField] float backGroundDist = 20.0f;
    [SerializeField] float fastZoomSeed = 1.0f;
    [SerializeField] float slowZoomSeed = 3.0f;
    public float curLensSize { get; private set; }
    public float defaultLensSize { get; private set; }
    float targetLensSize;
    float zoomSpeed;
    float curVelocity;
    //test

    public void ZoomCamera(CamDist targetLensSize, ZoomSpeed zoomSpeed)
    {
        Debug.Log("ZoomCamera");

        switch (targetLensSize)
        {
            case CamDist.Fore:
                this. targetLensSize = foreGroundDist;
                break;
            case CamDist.Middle:
                this.targetLensSize = middleGroundDist;
                break;
            case CamDist.Back:
                this.targetLensSize = backGroundDist;
                break;
        }
        switch(zoomSpeed)
        {
            case ZoomSpeed.Slow:
                this. zoomSpeed = slowZoomSeed;
                break;
            case ZoomSpeed.Fast:
                this. zoomSpeed = fastZoomSeed;
                break;
        }
    }

    //Update���� ī�޶� ���� ����
    void ControlCamLens()
    {
        if (Mathf.Abs(curLensSize - targetLensSize) > 0.01f)
        {
            curLensSize = Mathf.SmoothDamp(curLensSize, targetLensSize, ref curVelocity, zoomSpeed);
            virtualCamera.m_Lens.OrthographicSize = curLensSize;
        }
    }


    public void StageStartCameraZoomin()
    {
        curLensSize = backGroundDist ;
        virtualCamera.m_Lens.OrthographicSize = curLensSize;

        targetLensSize = middleGroundDist;
        zoomSpeed = slowZoomSeed;
    }

    public enum CamDist { Fore, Middle, Back}
    public enum ZoomSpeed { Fast, Slow}
    #endregion

}
