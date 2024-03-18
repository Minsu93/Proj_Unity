using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TaxiWorldMap : MonoBehaviour
{
    public Color selectColor = Color.white;
    public Color deselectColor = new Color(0.5f,0.5f,0.5f,1.0f);
    
    public List<Vector2> positions = new List<Vector2>();
    RectTransform rectTr;
    Vector2 defaultPos;

    Image[] images;
    

    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
        defaultPos = rectTr.anchoredPosition;

        images = GetComponentsInChildren<Image>();
        foreach(Image image in images)
        {
            Debug.Log(image.name);
        }
    }


    //전체 맵 보여주기

    public void OpenWorldMap()
    {

    }

    //특정 맵 집중해서 보여주기 
    public void OpenMap(int mapid)
    {
        float scaler = 2.0f;
        rectTr.localScale = new Vector3(scaler, scaler, 1);
        rectTr.anchoredPosition = defaultPos + (-1 * positions[mapid] * scaler);
        foreach (var image in images)
        {
            image.color = deselectColor;
        }

        images[mapid].color = selectColor;

    }

    public void CloseMap()
    {
        rectTr.localScale = Vector3.one;
        rectTr.anchoredPosition = defaultPos;
    }
}
