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

            
        //Reticle ��ġ ����
        Vector3 inputPos = Input.mousePosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, Camera.main.nearClipPlane));    //���콺 ���� ��ġ
        reticle.transform.position = mousePos;


        //ī�޶� ��ġ ����(ViewportPoint �� ���� �Ʒ��� (0,0) ������ ���� (1,1))
        ret = Camera.main.ScreenToViewportPoint(inputPos);
        ret *= 2;
        ret -= Vector2.one; //ī�޶� ��ġ�� ���� (-1,-1) ~ (1,1) �� �ȴ�. 
        float max = 0.9f;
        if(Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized;
        }
        Vector2 camPos = ret * threshold + (Vector2)GameManager.Instance.player.position;

        transform.position = camPos;
    }

    GameObject CreateRecticle()
    {
        reticle = new GameObject("Reticle");
        SpriteRenderer spr = reticle.AddComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>("UI/Reticle3");
        spr.sortingLayerName = "Effect";
        return reticle;
    }

}
