using Pathfinding.Util;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponCubeSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler,IPointerExitHandler
{
    private RectTransform rectTransform;
    CubeUI dragItem;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop");
        if (dragItem != null)
        {
            dragItem.rectTransform.position = rectTransform.position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        if (eventData.pointerDrag != null)
        {
            //RectTransform rect = eventData.pointerDrag.GetComponent<RectTransform>();
            //rect.anchoredPosition = rectTransform.anchoredPosition;
            //rect.position = rectTransform.position;

            dragItem = eventData.pointerDrag.GetComponent<CubeUI>();
            dragItem.dropInPosition = true;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit");
        if (dragItem != null)
        {
            dragItem.dropInPosition = false;
        }
        dragItem = null;
    }
}
