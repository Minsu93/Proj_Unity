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

        //��ġ �ʱ�ȭ
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
            ////ĳ���Ͱ� ������ ī�޶�� �÷��̾� ��ü�� ����ٴ�.
            //this.transform.position = player.transform.position;
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
