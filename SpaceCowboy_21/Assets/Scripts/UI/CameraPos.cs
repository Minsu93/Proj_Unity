using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    float movementInfluence = 40.0f;
    float camSpeed = 3f;
    float threshold = 2f;

    Vector2 prePlayerPos;
    Vector2 currCamPos;

    GameObject reticle;

    bool activate = false;


    public void CameraPosInitInStage(float movementInfluence, float camSpeed, float threshold)
    {
        this.movementInfluence = movementInfluence;
        this.camSpeed = camSpeed;
        this.threshold = threshold;

        GameManager.Instance.PlayerDeadEvent -= StopCameraFollow;
        GameManager.Instance.PlayerDeadEvent += StopCameraFollow;



        activate = true;
    }

    public GameObject CreateReticle()
    {
        reticle = CreateRecticle();
        return reticle;
    }


    [SerializeField] Vector2 ret;
    private void Update()
    {
        if (!activate) return;

        if (GameManager.Instance.player == null) return;
        if (reticle == null) return;

        //플레이어 움직임수치 카메라 추가
        //Vector2 movementVec = (Vector2)player.position - prePlayerPos;
        //prePlayerPos = player.position;
            
        //Reticle 위치 보정
        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos;
        inputPos.z = 10;    //z는 카메라에서부터의 거리
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //마우스 월드 위치
        mousePos.z = 0;
        reticle.transform.position = mousePos;

        //카메라 위치 보정(ViewportPoint 는 왼쪽 아래가 (0,0) 오른쪽 위가 (1,1))
        ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        ret *= 2;
        ret -= Vector2.one; //카메라 위치는 이제 (-1,-1) ~ (1,1) 이 된다. 
        float max = 0.9f;
        if(Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized;
        }
        Vector2 camPos= ret * threshold + (Vector2)GameManager.Instance.player.position;
        //Vector2 camPos = ret * threshold + movementVec * movementInfluence + (Vector2)player.position;

        currCamPos = Vector2.Lerp(currCamPos, camPos, Time.deltaTime * camSpeed);

        transform.position = currCamPos;
    }

    GameObject CreateRecticle()
    {
        reticle = new GameObject("Reticle");
        SpriteRenderer spr = reticle.AddComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>("UI/Reticle3");
        spr.sortingLayerName = "Effect";
        return reticle;
    }


    public void StopCameraFollow()
    {
        activate = false;
    }

    public void StartCameraFollow()
    {
        activate = true;
    }

    public void MoveCamPos(Vector2 pos)
    {
        transform.position = pos;
        reticle.transform.position = pos;
        prePlayerPos = pos;
        currCamPos = pos;
    }
}
