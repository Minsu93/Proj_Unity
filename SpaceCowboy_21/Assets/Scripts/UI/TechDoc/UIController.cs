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
    /// ������ ������Ʈ�Ѵ�.
    /// </summary>
    public void UpdateTechImage()
    {
        if (ImageUpdate != null) ImageUpdate();
    }

}
