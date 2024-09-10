using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    CinemachineVirtualCamera virtualCameraMain;
    CinemachineConfiner2D confiner;

    //CameraPos관련
    [Header("CameraPos")]
    [SerializeField] float movementInfluence = 40.0f;
    [SerializeField] float camSpeed = 3f;
    [SerializeField] float threshold = 2f;
    CameraPos cameraPos;

    private void Awake()
    {

        curLensSize = middleGroundDist;
        targetLensSize = curLensSize;
        defaultLensSize = curLensSize;
        
        //test. 해당 씬에서 그냥 시작할때.
        //if(virtualCameraMain == null)
        //{
        //    SetVirtualCam();
        //}
    }

    private void FixedUpdate()
    {
        UpdateCamLens();
    }

    #region Initialize Camera
    public void InitCam()
    {
        cameraPos = CreateCamPos();
        cameraPos.CameraPosInitInStage(movementInfluence, camSpeed, threshold);
        
        virtualCameraMain.Follow = cameraPos.transform;

    }
    public void InitLobbyCam(Transform tr)
    {
        cameraPos = CreateCamPos();
        cameraPos.CamPosInitLobby(tr);
        
        virtualCameraMain.Follow = cameraPos.transform;
    }

    public void SetVirtualCam()
    {
        Debug.Log("Set Vcam");

        GameObject camObj = GameObject.FindGameObjectWithTag("Vcam");
        if (camObj == null) return;
        
        virtualCameraMain = camObj.GetComponent<CinemachineVirtualCamera>();
        confiner = virtualCameraMain.GetComponent<CinemachineConfiner2D>();
        
        curLensSize = middleGroundDist;
        targetLensSize = curLensSize;
        defaultLensSize = curLensSize;

        virtualCameraMain.m_Lens.OrthographicSize = curLensSize;
    }

    CameraPos CreateCamPos()
    {
        GameObject camPosObj = new GameObject();
        CameraPos camPos = camPosObj.AddComponent<CameraPos>();
        return camPos;
    }

    public void ResetCam()
    {
        Debug.Log("Reset Vcam");

        virtualCameraMain = null;
    }

    #endregion

    #region Map Border Events
    //카메라 이벤트
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
        virtualCameraMain.gameObject.SetActive(active);
    }

    //카메라 이동.
    public void MoveCamera(Vector2 pos, Vector2 borderHalfSize, Vector2 borderPos)
    {
        //화면 절반의 넓이를 구한다. 
        Camera cam = Camera.main;
        float cameraHeightHalf = cam.orthographicSize;
        float ratio = (float) Screen.width / Screen.height;
        float cameraWidthHalf = cameraHeightHalf * ratio;

        float x = pos.x;
        float y = pos.y;


        x = Mathf.Clamp(x, borderPos.x - borderHalfSize.x + cameraWidthHalf, borderPos.x + borderHalfSize.x - cameraWidthHalf);
        y = Mathf.Clamp(y, borderPos.y - borderHalfSize.y  + cameraHeightHalf, borderPos.y + borderHalfSize.y - cameraHeightHalf);
        Vector2 newPos = new Vector2(x, y);

        //이동 시 pos가 화면 가장자리라면, 화면 절반 내부로 이동시킨다. 
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

    //Update에서 카메라 렌즈 조절
    float timer = 0;
    void UpdateCamLens()
    {
        if (Mathf.Abs(curLensSize - targetLensSize) > 0.1f)
        {
            curLensSize = Mathf.SmoothDamp(curLensSize, targetLensSize, ref curVelocity, zoomSpeed);
            virtualCameraMain.m_Lens.OrthographicSize = curLensSize;

            //confiner 를 0.2초마다 업데이트
            
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                timer = 0;
                if(confiner != null) confiner.InvalidateCache();
            }
        }
    }
    

    //카메라 줌 인 & 아웃
    public void ZoomCamera(CamDist targetLensSize, ZoomSpeed zoomSpeed)
    {
        this.targetLensSize =  SelectCameraDist(targetLensSize);
        this.zoomSpeed = SelectZoomSpeed(zoomSpeed);
    }

    
    //렌즈 즉시 변경
    public void SetStartCamera(CamDist startCamDist)
    {
        curLensSize = SelectCameraDist(startCamDist);
        virtualCameraMain.m_Lens.OrthographicSize = curLensSize;
    }


    //enum 에 따라 float 불러오는 함수
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
