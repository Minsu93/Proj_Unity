using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineVirtualCamera virtualCameraMain;
    CinemachineConfiner2D confiner;

    //CameraPos����
    [Header("CameraPos")]
    [SerializeField] float movementInfluence = 40.0f;
    [SerializeField] float camSpeed = 3f;
    [SerializeField] float threshold = 2f;
    CameraPos cameraPos;
    public GameObject reticle { get; private set; }

    private void Awake()
    {
        curLensSize = middleGroundDist;
        targetLensSize = curLensSize;
        defaultLensSize = curLensSize;
    }

    private void FixedUpdate()
    {
        //UpdateCamLens();
    }




    #region Initialize Camera

    public void InitCam()
    {
        GameObject camPosObj = new GameObject("CamPos");
        cameraPos = camPosObj.AddComponent<CameraPos>();
        cameraPos.CameraPosInitInStage(movementInfluence, camSpeed, threshold);
        reticle = cameraPos.CreateReticle();

        curLensSize = middleGroundDist;
        targetLensSize = curLensSize;
        defaultLensSize = curLensSize;
    }

    public void ChangeCamera(CinemachineVirtualCamera preVCam, CinemachineVirtualCamera currVCam)
    {
        if (preVCam != null) 
        { 
            preVCam.Priority = 0;
            preVCam.gameObject.SetActive(false);
        } 
        
        if(currVCam != null)
        {
            currVCam.gameObject.SetActive(true);
            currVCam.Priority = 1;
        }

        virtualCameraMain = currVCam;
        confiner = virtualCameraMain.GetComponent<CinemachineConfiner2D>();

        virtualCameraMain.m_Lens.OrthographicSize = curLensSize;
    }

    public void FollowCamera()
    {
        virtualCameraMain.PreviousStateIsValid = false;
        virtualCameraMain.Follow = cameraPos.transform;
    }

    public void ChangeCamFollow(Transform tr)
    {
        virtualCameraMain.Follow = tr;
    }


    #endregion





    #region Map Border Events
    //ī�޶� �̺�Ʈ
    public void StopCameraFollow()
    {
        virtualCameraMain.Follow = null;
        //cameraPos.StopCameraFollow();
    }

    public void SetActiveVirtualCam(bool active)
    {
        virtualCameraMain.PreviousStateIsValid = false;
        virtualCameraMain.gameObject.SetActive(active);
    }

    //ī�޶� �̵�.
    public void teleportCamera(Vector2 pos, Vector2 borderHalfSize, Vector2 borderPos)
    {
        Debug.Log("Teleport Cam");
        //ȭ�� ������ ���̸� ���Ѵ�. 
        Camera cam = Camera.main;
        float cameraHeightHalf = cam.orthographicSize;
        float ratio = (float) Screen.width / Screen.height;
        float cameraWidthHalf = cameraHeightHalf * ratio;

        float x = pos.x;
        float y = pos.y;


        x = Mathf.Clamp(x, borderPos.x - borderHalfSize.x + cameraWidthHalf, borderPos.x + borderHalfSize.x - cameraWidthHalf);
        y = Mathf.Clamp(y, borderPos.y - borderHalfSize.y  + cameraHeightHalf, borderPos.y + borderHalfSize.y - cameraHeightHalf);
        Vector2 newPos = new Vector2(x, y);

        //�̵� �� pos�� ȭ�� �����ڸ����, ȭ�� ���� ���η� �̵���Ų��. 
        //cameraPos.MoveCamPos(newPos);
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

    //Update���� ī�޶� ���� ����
    float timer = 0;
    void UpdateCamLens()
    {
        if (Mathf.Abs(curLensSize - targetLensSize) > 0.1f)
        {
            curLensSize = Mathf.SmoothDamp(curLensSize, targetLensSize, ref curVelocity, zoomSpeed);
            virtualCameraMain.m_Lens.OrthographicSize = curLensSize;

            //confiner �� 0.2�ʸ��� ������Ʈ
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                timer = 0;
                if(confiner != null) confiner.InvalidateCache();
            }
        }
    }
    

    //ī�޶� �� �� & �ƿ�
    public void ZoomCamera(CamDist targetLensSize, ZoomSpeed zoomSpeed)
    {
        this.targetLensSize =  SelectCameraDist(targetLensSize);
        this.zoomSpeed = SelectZoomSpeed(zoomSpeed);
    }

    
    //���� ��� ����
    public void SetStartCamera(CamDist startCamDist)
    {
        curLensSize = SelectCameraDist(startCamDist);
        virtualCameraMain.m_Lens.OrthographicSize = curLensSize;
    }


    //enum �� ���� float �ҷ����� �Լ�
    float SelectCameraDist(CamDist camDist)
    {
        switch (camDist)
        {
            case CamDist.Fore:
                return foreGroundDist;
            case CamDist.Middle:
                return  middleGroundDist;
            case CamDist.Back:
                return backGroundDist;
            default:
                return 0;
        }
    }

    float SelectZoomSpeed(ZoomSpeed zoomSpeed)
    {
        switch (zoomSpeed)
        {
            case ZoomSpeed.Slow:
                return slowZoomSeed;
            case ZoomSpeed.Fast:
                return fastZoomSeed;

            default:
                return 0;
        }

    }


    #endregion

}
public enum CamDist { Fore, Middle, Back }
public enum ZoomSpeed { Fast, Slow }
