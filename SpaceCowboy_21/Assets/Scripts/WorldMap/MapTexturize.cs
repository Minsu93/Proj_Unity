using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTexturize : MonoBehaviour
{
    public Texture2D lockedTex;
    public Texture2D buyableTex;
    public Texture2D openedTex;

    public GameObject PassUI;
    public GameObject MapUI;

    SpriteRenderer[] sprites;
    int selectedMapIndex = 0;   //현재 선택된 맵 번호.
    MapSelector selectedMap;    //현재 선택된 맵 

    // Start is called before the first frame update
    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    //맵 텍스쳐 변경
    public void ChangeTexture(MapState state, SpriteRenderer spr)
    {
        //해당 spr을 찾는다.
        int num = Array.IndexOf(sprites, spr);
        if(num > -1)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            //해당 텍스쳐를 변경한다 
            switch (state)
            {
                case MapState.Locked:
                    block.SetTexture("_MainTex", lockedTex);
                    spr.SetPropertyBlock(block);
                    Debug.Log("Texture Changed : Locked " + spr.gameObject.name);

                    break;

                case MapState.Buyable:
                    block.SetTexture("_MainTex", buyableTex);
                    spr.SetPropertyBlock(block);
                    Debug.Log("Texture Changed : Buyable  " + spr.gameObject.name);

                    break;
                    
                case MapState.Opened:
                    block.SetTexture("_MainTex", openedTex);
                    spr.SetPropertyBlock(block);
                    Debug.Log("Texture Changed : Opened " + spr.gameObject.name);

                    break;
            }
        }
    }

    //통행증 구매 PassUI 오픈 
    public void PassUIOpen(MapSelector map)
    {
        //해당 통행증 금액을 불러온다. 이미지 교체.
        //index에 따라 이미지를 교체한다. 
        selectedMap = map;
        selectedMapIndex = map.levelIndex;
        PassUI.SetActive(true);
    }

    //통행증 구매
    public void PassUIBuy()
    {
        //구매 가능? 
        //구매했으면 돈 차감 
        //구매가 불가능했으면 아무 일도 일어나지 않는다. 
        //현재 index의 속성을 바꾼다. 
        selectedMap.ChangeMapState(MapState.Opened);
        selectedMap = null;
        selectedMapIndex = -1;
        PassUI.SetActive(false);
    }
    //통행증 취소
    public void PassUICancel()
    {
        selectedMap = null;
        selectedMapIndex = -1;
        PassUI.SetActive(false);
    }

    //맵 속성 MapUI 오픈
    public void MapUIOpen(MapSelector map)
    {
        //맵UI 창을 띄우고 이미지를 해당 맵에 맞춰 교체한다. 
        MapUI.SetActive(true);
        selectedMap = map;
        selectedMapIndex = map.levelIndex;

    }

    //맵 입장
    public void MapUIEnter()
    {
        //맵 불러오기
        SceneManager.LoadScene(2);
    }

    //맵 입장 취소
    public void MapUICancel()
    {
        selectedMap = null;
        selectedMapIndex = -1;
        MapUI.SetActive(false);
    }
}
