using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : InteractableOBJ
{
    /// <summary>
    /// UI 화면을 켤때 업데이트 하거나, 로비 화면 시작 시 업데이트하거나.
    /// </summary>

    private bool uiOpened;
    [SerializeField] private Canvas UICanvas;

    public event Action UiUpdateEvent;
    
    public override void InteractAction()
    {
        if(uiOpened)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    private void OpenUI()
    {
        uiOpened = true;
        UICanvas.gameObject.SetActive(true);
        UpdateTechImage();
        GameManager.Instance.playerManager.DisablePlayerInput();

    }
    private void CloseUI()
    {
        uiOpened = false;
        UICanvas.gameObject.SetActive(false);
        GameManager.Instance.playerManager.EnablePlayerInput();

    }

    public override void StopInteract()
    {
        base.StopInteract();
        CloseUI();
    }

    /// <summary>
    /// 내용을 업데이트한다.
    /// </summary>
    public void UpdateTechImage()
    {
        if (UiUpdateEvent != null) UiUpdateEvent();
    }

}
