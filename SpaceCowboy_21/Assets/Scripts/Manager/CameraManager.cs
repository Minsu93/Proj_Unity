using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CameraPos cameraPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    //ī�޶�FOV ����
    //public float defaultFOV = 90f;
    //public float currFOV { get; private set; }
    //float targetFOV;

    //float approx = 0.05f;
    //[SerializeField] float defaultFovControlSpd = 5.0f;
    //float fovControlSpd;

    //CameraPos����
    //public float defaultCamSpeed = 3f;
    //public float defaultThreshold = 2f;


    private void Awake()
    {
        GameManager.Instance.cameraManager = this;

        //�ʱ� FOV
        //fovControlSpd = defaultFovControlSpd;
        //currFOV = defaultFOV;
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

    public void ActiveVirtualCam(bool active)
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

}
