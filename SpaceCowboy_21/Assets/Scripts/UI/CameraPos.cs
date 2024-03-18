using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public GameObject ReticleOBJ;
    public float threshold;
    public Sprite reticleSprite;

    Transform player;
    GameObject reticle;

    bool activate = true;

    private void Awake()
    {
        reticle = ReticleOBJ;
        SpriteRenderer spr = reticle.AddComponent<SpriteRenderer>();
        spr.sprite = reticleSprite;
        spr.sortingLayerName = "Effect";
    }
    private void Start()
    {
        this.player = GameManager.Instance.player;
        this.transform.position = player.transform.position;
        GameManager.Instance.PlayerDeadEvent += StopCameraFollow;

        //위치 초기화
        reticle.transform.position = player.position;
    }
    private void Update()
    {
        if (!activate)
        {
            //캐릭터가 죽으면 카메라는 플레이어 시체를 따라다님.
            this.transform.position = player.transform.position;

            return;
        }

        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos;
        inputPos.z = 10;    //z는 카메라에서부터의 거리
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //마우스 월드 위치
        mousePos.z = 0;

        reticle.transform.position = mousePos;


        Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        ret *= 2;
        ret -= Vector2.one;
        
        float max = 0.9f;
        if(Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized;
        }

        this.transform.position = ret * threshold + (Vector2)player.position;
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
