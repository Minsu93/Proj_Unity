using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    
    public float threshold;
    public Sprite reticleSprite;

    Transform player;
    GameObject reticle;

    private void Awake()
    {
        this.player = GameManager.Instance.player;
        this.transform.position = player.transform.position;

        //reticle = new GameObject("Reticle");
        //reticle.transform.position = player.position;
        //reticle.transform.parent = transform;
        //SpriteRenderer spr = reticle.AddComponent<SpriteRenderer>();
        //spr.sprite = reticleSprite;
        //spr.sortingLayerName = "Above";
    }
    private void Update()
    {
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
}
