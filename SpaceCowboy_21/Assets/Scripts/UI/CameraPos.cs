using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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


    public void CameraPosInitInStage()
    {
        this.player = GameManager.Instance.player;
        this.transform.position = player.transform.position;
        GameManager.Instance.PlayerDeadEvent += StopCameraFollow;
        activate = true;

        GameObject reticle =  CreateRecticle();
        GameManager.Instance.playerManager.SetReticleFollower(reticle);

    }

    public void CamPosInitLobby(Transform playerTr)
    {
        activate = true;
        player = playerTr;
        CreateRecticle();
    }

    [SerializeField] Vector2 ret;
    private void Update()
    {
        if (!activate || player == null)
        {
            return;
        }

        //�÷��̾� �����Ӽ�ġ ī�޶� �߰�
        Vector2 movementVec = (Vector2)player.position - prePlayerPos;
        prePlayerPos = player.position;
            
        //Reticle ��ġ ����
        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos;
        inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
        mousePos = Camera.main.ScreenToWorldPoint(inputPos);    //���콺 ���� ��ġ
        mousePos.z = 0;
        reticle.transform.position = mousePos;

        //ī�޶� ��ġ ����(ViewportPoint �� ���� �Ʒ��� (0,0) ������ ���� (1,1))
        ret = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        ret *= 2;
        ret -= Vector2.one; //ī�޶� ��ġ�� ���� (-1,-1) ~ (1,1) �� �ȴ�. 
        float max = 0.9f;
        if(Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized;
        }
        Vector2 camPos= ret * threshold + movementVec * movementInfluence + (Vector2)player.position;
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
