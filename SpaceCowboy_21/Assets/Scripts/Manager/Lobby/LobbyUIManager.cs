using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField]
    UiPanel stageUIPanel;
    [SerializeField]
    UiPanel[] stagePanels;

    [SerializeField]
    CanvasGroup stageCanvasGroup;

    /// <summary>
    /// 탭을 연다
    /// </summary>
    /// <param name="panel"></param>
    public void OpenPanel(int index)
    {
        stagePanels[index].opened = true;
        stagePanels[index].panel.SetActive(true);
        stageCanvasGroup.interactable = false;
    }

    /// <summary>
    /// 열려있는 탭을 닫는다. 
    /// </summary>
    public void CloseAllTab()
    {
        for (int i = 0; i < stagePanels.Length; i++)
        {
            //열려 있으면 닫는다.
            if (stagePanels[i].opened)
            {
                stagePanels[i].opened = false;
                stagePanels[i].panel.SetActive(false);
            }
        }

        //메인 메뉴 활성화
        stageCanvasGroup.interactable = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllTab();
        }
    }

}

[Serializable]
public struct UiPanel
{
    [HideInInspector] public bool opened;
    public GameObject panel;
}
