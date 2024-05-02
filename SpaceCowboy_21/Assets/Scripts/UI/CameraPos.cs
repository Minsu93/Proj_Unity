using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public GameObject ReticleOBJ;
    public float movementInfluence;
    public float camSpeed = 3f;
    public float threshold = 2f;

    public Sprite reticleSprite;

    Vector2 prePlayerPos;
    Vector2 currCamPos;

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
    private void FixedUpdate()
    {
        if (!activate)
        {
            //ĳ���Ͱ� ������ ī�޶�� �÷��̾� ��ü�� ����ٴ�.
            this.transform.position = player.transform.position;

            return;
        }


        //�÷��̾� �����Ӽ�ġ ī�޶� �߰�
        Vector2 movementVec = SeePlayerFront();
        //movementVec.y = movementVec.y * 0.1f;
            
        //Reticle ��ġ ����
        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos;
        inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //���콺 ���� ��ġ
        mousePos.z = 0;

        reticle.transform.position = mousePos;

        //ī�޶� ��ġ ����
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
