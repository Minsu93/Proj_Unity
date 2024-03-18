using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int levelIndex = 0;
    public MapState curState = MapState.Locked;
    public Vector3 Scaler = new Vector3(1.2f, 1.2f, 1.2f);
    int orderInLayer;
    SpriteRenderer spr;
    MapTexturize mapTex;

    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");

        switch (curState)
        {
            case MapState.Locked:
                //���� �Ұ�
                return;
            case MapState.Buyable:
                //���� UI ����
                mapTex.PassUIOpen(this);
                return;
            case MapState.Opened:
                //�̵�
                PlayerIcon.icon.MoveIcon(this);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (curState == MapState.Locked )
            return;

        transform.localScale = Scaler;
        spr.sortingOrder = 10000;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (curState == MapState.Locked)
            return;

        transform.localScale = Vector3.one;
        spr.sortingOrder = orderInLayer;
    }
    
    void Awake()
    {
        spr = this.GetComponent<SpriteRenderer>();
        orderInLayer = spr.sortingOrder;
        mapTex = GetComponentInParent<MapTexturize>();
    }


    void Start()
    {
        mapTex.ChangeTexture(curState, spr);    
    }

    //�� UI ����
    public void OpenMapUI()
    {
        mapTex.MapUIOpen(this);
    }


    //�� �ؽ��� �ٲٱ�
    public void ChangeMapState(MapState state)
    {
        curState = state;
        mapTex.ChangeTexture(curState, spr);
    }
}

public enum MapState {Locked, Buyable, Opened}