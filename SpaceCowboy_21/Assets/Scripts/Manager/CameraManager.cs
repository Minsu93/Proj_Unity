using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CameraPos cameraPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    //카메라FOV 관련
    //public float defaultFOV = 90f;
    //public float currFOV { get; private set; }
    //float targetFOV;

    //float approx = 0.05f;
    //[SerializeField] float defaultFovControlSpd = 5.0f;
    //float fovControlSpd;

    //CameraPos관련
    //public float defaultCamSpeed = 3f;
    //public float defaultThreshold = 2f;


    private void Awake()
    {
        GameManager.Instance.cameraManager = this;

        //초기 FOV
        //fovControlSpd = defaultFovControlSpd;
        //currFOV = defaultFOV;
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

    public void ActiveVirtualCam(bool active)
    {
        virtualCamera.gameObject.SetActive(active);
    }

    //카메라 이동.
    public void MoveCamera(Vector2 pos, Vector2 limitSize)
    {
        //화면 절반의 넓이를 구한다. 
        Camera cam = Camera.main;
        float cameraHeightHalf = cam.orthographicSize;
        float ratio = (float) Screen.width / Screen.height;
        float cameraWidthHalf = cameraHeightHalf * ratio;

        float x = pos.x;
        float y = pos.y;
        x = Mathf.Clamp(x, -limitSize.x + cameraWidthHalf, limitSize.x - cameraWidthHalf);
        y = Mathf.Clamp(y, -limitSize.y + cameraHeightHalf, limitSize.y - cameraHeightHalf);
        Vector2 newPos = new Vector2(x, y);

        //이동 시 pos가 화면 가장자리라면, 화면 절반 내부로 이동시킨다. 
        cameraPos.MoveCamPos(newPos);
    }

}
