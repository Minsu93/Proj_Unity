using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    //public GameObject ReticleOBJ;
    public float movementInfluence;
    public float camSpeed = 3f;
    public float threshold = 2f;

    //public Sprite reticleSprite;


    Vector2 prePlayerPos;
    Vector2 currCamPos;

    Transform player;
    GameObject reticle;

    bool activate = false;

    private void Awake()
    {
        CreateRecticle();

        //위치 초기화
        //reticle.transform.position = player.position;
    }
    public void CameraPosInit()
    {
        this.player = GameManager.Instance.player;
        this.transform.position = player.transform.position;
        GameManager.Instance.PlayerDeadEvent += StopCameraFollow;
        activate = true;
    }
    private void FixedUpdate()
    {
        if (!activate)
        {
            ////캐릭터가 죽으면 카메라는 플레이어 시체를 따라다님.
            //this.transform.position = player.transform.position;
            return;
        }


        //플레이어 움직임수치 카메라 추가
        Vector2 movementVec = SeePlayerFront();
        //movementVec.y = movementVec.y * 0.1f;
            
        //Reticle 위치 보정
        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos;
        inputPos.z = 10;    //z는 카메라에서부터의 거리
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //마우스 월드 위치
        mousePos.z = 0;

        reticle.transform.position = mousePos;

        //카메라 위치 보정
        Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        ret *= 2;
        ret -= Vector2.one;
        
        float max = 0.9f;
        if(Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized;
        }
        Vector2 camPos= ret * threshold + movementVec * movementInfluence + (Vector2)player.position;
        currCamPos = Vector2.Lerp(currCamPos, camPos, Time.deltaTime * camSpeed);

        transform.position = currCamPos;
    }

    void CreateRecticle()
    {
        reticle = new GameObject("Reticle");
        SpriteRenderer spr = reticle.AddComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>("/UI/Reticle3");
        spr.sortingLayerName = "Effect";
    }

    Vector2 SeePlayerFront()
    {
        Vector2 front = (Vector2)player.position - prePlayerPos;
        prePlayerPos = player.position;
        return front;
    }

    public void StopCameraFollow()
    {
        activate = false;
    }

    public void StartCameraFollow()
    {
        activate = true;
    }
}
