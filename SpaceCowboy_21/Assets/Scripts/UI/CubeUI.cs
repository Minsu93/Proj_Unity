using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private Canvas canvas;
    private RectTransform cubeSlotRectTransform;
    
    public RectTransform rectTransform { get; private set; }
    private CanvasGroup canvasGroup;
    private Image image;
    private AlienTechCube cube;
    Vector2 startAnchoredPos;
    public bool dropInPosition { get; set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponentInChildren<Image>();

    }

    public void InitializeCube(Canvas canvas, RectTransform slotRect, AlienTechCube cube)
    {
        this.canvas = canvas;
        cubeSlotRectTransform = slotRect;
        this.cube = cube;
        image.sprite = cube.CubeSprite;

        startAnchoredPos = cubeSlotRectTransform.anchoredPosition;

    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Start");
        canvasGroup.blocksRaycasts = false;
        dropInPosition = false;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag End");
        if (!dropInPosition)
        {
            rectTransform.anchoredPosition = startAnchoredPos;
        }
        canvasGroup.blocksRaycasts = true;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Click");
    }

    public void OnDrop(PointerEventData eventData)
    {

    }
}
