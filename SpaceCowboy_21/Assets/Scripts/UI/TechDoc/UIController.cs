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
    [SerializeField] private Canvas techDocUICanvas;

    public event Action ImageUpdate;
    
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
        techDocUICanvas.gameObject.SetActive(true);
        UpdateTechImage();
        GameManager.Instance.playerManager.DisablePlayerInput();

    }
    private void CloseUI()
    {
        uiOpened = false;
        techDocUICanvas.gameObject.SetActive(false);
        GameManager.Instance.playerManager.EnablePlayerInput();

    }


    /// <summary>
    /// 내용을 업데이트한다.
    /// </summary>
    public void UpdateTechImage()
    {
        if (ImageUpdate != null) ImageUpdate();
    }

}
