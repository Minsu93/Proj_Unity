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

        //��ġ �ʱ�ȭ
        reticle.transform.position = player.position;
    }
    private void Update()
    {
        if (!activate)
        {
            //ĳ���Ͱ� ������ ī�޶�� �÷��̾� ��ü�� ����ٴ�.
            this.transform.position = player.transform.position;

            return;
        }

        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos;
        inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //���콺 ���� ��ġ
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
