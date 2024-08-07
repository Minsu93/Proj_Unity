using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : InteractableOBJ
{
    /// <summary>
    /// UI ȭ���� �Ӷ� ������Ʈ �ϰų�, �κ� ȭ�� ���� �� ������Ʈ�ϰų�.
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
    /// ������ ������Ʈ�Ѵ�.
    /// </summary>
    public void UpdateTechImage()
    {
        if (UiUpdateEvent != null) UiUpdateEvent();
    }

}
