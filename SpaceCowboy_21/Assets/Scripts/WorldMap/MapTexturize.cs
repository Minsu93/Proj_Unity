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
    int selectedMapIndex = 0;   //���� ���õ� �� ��ȣ.
    MapSelector selectedMap;    //���� ���õ� �� 

    // Start is called before the first frame update
    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    //�� �ؽ��� ����
    public void ChangeTexture(MapState state, SpriteRenderer spr)
    {
        //�ش� spr�� ã�´�.
        int num = Array.IndexOf(sprites, spr);
        if(num > -1)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            //�ش� �ؽ��ĸ� �����Ѵ� 
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

    //������ ���� PassUI ���� 
    public void PassUIOpen(MapSelector map)
    {
        //�ش� ������ �ݾ��� �ҷ��´�. �̹��� ��ü.
        //index�� ���� �̹����� ��ü�Ѵ�. 
        selectedMap = map;
        selectedMapIndex = map.levelIndex;
        PassUI.SetActive(true);
    }

    //������ ����
    public void PassUIBuy()
    {
        //���� ����? 
        //���������� �� ���� 
        //���Ű� �Ұ��������� �ƹ� �ϵ� �Ͼ�� �ʴ´�. 
        //���� index�� �Ӽ��� �ٲ۴�. 
        selectedMap.ChangeMapState(MapState.Opened);
        selectedMap = null;
        selectedMapIndex = -1;
        PassUI.SetActive(false);
    }
    //������ ���
    public void PassUICancel()
    {
        selectedMap = null;
        selectedMapIndex = -1;
        PassUI.SetActive(false);
    }

    //�� �Ӽ� MapUI ����
    public void MapUIOpen(MapSelector map)
    {
        //��UI â�� ���� �̹����� �ش� �ʿ� ���� ��ü�Ѵ�. 
        MapUI.SetActive(true);
        selectedMap = map;
        selectedMapIndex = map.levelIndex;

    }

    //�� ����
    public void MapUIEnter()
    {
        //�� �ҷ�����
        SceneManager.LoadScene(2);
    }

    //�� ���� ���
    public void MapUICancel()
    {
        selectedMap = null;
        selectedMapIndex = -1;
        MapUI.SetActive(false);
    }
}
