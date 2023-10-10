using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    
    public float threshold;
    public Sprite reticleSprite;

    Transform player;
    GameObject reticle;

    bool activate = true;

    private void Awake()
    {
        this.player = GameManager.Instance.player;
        this.transform.position = player.transform.position;
        GameManager.Instance.PlayerDeadEvent += StopCameraFollow;
        //reticle = new GameObject("Reticle");
        //reticle.transform.position = player.position;
        //reticle.transform.parent = transform;
        //SpriteRenderer spr = reticle.AddComponent<SpriteRenderer>();
        //spr.sprite = reticleSprite;
        //spr.sortingLayerName = "Above";
    }
    private void Update()
    {
        if (!activate)
        {
            //캐릭터가 죽으면 카메라는 플레이어 시체를 따라다님.
            this.transform.position = player.transform.position;

            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        //reticle.transform.position = mousePos;


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
